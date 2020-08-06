// <copyright file="TransientTenantManagerSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Services.Tenancy.Specs.Steps
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Marain.TenantManagement;
    using Marain.TenantManagement.ServiceManifests;
    using Marain.TenantManagement.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class TransientTenantManagerSteps
    {
        private readonly ScenarioContext scenarioContext;
        private readonly FeatureContext featureContext;

        public TransientTenantManagerSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
            this.featureContext = featureContext;
        }

        [When("I create a transient service tenant using the embedded resource '(.*)'")]
        public async Task WhenICreateATransientServiceTenantUsingTheEmbeddedResource(string resourceName)
        {
            // Retrieve the manifest and stash it in the scenario - we'll need it later.
            ServiceManifest manifest = this.GetServiceManifestFromEmbeddedResource(resourceName);
            this.scenarioContext.Set(manifest);

            var transientTenantManager = TransientTenantManager.GetInstance(this.featureContext);
            await transientTenantManager.EnsureInitialisedAsync().ConfigureAwait(false);
            ITenant tenant = await transientTenantManager.CreateTransientServiceTenantFromEmbeddedResourceAsync(
                typeof(TransientTenantManagerSteps).Assembly,
                resourceName).ConfigureAwait(false);

            this.scenarioContext.Set(tenant);
        }

        [Then("the transient tenant is created")]
        public void ThenTheTransientTenantIsCreated()
        {
            Assert.IsNotNull(this.scenarioContext.Get<ITenant>());
        }

        [Then("the transient tenant Id is different from the service as described in the manifest")]
        public void ThenTheTransientTenantIdIsDifferentFromTheServiceAsDescribedInTheManifest()
        {
            ServiceManifest manifest = this.scenarioContext.Get<ServiceManifest>();
            string expectedServiceTenantId = WellKnownTenantIds.ServiceTenantParentId.CreateChildId(manifest.WellKnownTenantGuid);

            ITenant transientTenant = this.scenarioContext.Get<ITenant>();
            Assert.AreNotEqual(expectedServiceTenantId, transientTenant.Id);
        }

        private ServiceManifest GetServiceManifestFromEmbeddedResource(string resourceName)
        {
            IJsonSerializerSettingsProvider serializationSettingsProvider =
                ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IJsonSerializerSettingsProvider>();

            using Stream stream = typeof(TransientTenantManagerSteps).Assembly.GetManifestResourceStream(resourceName)
                ?? throw new ArgumentException($"Could not find an embedded resource named '{resourceName}'");
            using var reader = new StreamReader(stream);

            string manifestJson = reader.ReadToEnd();

            ServiceManifest manifest = JsonConvert.DeserializeObject<ServiceManifest>(
                manifestJson,
                serializationSettingsProvider.Instance);

            return manifest;
        }
    }
}
