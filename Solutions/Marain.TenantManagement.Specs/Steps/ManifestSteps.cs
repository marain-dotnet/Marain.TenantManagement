// <copyright file="ManifestSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Specs.Steps
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.Tenancy;
    using Corvus.Testing.SpecFlow;

    using Marain.TenantManagement.Configuration;
    using Marain.TenantManagement.EnrollmentConfiguration;
    using Marain.TenantManagement.ServiceManifests;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

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
            this.GivenIHaveAServiceManifestCalled(manifestName, null!);
        }

        [Given("I have a service manifest called '(.*)' for a service called '(.*)'")]
        [Given("I have a legacy V2 service manifest called '([^']*)' for a service called '([^']*)'")]
        public void GivenIHaveAServiceManifestCalled(string manifestName, string serviceName)
        {
            if (serviceName != null)
            {
                serviceName = serviceName.TrimStart('"').TrimEnd('"');
            }

            var manifest = new ServiceManifest
            {
                WellKnownTenantGuid = Guid.NewGuid(),
                ServiceName = serviceName,
            };

            this.namedManifests.Add(manifestName, manifest);
        }

        [Given("the well-known tenant Guid for the manifest called '(.*)' is '(.*)'")]
        [Given("the well-known tenant Guid for the legacy V2 manifest called '([^']*)' is '([^']*)'")]
        public void GivenTheWell_KnownTenantGuidForTheManifestCalledIs(string manifestName, Guid wellKnownTenantGuid)
        {
            ServiceManifest manifest = this.NamedManifest(manifestName);
            manifest.WellKnownTenantGuid = wellKnownTenantGuid;
        }

        [Given("the service manifest called '(.*)' has the following dependencies")]
        [Given("the legacy V2 service manifest called '([^']*)' has the following dependencies")]
        public void GivenTheServiceManifestCalledHasTheFollowingDependencies(string manifestName, Table dependencyTable)
        {
            ServiceManifest manifest = this.NamedManifest(manifestName);

            foreach (TableRow row in dependencyTable.Rows)
            {
                manifest.DependsOnServiceTenants.Add(new ServiceDependency
                {
                    Id = row[0],
                    ExpectedName = row[1],
                });
            }
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
            ServiceManifest manifest = this.NamedManifest(manifestName);

            manifest.RequiredConfigurationEntries.Add(new ServiceManifestBlobStorageConfigurationEntry
            {
                Key = table.Rows[0]["Key"],
                Description = table.Rows[0]["Description"],
            });
        }

        [Given("the service manifest called '([^']*)' has the following legacy V2 Azure Blob Storage configuration entries")]
        public void GivenTheServiceManifestCalledHasTheFollowingLegacyV2AzureBlobStorageConfigurationEntries(
            string manifestName, Table table)
        {
            ServiceManifest manifest = this.NamedManifest(manifestName);

            manifest.RequiredConfigurationEntries.Add(new ServiceManifestLegacyV2BlobStorageConfigurationEntry
            {
                Key = table.Rows[0]["Key"],
                Description = table.Rows[0]["Description"],
                ContainerDefinition = new LegacyV2BlobStorageContainerDefinition(table.Rows[0]["Container Name"]),
            });
        }

        [Given("the service manifest called '(.*)' has the following Azure Table Storage configuration entries")]
        public void GivenTheServiceManifestCalledHasTheFollowingAzureTableStorageConfigurationEntries(string manifestName, Table table)
        {
            ServiceManifest manifest = this.NamedManifest(manifestName);

            manifest.RequiredConfigurationEntries.Add(new ServiceManifestTableStorageConfigurationEntry
            {
                Key = table.Rows[0]["Key"],
                Description = table.Rows[0]["Description"],
            });
        }

        [Given("the service manifest called '(.*)' has the following legacy V2 Azure Table Storage configuration entries")]
        public void GivenTheServiceManifestCalledHasTheFollowingLegacyV2AzureTableStorageConfigurationEntries(
            string manifestName, Table table)
        {
            ServiceManifest manifest = this.NamedManifest(manifestName);

            manifest.RequiredConfigurationEntries.Add(new ServiceManifestLegacyV2TableStorageConfigurationEntry
            {
                Key = table.Rows[0]["Key"],
                Description = table.Rows[0]["Description"],
                ContainerDefinition = new LegacyV2TableStorageTableDefinition(table.Rows[0]["Table Name"]),
            });
        }

        [Given("the service manifest called '(.*)' has the following Azure CosmosDb Storage configuration entries")]
        public void GivenTheServiceManifestCalledHasTheFollowingAzureCosmosDbStorageConfigurationEntries(string manifestName, Table table)
        {
            ServiceManifest manifest = this.NamedManifest(manifestName);

            manifest.RequiredConfigurationEntries.Add(new ServiceManifestCosmosDbConfigurationEntry
            {
                Key = table.Rows[0]["Key"],
                Description = table.Rows[0]["Description"],
            });
        }

        [Given("the service manifest called '(.*)' has the following legacy V2 Azure CosmosDb Storage configuration entries")]
        public void GivenTheServiceManifestCalledHasTheFollowingLegacyV2AzureCosmosDbStorageConfigurationEntries(
            string manifestName, Table table)
        {
            ServiceManifest manifest = this.NamedManifest(manifestName);

            manifest.RequiredConfigurationEntries.Add(new ServiceManifestLegacyV2CosmosDbConfigurationEntry
            {
                Key = table.Rows[0]["Key"],
                Description = table.Rows[0]["Description"],
                ContainerDefinition = new LegacyV2CosmosContainerDefinition(
                    table.Rows[0]["Database Name"],
                    table.Rows[0]["Container Name"],
                    null,
                    null,
                    null),
            });
        }

        private ServiceManifest LoadManifestFile(string manifestName)
        {
            IJsonSerializerSettingsProvider settingsProvider =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<IJsonSerializerSettingsProvider>();

            using Stream manifestStream = this.GetType().Assembly.GetManifestResourceStream($"Marain.TenantManagement.Specs.Data.ServiceManifests.{manifestName}.jsonc")
                ?? throw new ArgumentException($"Could not find a resource in the Marain.TenantManagement.Specs.Data.ServiceManifests namespace called {manifestName}.jsonc");

            using var manifestStreamReader = new StreamReader(manifestStream);

            string manifestJson = manifestStreamReader.ReadToEnd();

            return JsonConvert.DeserializeObject<ServiceManifest>(manifestJson, settingsProvider.Instance);
        }
    }
}