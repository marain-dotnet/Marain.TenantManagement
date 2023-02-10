// <copyright file="ManifestSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Specs.Steps
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Corvus.Json.Serialization;
    using Corvus.Tenancy;
    using Corvus.Testing.SpecFlow;

    using Marain.TenantManagement.ServiceManifests;

    using Microsoft.Extensions.DependencyInjection;

    using NUnit.Framework;

    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

    [Binding]
    public class ManifestSteps
    {
        private readonly ScenarioContext scenarioContext;
        private readonly Dictionary<string, ServiceManifest> namedManifests = new();
        private ServiceManifest? manifest;

        public ManifestSteps(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }

        public ServiceManifest Manifest => this.manifest ?? throw new InvalidOperationException("This test has not loaded or deserialized an unnamed manifest");

        public ServiceManifest NamedManifest(string name) => this.namedManifests.TryGetValue(name, out ServiceManifest? serviceManifest)
            ? serviceManifest
            : throw new InvalidOperationException($"This test has not loaded or deserialized a ServiceManifest called {name}");

        [Given("I have a service manifest called '(.*)' with no service name")]
        [Given("I have a legacy V2 service manifest called '([^']*)' with no service name")]
        public void GivenIHaveAServiceManifestCalled(string manifestName)
        {
            this.GivenIHaveAServiceManifestCalled(manifestName, null);
        }

        [Given("I have a service manifest called '(.*)' for a service called '(.*)'")]
        [Given("I have a legacy V2 service manifest called '([^']*)' for a service called '([^']*)'")]
        public void GivenIHaveAServiceManifestCalled(string manifestName, string? serviceName)
        {
            serviceName = serviceName is null
                ? string.Empty
                : serviceName.TrimStart('"').TrimEnd('"');

            var manifest = new ServiceManifest(
                WellKnownTenantGuid: Guid.NewGuid(),
                ServiceName: serviceName);

            this.namedManifests.Add(manifestName, manifest);
        }

        [Given("the well-known tenant Guid for the manifest called '(.*)' is '(.*)'")]
        [Given("the well-known tenant Guid for the legacy V2 manifest called '([^']*)' is '([^']*)'")]
        public void GivenTheWell_KnownTenantGuidForTheManifestCalledIs(string manifestName, Guid wellKnownTenantGuid)
        {
            this.ModifyManifest(
                manifestName,
                m => m with { WellKnownTenantGuid = wellKnownTenantGuid });
        }

        [Given("the service manifest called '(.*)' has the following dependencies")]
        [Given("the legacy V2 service manifest called '([^']*)' has the following dependencies")]
        public void GivenTheServiceManifestCalledHasTheFollowingDependencies(string manifestName, Table dependencyTable)
        {
            this.ModifyManifest(
                manifestName,
                m => m with
                {
                    DependsOnServiceTenants = dependencyTable
                        .CreateSet<(string Id, string Name)>()
                        .Select(kd => new ServiceDependency(kd.Id, kd.Name))
                        .ToList(),
                });
        }

        [When("I validate the service manifest called '(.*)'")]
        [When("I validate the legacy V2 service manifest called '([^']*)'")]
        public async Task WhenIValidateTheServiceManifestCalled(string manifestName)
        {
            ITenantStore store = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantStore>();
            ServiceManifest manifest = this.NamedManifest(manifestName);

            await CatchException.AndStoreInScenarioContextAsync(
                this.scenarioContext,
                () => manifest.ValidateAndThrowAsync(store));
        }

        [When("I deserialize the manifest called '(.*)'")]
        public void WhenIDeserializeTheManifestCalled(string manifestName)
        {
            this.manifest = this.LoadManifestFile(manifestName);
        }

        [When("I deserialize the manifest called '(.*)' anticipating an exception")]
        public void WhenIDeserializeTheManifestCalledAnticipatingException(string manifestName)
        {
            CatchException.AndStoreInScenarioContext(
                this.scenarioContext,
                () => this.manifest = this.LoadManifestFile(manifestName));
        }

        [Given("I have loaded the manifest called '(.*)'")]
        public void GivenIHaveLoadedTheManifestCalled(string manifestName)
        {
            ServiceManifest manifest = this.LoadManifestFile(manifestName);
            this.namedManifests.Add(manifestName, manifest);
        }

        [Then("the resulting manifest should have the service name '(.*)'")]
        public void ThenTheResultingManifestShouldHaveTheServiceName(string serviceName)
        {
            Assert.AreEqual(serviceName, this.Manifest.ServiceName);
        }

        [Then("the resulting manifest should have a well known service GUID of '(.*)'")]
        public void ThenTheResultingManifestShouldHaveAWellKnownServiceGUIDOf(Guid expectedWellKnownServiceGuid)
        {
            Assert.AreEqual(expectedWellKnownServiceGuid, this.Manifest.WellKnownTenantGuid);
        }

        [Then("the resulting manifest should not have any dependencies")]
        public void ThenTheResultingManifestShouldNotHaveAnyDependencies()
        {
            this.ThenTheResultingManifestShouldHaveDependencies(0);
        }

        [Then("the resulting manifest should not have any required configuration entries")]
        public void ThenTheResultingManifestShouldNotHaveAnyRequiredConfigurationEntries()
        {
            Assert.IsEmpty(this.Manifest.RequiredConfigurationEntries);
        }

        [Then("the resulting manifest should have (.*) dependencies")]
        public void ThenTheResultingManifestShouldHaveDependencies(int expectedDependencyCount)
        {
            Assert.AreEqual(expectedDependencyCount, this.Manifest.DependsOnServiceTenants.Count);
        }

        [Then("the resulting manifest should have (.*) required configuration entry")]
        [Then("the resulting manifest should have (.*) required configuration entries")]
        public void ThenTheResultingManifestShouldHaveRequiredConfigurationEntry(int expectedConfigurationEntryCount)
        {
            Assert.AreEqual(expectedConfigurationEntryCount, this.Manifest.RequiredConfigurationEntries.Count);
        }

        [Then("the configuration item with index (.*) should be of type '(.*)'")]
        public void ThenTheConfigurationItemWithIndexShouldBeOfType(int index, string expectedTypeName)
        {
            ServiceManifestRequiredConfigurationEntry configurationItem = this.Manifest.RequiredConfigurationEntries[index];

            Assert.AreEqual(expectedTypeName, configurationItem.GetType().Name);
        }

        [Then("the resulting manifest should have a dependency with Id '(.*)'")]
        public void ThenTheResultingManifestShouldHaveADependencyWithId(string expectedDependencyId)
        {
            Assert.IsTrue(this.Manifest.DependsOnServiceTenants.Any(x => x.Id == expectedDependencyId));
        }

        [Then("the ServiceManifestBlobStorageConfigurationEntry at index (.*) should have a null LegacyV2Key")]
        public void ThenTheServiceManifestBlobStorageConfigurationEntryAtIndexShouldHaveANullLegacyV2Key(
            int index)
        {
            var configuration = (ServiceManifestBlobStorageConfigurationEntry)this.Manifest.RequiredConfigurationEntries[index];
            Assert.IsNull(configuration.LegacyV2Key);
        }

        [Then("the ServiceManifestBlobStorageConfigurationEntry at index (.*) should have a LegacyV2Key of '([^']*)'")]
        public void ThenTheServiceManifestBlobStorageConfigurationEntryAtIndexShouldHaveALegacyV2KeyOf(
            int index, string expectedLegacyV2Key)
        {
            var configuration = (ServiceManifestBlobStorageConfigurationEntry)this.Manifest.RequiredConfigurationEntries[index];
            Assert.AreEqual(expectedLegacyV2Key, configuration.LegacyV2Key);
        }

        [Then("the ServiceManifestCosmosDbConfigurationEntry at index (.*) should have a null LegacyV2Key")]
        public void ThenTheServiceManifestCosmosDbConfigurationEntryAtIndexShouldHaveNullLegacyV2Key(
            int index)
        {
            var configuration = (ServiceManifestCosmosDbConfigurationEntry)this.Manifest.RequiredConfigurationEntries[index];
            Assert.IsNull(configuration.LegacyV2Key);
        }

        [Then("the ServiceManifestCosmosDbConfigurationEntry at index (.*) should have a LegacyV2Key of '([^']*)'")]
        public void ThenTheServiceManifestCosmosDbConfigurationEntryAtIndexShouldHaveALegacyV2KeyOf(
            int index, string expectedLegacyV2Key)
        {
            var configuration = (ServiceManifestCosmosDbConfigurationEntry)this.Manifest.RequiredConfigurationEntries[index];
            Assert.AreEqual(expectedLegacyV2Key, configuration.LegacyV2Key);
        }

        [Then("the ServiceManifestTableStorageConfigurationEntry at index (.*) should have a null LegacyV2Key")]
        public void ThenTheServiceManifestTableStorageConfigurationEntryAtIndexShouldHaveSupportsLegacyVConfigurationOf(
            int index)
        {
            var configuration = (ServiceManifestTableStorageConfigurationEntry)this.Manifest.RequiredConfigurationEntries[index];
            Assert.IsNull(configuration.LegacyV2Key);
        }

        [Then("the ServiceManifestTableStorageConfigurationEntry at index (.*) should have a LegacyV2Key of '([^']*)'")]
        public void ThenTheServiceManifestTableStorageConfigurationEntryAtIndexShouldHaveSupportsLegacyVConfigurationOf(
            int index, string expectedLegacyV2Key)
        {
            var configuration = (ServiceManifestTableStorageConfigurationEntry)this.Manifest.RequiredConfigurationEntries[index];
            Assert.AreEqual(expectedLegacyV2Key, configuration.LegacyV2Key);
        }

        [Given("the service manifest called '(.*)' has the following Azure Blob Storage configuration entries")]
        public void GivenTheServiceManifestCalledHasTheFollowingAzureBlobStorageConfigurationEntries(
            string manifestName, Table table)
        {
            this.ModifyManifest(
                manifestName,
                m => m with
                {
                    RequiredConfigurationEntries = table
                        .CreateSet<(string Key, string Description)>()
                        .Select(kd => new ServiceManifestBlobStorageConfigurationEntry(kd.Key, kd.Description))
                        .ToList<ServiceManifestRequiredConfigurationEntry>(),
                });
        }

        [Given("the service manifest called '([^']*)' has the following legacy V2 Azure Blob Storage configuration entries")]
        public void GivenTheServiceManifestCalledHasTheFollowingLegacyV2AzureBlobStorageConfigurationEntries(
            string manifestName, Table table)
        {
            this.ModifyManifest(
                manifestName,
                m => m with
                {
                    RequiredConfigurationEntries = table
                        .CreateSet<(string Key, string Description)>()
                        .Select(kd => new ServiceManifestLegacyV2BlobStorageConfigurationEntry(kd.Key, kd.Description))
                        .ToList<ServiceManifestRequiredConfigurationEntry>(),
                });
        }

        [Given("the service manifest called '(.*)' has the following Azure Table Storage configuration entries")]
        public void GivenTheServiceManifestCalledHasTheFollowingAzureTableStorageConfigurationEntries(string manifestName, Table table)
        {
            this.ModifyManifest(
                manifestName,
                m => m with
                {
                    RequiredConfigurationEntries = table
                        .CreateSet<(string Key, string Description)>()
                        .Select(kd => new ServiceManifestTableStorageConfigurationEntry(kd.Key, kd.Description))
                        .ToList<ServiceManifestRequiredConfigurationEntry>(),
                });
        }

        [Given("the service manifest called '(.*)' has the following legacy V2 Azure Table Storage configuration entries")]
        public void GivenTheServiceManifestCalledHasTheFollowingLegacyV2AzureTableStorageConfigurationEntries(
            string manifestName, Table table)
        {
            this.ModifyManifest(
                manifestName,
                m => m with
                {
                    RequiredConfigurationEntries = table
                        .CreateSet<(string Key, string Description)>()
                        .Select(kd => new ServiceManifestLegacyV2TableStorageConfigurationEntry(kd.Key, kd.Description))
                        .ToList<ServiceManifestRequiredConfigurationEntry>(),
                });
        }

        [Given("the service manifest called '(.*)' has the following Azure CosmosDb Storage configuration entries")]
        public void GivenTheServiceManifestCalledHasTheFollowingAzureCosmosDbStorageConfigurationEntries(string manifestName, Table table)
        {
            this.ModifyManifest(
                manifestName,
                m => m with
                {
                    RequiredConfigurationEntries = table
                        .CreateSet<(string Key, string Description)>()
                        .Select(kd => new ServiceManifestCosmosDbConfigurationEntry(kd.Key, kd.Description))
                        .ToList<ServiceManifestRequiredConfigurationEntry>(),
                });
        }

        [Given("the service manifest called '(.*)' has the following legacy V2 Azure CosmosDb Storage configuration entries")]
        public void GivenTheServiceManifestCalledHasTheFollowingLegacyV2AzureCosmosDbStorageConfigurationEntries(
            string manifestName, Table table)
        {
            this.ModifyManifest(
                manifestName,
                m => m with
                {
                    RequiredConfigurationEntries = table
                        .CreateSet<(string Key, string Description)>()
                        .Select(kd => new ServiceManifestLegacyV2CosmosDbConfigurationEntry(kd.Key, kd.Description))
                        .ToList<ServiceManifestRequiredConfigurationEntry>(),
                });
        }

        private ServiceManifest LoadManifestFile(string manifestName)
        {
            IJsonSerializerOptionsProvider settingsProvider =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<IJsonSerializerOptionsProvider>();

            using Stream manifestStream = this.GetType().Assembly.GetManifestResourceStream($"Marain.TenantManagement.Specs.Data.ServiceManifests.{manifestName}.jsonc")
                ?? throw new ArgumentException($"Could not find a resource in the Marain.TenantManagement.Specs.Data.ServiceManifests namespace called {manifestName}.jsonc");

            using var manifestStreamReader = new StreamReader(manifestStream);

            string manifestJson = manifestStreamReader.ReadToEnd();

            return JsonSerializer.Deserialize<ServiceManifest>(manifestJson, settingsProvider.Instance)!;
        }

        private void ModifyManifest(string manifestName, Func<ServiceManifest, ServiceManifest> modify)
        {
            this.namedManifests[manifestName] = modify(this.NamedManifest(manifestName));
        }
    }
}