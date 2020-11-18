﻿namespace Marain.TenantManagement.Specs.Steps
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.Azure.Storage.Tenancy;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Marain.TenantManagement.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using TechTalk.SpecFlow;

    [Binding]
    public class AddOrUpdateStorageConfigurationSteps
    {
        private readonly ScenarioContext scenarioContext;

        public AddOrUpdateStorageConfigurationSteps(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }

        [Given("I have configuration called '(.*)'")]
        public void GivenIHaveConfigurationCalled(string configurationName)
        {
            this.scenarioContext.Set(new List<ConfigurationItem>(), configurationName);
        }

        [Given("the configuration called '(.*)' contains the following Blob Storage configuration items")]
        public void GivenTheConfigurationCalledContainsTheFollowingBlobStorageConfigurationItems(string configurationName, Table configurationEntries)
        {
            List<ConfigurationItem> configuration =
                this.scenarioContext.Get<List<ConfigurationItem>>(configurationName);

            configuration.AddRange(
                configurationEntries.Rows.Select(
                    row => new BlobStorageConfigurationItem
                    {
                        Definition = new BlobStorageContainerDefinition(row["Definition - Container"]),
                        Configuration = new BlobStorageConfiguration
                        {
                            AccountName = row["Configuration - Account Name"],
                            Container = row["Configuration - Container"],
                        },
                    }));
        }

        [Given("the configuration called '(.*)' contains the following Table Storage configuration items")]
        public void GivenTheConfigurationCalledContainsTheFollowingTableStorageConfigurationItems(string configurationName, Table configurationEntries)
        {
            List<ConfigurationItem> configuration =
                this.scenarioContext.Get<List<ConfigurationItem>>(configurationName);

            configuration.AddRange(
                configurationEntries.Rows.Select(
                    row => new TableStorageConfigurationItem
                    {
                        Definition = new TableStorageTableDefinition(row["Definition - Table"]),
                        Configuration = new TableStorageConfiguration
                        {
                            AccountName = row["Configuration - Account Name"],
                            TableName = row["Configuration - Table"],
                        },
                    }));
        }

        [Given("the configuration called '(.*)' contains the following Cosmos configuration items")]
        public void GivenTheConfigurationCalledContainsTheFollowingCosmosConfigurationItems(string configurationName, Table configurationEntries)
        {
            List<ConfigurationItem> configuration =
                this.scenarioContext.Get<List<ConfigurationItem>>(configurationName);

            configuration.AddRange(
                configurationEntries.Rows.Select(
                    row => new CosmosConfigurationItem
                    {
                        Definition = new CosmosContainerDefinition(row["Definition - Database"], row["Definition - Container"], null),
                        Configuration = new CosmosConfiguration
                        {
                            AccountUri = row["Configuration - Account Uri"],
                            DatabaseName = row["Configuration - Database"],
                            ContainerName = row["Configuration - Container"],
                        },
                    }));
        }

        [When("I use the tenant management service with the configuration called '(.*)' to add config for the tenant called '(.*)'")]
        public Task WhenIUseTheTenantManagementServiceWithTheConfigurationCalledToAddConfigForTheTenantCalled(string configurationName, string tenantName)
        {
            return this.AddConfiguration(tenantName, configurationName);
        }

        private async Task AddConfiguration(
            string tenantName,
            string configurationName)
        {
            ITenantManagementService managementService =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantManagementService>();
            ITenantProvider tenantProvider = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantProvider>();

            ITenant tenant = await tenantProvider.GetTenantAsync(this.scenarioContext.Get<string>(tenantName)).ConfigureAwait(false);

            List<ConfigurationItem> configuration = this.scenarioContext.Get<List<ConfigurationItem>>(configurationName);

            await CatchException.AndStoreInScenarioContextAsync(
                this.scenarioContext,
                () => managementService.AddOrUpdateStorageConfigurationAsync(
                    tenant,
                    configuration.ToArray())).ConfigureAwait(false);
        }
    }
}