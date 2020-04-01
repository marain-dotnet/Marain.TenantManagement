// <copyright file="ManifestSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Specs.Steps
{
    using System;
    using System.Threading.Tasks;
    using Corvus.SpecFlow.Extensions;
    using Marain.TenantManagement.ServiceManifests;
    using Microsoft.Extensions.DependencyInjection;
    using TechTalk.SpecFlow;

    [Binding]
    public class ManifestSteps
    {
        private readonly ScenarioContext scenarioContext;

        public ManifestSteps(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }

        [When("I create a service manifest without providing a service name")]
        public void WhenICreateAServiceManifestWithoutProvidingAServiceName()
        {
            this.WhenICreateAServiceManifestWithServiceName(null);
        }

        [When("I create a service manifest with service name '(.*)'")]
        public void WhenICreateAServiceManifestWithServiceName(string? serviceName)
        {
            if (serviceName != null)
            {
                serviceName = serviceName.TrimStart('"').TrimEnd('"');
            }

            CatchException.AndStoreInScenarioContext(
                this.scenarioContext,
                () => new ServiceManifest(serviceName!));
        }

        [Given("I have a service manifest called '(.*)' for a service called '(.*)'")]
        public void GivenIHaveAServiceManifestCalled(string manifestName, string serviceName)
        {
            var manifest = new ServiceManifest(serviceName);
            this.scenarioContext.Set(manifest, manifestName);
        }

        [Given("the service manifest called '(.*)' has the following dependencies")]
        public void GivenTheServiceManifestCalledHasTheFollowingDependencies(string manifestName, Table dependencyTable)
        {
            ServiceManifest manifest = this.scenarioContext.Get<ServiceManifest>(manifestName);

            foreach (TableRow row in dependencyTable.Rows)
            {
                manifest.DependsOnServiceNames.Add(row[0]);
            }
        }

        [When("I validate the service manifest called '(.*)'")]
        public Task WhenIValidateTheServiceManifestCalled(string manifestName)
        {
            ITenantManagementService service = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantManagementService>();
            ServiceManifest manifest = this.scenarioContext.Get<ServiceManifest>(manifestName);

            return CatchException.AndStoreInScenarioContextAsync(
                this.scenarioContext,
                () => manifest.ValidateAndThrowAsync(service));
        }
    }
}
