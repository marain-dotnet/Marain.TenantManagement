﻿// <copyright file="ManifestSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Specs.Steps
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.Azure.Storage.Tenancy;
    using Corvus.Extensions.Json;
    using Corvus.SpecFlow.Extensions;
    using Marain.TenantManagement.ServiceManifests;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class ManifestSteps
    {
        private readonly ScenarioContext scenarioContext;

        public ManifestSteps(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }

        [Given("I have a service manifest called '(.*)' with no service name")]
        public void GivenIHaveAServiceManifestCalled(string manifestName)
        {
            this.GivenIHaveAServiceManifestCalled(manifestName, null!);
        }

        [Given("I have a service manifest called '(.*)' for a service called '(.*)'")]
        public void GivenIHaveAServiceManifestCalled(string manifestName, string serviceName)
        {
            if (serviceName != null)
            {
                serviceName = serviceName.TrimStart('"').TrimEnd('"');
            }

            var manifest = new ServiceManifest { ServiceName = serviceName };
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

        [When("I deserialize the manifest called '(.*)'")]
        public void WhenIDeserializeTheManifestCalled(string manifestName)
        {
            ServiceManifest manifest = this.LoadManifestFile(manifestName);
            this.scenarioContext.Set(manifest);
        }

        [Given("I have loaded the manifest called '(.*)'")]
        public void GivenIHaveLoadedTheManifestCalled(string manifestName)
        {
            ServiceManifest manifest = this.LoadManifestFile(manifestName);
            this.scenarioContext.Set(manifest, manifestName);
        }

        [Then("the resulting manifest should have the service name '(.*)'")]
        public void ThenTheResultingManifestShouldHaveTheServiceName(string serviceName)
        {
            ServiceManifest manifest = this.scenarioContext.Get<ServiceManifest>();

            Assert.AreEqual(serviceName, manifest.ServiceName);
        }

        [Then("the resulting manifest should not have any dependencies")]
        public void ThenTheResultingManifestShouldNotHaveAnyDependencies()
        {
            this.ThenTheResultingManifestShouldHaveDependencies(0);
        }

        [Then("the resulting manifest should not have any required configuration entries")]
        public void ThenTheResultingManifestShouldNotHaveAnyRequiredConfigurationEntries()
        {
            ServiceManifest manifest = this.scenarioContext.Get<ServiceManifest>();

            Assert.IsEmpty(manifest.RequiredConfigurationEntries);
        }

        [Then("the resulting manifest should have (.*) dependencies")]
        public void ThenTheResultingManifestShouldHaveDependencies(int expectedDependencyCount)
        {
            ServiceManifest manifest = this.scenarioContext.Get<ServiceManifest>();

            Assert.AreEqual(expectedDependencyCount, manifest.DependsOnServiceNames.Count);
        }

        [Then("the resulting manifest should have (.*) required configuration entry")]
        [Then("the resulting manifest should have (.*) required configuration entries")]
        public void ThenTheResultingManifestShouldHaveRequiredConfigurationEntry(int expectedConfigurationEntryCount)
        {
            ServiceManifest manifest = this.scenarioContext.Get<ServiceManifest>();

            Assert.AreEqual(expectedConfigurationEntryCount, manifest.RequiredConfigurationEntries.Count);
        }

        [Then("the configuration item with index (.*) should be of type '(.*)'")]
        public void ThenTheConfigurationItemWithIndexShouldBeOfType(int index, string expectedTypeName)
        {
            ServiceManifest manifest = this.scenarioContext.Get<ServiceManifest>();

            ServiceManifestRequiredConfigurationEntry configurationItem = manifest.RequiredConfigurationEntries[index];

            Assert.AreEqual(expectedTypeName, configurationItem.GetType().Name);
        }

        [Then("the resulting manifest should have a dependency called '(.*)'")]
        public void ThenTheResultingManifestShouldHaveADependencyCalled(string expectedDependencyName)
        {
            ServiceManifest manifest = this.scenarioContext.Get<ServiceManifest>();

            Assert.IsTrue(manifest.DependsOnServiceNames.Contains(expectedDependencyName));
        }

        [Given("the service manifest called '(.*)' has the following Azure Blob Storage configuration entries")]
        public void GivenTheServiceManifestCalledHasTheFollowingAzureBlobStorageConfigurationEntries(string manifestName, Table table)
        {
            ServiceManifest manifest = this.scenarioContext.Get<ServiceManifest>(manifestName);

            manifest.RequiredConfigurationEntries.Add(new ServiceManifestBlobStorageConfigurationEntry
            {
                Key = table.Rows[0]["Key"],
                Description = table.Rows[0]["Description"],
                ContainerDefinition = new BlobStorageContainerDefinition(table.Rows[0]["Container Name"]),
            });
        }

        [Given("the service manifest called '(.*)' has the following Azure CosmosDb Storage configuration entries")]
        public void GivenTheServiceManifestCalledHasTheFollowingAzureCosmosDbStorageConfigurationEntries(string manifestName, Table table)
        {
            ServiceManifest manifest = this.scenarioContext.Get<ServiceManifest>(manifestName);

            manifest.RequiredConfigurationEntries.Add(new ServiceManifestCosmosDbConfigurationEntry
            {
                Key = table.Rows[0]["Key"],
                Description = table.Rows[0]["Description"],
                ContainerDefinition = new CosmosContainerDefinition(
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

            using Stream manifestStream = this.GetType().Assembly.GetManifestResourceStream($"Marain.TenantManagement.Specs.Data.ServiceManifests.{manifestName}.json")
                ?? throw new ArgumentException($"Could not find a resource in the Marain.TenantManagement.Specs.Data.ServiceManifests namespace called {manifestName}.json");

            using var manifestStreamReader = new StreamReader(manifestStream);

            string manifestJson = manifestStreamReader.ReadToEnd();

            return JsonConvert.DeserializeObject<ServiceManifest>(manifestJson, settingsProvider.Instance);
        }
    }
}
