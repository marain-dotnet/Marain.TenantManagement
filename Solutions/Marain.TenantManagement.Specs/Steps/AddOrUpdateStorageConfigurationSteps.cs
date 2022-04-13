namespace Marain.TenantManagement.Specs.Steps
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Storage.Azure.BlobStorage;
    using Corvus.Storage.Azure.Cosmos;
    using Corvus.Storage.Azure.TableStorage;
    using Corvus.Tenancy;
    using Corvus.Testing.SpecFlow;
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
                        ConfigurationKey = row["ConfigurationKey"],
                        Configuration = new BlobContainerConfiguration
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
                        ConfigurationKey = row["ConfigurationKey"],
                        Configuration = new TableConfiguration
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
                        ConfigurationKey = row["ConfigurationKey"],
                        Configuration = new CosmosContainerConfiguration
                        {
                            AccountUri = row["Configuration - Account Uri"],
                            Database = row["Configuration - Database"],
                            Container = row["Configuration - Container"],
                        },
                    }));
        }

        [When("I use the tenant store with the configuration called '(.*)' to add config for the tenant called '(.*)'")]
        public Task WhenIUseTheTenantStoreWithTheConfigurationCalledToAddConfigForTheTenantCalled(string configurationName, string tenantName)
        {
            return this.AddConfiguration(tenantName, configurationName);
        }

        private async Task AddConfiguration(
            string tenantName,
            string configurationName)
        {
            ITenantStore tenantStore =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantStore>();
            ITenantProvider tenantProvider = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantProvider>();

            ITenant tenant = await tenantProvider.GetTenantAsync(this.scenarioContext.Get<string>(tenantName)).ConfigureAwait(false);

            List<ConfigurationItem> configuration = this.scenarioContext.Get<List<ConfigurationItem>>(configurationName);

            await CatchException.AndStoreInScenarioContextAsync(
                this.scenarioContext,
                () => tenantStore.AddOrUpdateStorageConfigurationAsync(
                    tenant,
                    configuration.ToArray())).ConfigureAwait(false);
        }
    }
}