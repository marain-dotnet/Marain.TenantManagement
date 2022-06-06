// <copyright file="TenantSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Specs.Steps
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Storage.Azure.BlobStorage;
    using Corvus.Storage.Azure.BlobStorage.Tenancy;
    using Corvus.Storage.Azure.Cosmos;
    using Corvus.Storage.Azure.Cosmos.Tenancy;
    using Corvus.Storage.Azure.TableStorage;
    using Corvus.Storage.Azure.TableStorage.Tenancy;
    using Corvus.Tenancy;
    using Corvus.Tenancy.Exceptions;
    using Corvus.Testing.SpecFlow;
    using Marain.TenantManagement;
    using Marain.TenantManagement.ServiceManifests;
    using Marain.TenantManagement.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class TenantSteps
    {
        private readonly ScenarioContext scenarioContext;
        private readonly ManifestSteps manifestSteps;
        private readonly ITenantStore tenantStore;

        public TenantSteps(
            ScenarioContext scenarioContext,
            ManifestSteps manifestSteps)
        {
            this.scenarioContext = scenarioContext;
            this.manifestSteps = manifestSteps;

            this.tenantStore = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantStore>();
        }

        [When("I use the tenant store to create a new client tenant called '(.*)'")]
        public Task WhenIUseTheTenantStoreToCreateANewClientTenantCalled(string clientName)
        {
            return CatchException.AndStoreInScenarioContextAsync(
                this.scenarioContext,
                async () =>
                {
                    ITenant newTenant = await this.tenantStore.CreateClientTenantAsync(clientName).ConfigureAwait(false);
                    this.scenarioContext.Set(newTenant.Id, clientName);
                });
        }

        [Given("I have an existing client tenant with a well known Guid '(.*)' called '(.*)'")]
        [When("I use the tenant store to create a new client tenant with well known Guid '(.*)' called '(.*)'")]
        public Task WhenIUseTheTenantStoreToCreateANewClientTenantWithWellKnownGuidCalled(Guid wellKnownGuid, string clientName)
        {
            return this.CreateTenant(wellKnownGuid, clientName);
        }

        [Given("the client tenant '(.*)' does not exist")]
        public void GivenTheClientTenantDoesNotExist(string parentClientName)
        {
            this.scenarioContext.Set("FakeId", parentClientName);
        }

        [When("I use the tenant store to create a new child client tenant of the '(.*)' client tenant with well known Guid '(.*)' called '(.*)'")]
        public Task WhenIUseTheTenantStoreToCreateANewChildClientTenantOfTheClientTenantWithWellKnownGuidCalled(string parentClientName, Guid wellKnownGuid, string clientName)
        {
            string? parentId = this.scenarioContext.Get<string>(parentClientName);

            return this.CreateTenant(wellKnownGuid, clientName, parentId);
        }

        [Given("I have used the tenant store to create a new client tenant called '(.*)'")]
        public async Task GivenIHaveUsedTheTenantStoreToCreateANewClientTenantCalled(string clientName)
        {
            ITenant newTenant = await this.tenantStore.CreateClientTenantAsync(clientName).ConfigureAwait(false);
            this.scenarioContext.Set(newTenant.Id, clientName);
        }

        [Given("I have used the tenant store to create a service tenant with manifest '(.*)'")]
        [Given("I have used the tenant store to create a service tenant with legacy V2 manifest '([^']*)'")]
        public async Task GivenIHaveUsedTheTenantStoreToCreateAServiceTenantWithManifest(string manifestName)
        {
            ServiceManifest manifest = this.manifestSteps.NamedManifest(manifestName);

            ITenant newTenant = await this.tenantStore.CreateServiceTenantAsync(manifest).ConfigureAwait(false);
            this.scenarioContext.Set(newTenant.Id, manifest.ServiceName);
        }

        [Given("I have loaded the manifest called '(.*)' and used the tenant store to create a service tenant with it")]
        public async Task GivenIHaveLoadedTheManifestCalled(string manifestName)
        {
            this.manifestSteps.GivenIHaveLoadedTheManifestCalled(manifestName);
            await this.GivenIHaveUsedTheTenantStoreToCreateAServiceTenantWithManifest(manifestName).ConfigureAwait(false);
        }

        [Given("I have an existing service tenant with manifest '(.*)'")]
        [When("I use the tenant store to create a new service tenant with manifest '(.*)'")]
        public Task WhenIUseTheTenantStoreToCreateANewServiceTenantWithManifest(string manifestName)
        {
            ServiceManifest manifest = this.manifestSteps.NamedManifest(manifestName);

            return this.CreateServiceTenantWithExceptionHandlingAsync(manifest);
        }

        [When("I use the tenant store to create a new service tenant without supplying a manifest")]
        public Task WhenIUseTheTenantStoreToCreateANewServiceTenantWithoutSupplyingAManifest()
        {
            return this.CreateServiceTenantWithExceptionHandlingAsync(null!);
        }

        public Task CreateServiceTenantWithExceptionHandlingAsync(ServiceManifest manifest)
        {
            return CatchException.AndStoreInScenarioContextAsync(
                this.scenarioContext,
                async () =>
                {
                    ITenant newTenant = await this.tenantStore.CreateServiceTenantAsync(manifest).ConfigureAwait(false);
                    this.scenarioContext.Set(newTenant.Id, manifest.ServiceName);
                });
        }

        [Then("the tenancy provider contains (.*) tenants as children of the root tenant")]
        public async Task ThenTheTenancyProviderContainsTenantsAsChildrenOfTheRootTenant(int expectedTenantCount)
        {
            TenantCollectionResult rootTenantChildren = await this.tenantStore.GetChildrenAsync(this.tenantStore.Root.Id).ConfigureAwait(false);
            Assert.AreEqual(expectedTenantCount, rootTenantChildren.Tenants.Count);
        }

        [Then("there is a tenant with Id '(.*)' as a child of the root tenant")]
        public async Task ThenThereIsATenantWithIdAsAChildOfTheRootTenant(string tenantId)
        {
            ITenantProvider tenantProvider = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantProvider>();

            // A TenantNotFound exception will be thrown if the tenant doesn't exist
            ITenant tenant = await tenantProvider.GetTenantAsync(tenantId).ConfigureAwait(false);

            Assert.AreEqual(tenantProvider.Root.Id, tenant.GetRequiredParentId());
        }

        [Then("there is no tenant with Id '(.*)' as a child of the root tenant")]
        public async Task ThenThereIsNoTenantWithIdAsAChildOfTheRootTenant(string tenantId)
        {
            ITenantProvider tenantProvider = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantProvider>();

            try
            {
                ITenant tenant = await tenantProvider.GetTenantAsync(tenantId).ConfigureAwait(false);

                // The tenant exists. If it's a child of the root tenant, the test is failed.
                Assert.AreNotEqual(
                    tenantProvider.Root.Id,
                    tenant.GetRequiredParentId(),
                    $"A tenant with Id '{tenantId}' does exist and is a child of the root tenant.");
            }
            catch (TenantNotFoundException)
            {
                // This means the test has passed.
            }
        }

        [Then("the tenant with Id '(.*)' is called '(.*)'")]
        public async Task ThenTheTenantWithIdIsCalled(string tenantId, string expectedTenantName)
        {
            ITenantProvider tenantProvider = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantProvider>();
            ITenant tenant = await tenantProvider.GetTenantAsync(tenantId).ConfigureAwait(false);
            Assert.AreEqual(expectedTenantName, tenant.Name);
        }

        [Then("the tenant with Id '(.*)' has a parent with Id '(.*)'")]
        public async Task ThenTheTenantWithIdHasAParentWithIdAsync(string tenantId, string expectedParentId)
        {
            ITenantProvider tenantProvider = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantProvider>();
            ITenant tenant = await tenantProvider.GetTenantAsync(tenantId).ConfigureAwait(false);
            Assert.AreEqual(expectedParentId, tenant.GetParentId());
        }

        [Then("there is a tenant called '(.*)' as a child of the root tenant")]
        public async Task ThenANewTenantCalledIsCreatedAsAChildOfTheRootTenant(string tenantName)
        {
            TenantCollectionResult rootTenantChildren = await this.tenantStore.GetChildrenAsync(this.tenantStore.Root.Id).ConfigureAwait(false);
            ITenant[] tenants = await Task.WhenAll(rootTenantChildren.Tenants.Select(x => this.tenantStore.GetTenantAsync(x))).ConfigureAwait(false);
            ITenant? matchingTenant = Array.Find(tenants, x => x.Name == tenantName);

            Assert.IsNotNull(matchingTenant, $"Could not find a child of the root tenant with the name '{tenantName}'");
        }

        [Then("there is no tenant called '(.*)' as a child of the root tenant")]
        public async Task ThenThereIsNoTenantCalledAsAChildOfTheRootTenant(string tenantName)
        {
            TenantCollectionResult rootTenantChildren = await this.tenantStore.GetChildrenAsync(this.tenantStore.Root.Id).ConfigureAwait(false);
            ITenant[] tenants = await Task.WhenAll(rootTenantChildren.Tenants.Select(x => this.tenantStore.GetTenantAsync(x))).ConfigureAwait(false);
            ITenant? matchingTenant = Array.Find(tenants, x => x.Name == tenantName);

            Assert.IsNull(matchingTenant, $"Could not find a child of the root tenant with the name '{tenantName}'");
        }

        [Then("there is a service tenant with Id '(.*)'")]
        public async Task ThenThereIsAServiceTenantWithId(string expectedServiceTenantId)
        {
            ITenantProvider tenantProvider = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantProvider>();
            ITenant tenant = await tenantProvider.GetTenantAsync(expectedServiceTenantId).ConfigureAwait(false);
            Assert.AreEqual(WellKnownTenantIds.ServiceTenantParentId, tenant.GetRequiredParentId());
        }

        [Then("there is a client tenant called '(.*)'")]
        public async Task ThenThereIsAClientTenantCalled(string clientTenantName)
        {
            await foreach (string clientTenantId in this.tenantStore.EnumerateAllChildrenAsync(WellKnownTenantIds.ClientTenantParentId))
            {
                ITenant tenant = await this.tenantStore.GetTenantAsync(clientTenantId).ConfigureAwait(false);
                if (tenant.Name == clientTenantName)
                {
                    return;
                }
            }

            Assert.Fail($"There is no client tenant called '{clientTenantName}'");
        }

        [Then("a new child tenant called '(.*)' of the service tenant called '(.*)' has been created")]
        public async Task ThenANewChildTenantCalledOfTheServiceTenantCalledHasBeenCreated(string childTenantName, string serviceTenantName)
        {
            ITenant? matchingChild =
                await this.GetChildTenantOfServiceTenant(childTenantName, serviceTenantName).ConfigureAwait(false);

            Assert.IsNotNull(matchingChild, $"The service tenant '{serviceTenantName}' does not contain a child tenant called '{childTenantName}'");
        }

        [Then("there should not be a child tenant called '(.*)' of the service tenant called '(.*)'")]
        [Then("no new child tenant called '(.*)' of the service tenant called '(.*)' has been created")]
        public async Task ThenNoNewChildTenantCalledOfTheServiceTenantCalledHasBeenCreated(string childTenantName, string serviceTenantName)
        {
            ITenant? matchingChild =
                await this.GetChildTenantOfServiceTenant(childTenantName, serviceTenantName).ConfigureAwait(false);

            Assert.IsNull(matchingChild, $"The service tenant '{serviceTenantName}' contains a child tenant called '{childTenantName}'");
        }

        [Then("the tenant called '([^']*)' should contain blob storage configuration under the key '([^']*)' for the account '([^']*)' and container name '([^']*)'")]
        public void ThenTheTenantCalledShouldContainBlobStorageConfigurationForABlobStorageContainerDefinitionWithContainerName(
            string tenantName,
            string configurationKey,
            string accountName,
            string containerName)
        {
            InMemoryTenantProvider tenantProvider =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<InMemoryTenantProvider>();

            ITenant tenant = tenantProvider.GetTenantByName(tenantName)
                ?? throw new TenantNotFoundException($"Could not find tenant with name '{tenantName}'");

            BlobContainerConfiguration tenantConfigItem = tenant.GetBlobContainerConfiguration(configurationKey);

            // GetBlobStorageConfiguration would have thrown an exception if the config didn't exist, but we'll do a
            // not null assertion anyway...
            Assert.IsNotNull(tenantConfigItem);
            Assert.AreEqual(accountName, tenantConfigItem.AccountName);
            Assert.AreEqual(containerName, tenantConfigItem.Container);
        }

        [Then("the tenant called '([^']*)' should contain legacy V2 blob storage configuration under the key '([^']*)' for the account '([^']*)' and container name '([^']*)'")]
        public void ThenTheTenantCalledShouldContainLegacyV2BlobStorageConfigurationForABlobStorageContainerDefinitionWithContainerName(
            string tenantName,
            string configurationKey,
            string accountName,
            string containerName)
        {
            InMemoryTenantProvider tenantProvider =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<InMemoryTenantProvider>();

            ITenant tenant = tenantProvider.GetTenantByName(tenantName)
                ?? throw new TenantNotFoundException($"Could not find tenant with name '{tenantName}'");

            Assert.IsTrue(
                tenant.Properties.TryGet(configurationKey, out LegacyV2BlobStorageConfiguration? tenantConfigItem),
                $"Tenant '{tenant.Name}' ({tenant.Id}) did not contain {nameof(LegacyV2BlobStorageConfiguration)} with key {configurationKey}");

            Assert.IsNotNull(tenantConfigItem);
            Assert.AreEqual(accountName, tenantConfigItem!.AccountName);
            Assert.AreEqual(containerName, tenantConfigItem.Container);
        }

        [Then("the tenant called '(.*)' should contain table storage configuration under the key '([^']*)' for the account '([^']*)' and table name '([^']*)'")]
        public void ThenTheTenantCalledShouldContainTableStorageConfigurationForATableStorageTableDefinitionWithTableName(
            string tenantName,
            string configurationKey,
            string accountName,
            string tableName)
        {
            InMemoryTenantProvider tenantProvider =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<InMemoryTenantProvider>();

            ITenant tenant = tenantProvider.GetTenantByName(tenantName)
                ?? throw new TenantNotFoundException($"Could not find tenant with name '{tenantName}'");

            TableConfiguration tenantConfigItem = tenant.GetTableStorageConfiguration(configurationKey);

            // GetTableStorageConfiguration would have thrown an exception if the config didn't exist, but we'll do a
            // not null assertion anyway...
            Assert.IsNotNull(tenantConfigItem);
            Assert.AreEqual(accountName, tenantConfigItem.AccountName);
            Assert.AreEqual(tableName, tenantConfigItem.TableName);
        }

        [Then("the tenant called '(.*)' should contain legacy V2 table storage configuration under the key '([^']*)' for the account '([^']*)' and table name '([^']*)'")]
        public void ThenTheTenantCalledShouldContainLegacyV2TableStorageConfigurationForATableStorageTableDefinitionWithTableName(
            string tenantName,
            string configurationKey,
            string accountName,
            string tableName)
        {
            InMemoryTenantProvider tenantProvider =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<InMemoryTenantProvider>();

            ITenant tenant = tenantProvider.GetTenantByName(tenantName)
                ?? throw new TenantNotFoundException($"Could not find tenant with name '{tenantName}'");

            Assert.IsTrue(
                tenant.Properties.TryGet(configurationKey, out LegacyV2TableConfiguration? tenantConfigItem),
                $"Tenant '{tenant.Name}' ({tenant.Id}) did not contain {nameof(LegacyV2TableConfiguration)} with key {configurationKey}");

            // GetTableStorageConfiguration would have thrown an exception if the config didn't exist, but we'll do a
            // not null assertion anyway...
            Assert.IsNotNull(tenantConfigItem);
            Assert.AreEqual(accountName, tenantConfigItem!.AccountName);
            Assert.AreEqual(tableName, tenantConfigItem.TableName);
        }

        [Then("the tenant called '([^']*)' should contain Cosmos configuration under the key '([^']*)' with database name '([^']*)' and container name '([^']*)'")]
        public void ThenTheTenantCalledShouldContainCosmosConfigurationUnderTheKeyWithDatabaseNameAndContainerName(
            string tenantName,
            string configurationKey,
            string databaseName,
            string containerName)
        {
            InMemoryTenantProvider tenantProvider =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<InMemoryTenantProvider>();

            ITenant tenant = tenantProvider.GetTenantByName(tenantName)
                ?? throw new TenantNotFoundException($"Could not find tenant with name '{tenantName}'");

            CosmosContainerConfiguration tenantConfigItem = tenant.GetCosmosConfiguration(configurationKey);

            // GetCosmosStorageConfiguration would have thrown an exception if the config didn't exist, but we'll do a
            // not null assertion anyway...
            Assert.IsNotNull(tenantConfigItem);
            Assert.AreEqual(databaseName, tenantConfigItem.Database);
            Assert.AreEqual(containerName, tenantConfigItem.Container);
        }

        [Then("the tenant called '([^']*)' should contain legacy V2 Cosmos configuration under the key '([^']*)' with database name '([^']*)' and container name '([^']*)'")]
        public void ThenTheTenantCalledShouldContainLegacyV2CosmosConfigurationUnderTheKeyWithDatabaseNameAndContainerName(
            string tenantName,
            string configurationKey,
            string databaseName,
            string containerName)
        {
            InMemoryTenantProvider tenantProvider =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<InMemoryTenantProvider>();

            ITenant tenant = tenantProvider.GetTenantByName(tenantName)
                ?? throw new TenantNotFoundException($"Could not find tenant with name '{tenantName}'");

            Assert.IsTrue(
                tenant.Properties.TryGet(configurationKey, out LegacyV2CosmosContainerConfiguration? tenantConfigItem),
                $"Tenant '{tenant.Name}' ({tenant.Id}) did not contain {nameof(LegacyV2CosmosContainerConfiguration)} with key {configurationKey}");

            Assert.IsNotNull(tenantConfigItem);
            Assert.AreEqual(databaseName, tenantConfigItem!.DatabaseName);
            Assert.AreEqual(containerName, tenantConfigItem.ContainerName);
        }

        [Then("the tenant called '([^']*)' should not contain blob storage configuration under key '(.*)'")]
        public void ThenTheTenantCalledShouldNotContainBlobStorageConfigurationForABlobStorageContainerDefinitionWithContainerName(
            string tenantName,
            string configurationKey)
        {
            InMemoryTenantProvider tenantProvider =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<InMemoryTenantProvider>();

            ITenant enrolledTenant = tenantProvider.GetTenantByName(tenantName)
                ?? throw new TenantNotFoundException($"Could not find tenant with name '{tenantName}'");

            try
            {
                // This should throw. If it doesn't, then the config exists and the test fails.
                enrolledTenant.GetBlobContainerConfiguration(configurationKey);
                Assert.Fail($"Did not expect to find blob storage configuration in tenant '{tenantName}' for container definition with container name '{configurationKey}', but it was present.");
            }
            catch (InvalidOperationException)
            {
                // This is what's expected - all is well.
            }
        }

        [Then("the tenant called '([^']*)' should not contain Cosmos configuration for a Cosmos container definition under the key '([^']*)'")]
        public void ThenTheTenantCalledShouldNotContainCosmosConfigurationForACosmosContainerDefinitionUnderTheKey(
            string tenantName,
            string configurationKey)
        {
            InMemoryTenantProvider tenantProvider =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<InMemoryTenantProvider>();

            ITenant enrolledTenant = tenantProvider.GetTenantByName(tenantName)
                ?? throw new TenantNotFoundException($"Could not find tenant with name '{tenantName}'");

            try
            {
                // This should throw. If it doesn't, then the config exists and the test fails.
                enrolledTenant.GetCosmosConfiguration(configurationKey);
                Assert.Fail($"Did not expect to find Cosmos configuration in tenant '{tenantName}' for container definition with key '{configurationKey}', but it was present.");
            }
            catch (InvalidOperationException)
            {
                // This is what's expected - all is well.
            }
        }

        [Then("the tenant called '([^']*)' should not contain table storage configuration under key '(.*)'")]
        public void ThenTheTenantCalledShouldNotContainTableStorageConfigurationForABlobStorageContainerDefinitionWithContainerName(
            string tenantName,
            string configurationKey)
        {
            InMemoryTenantProvider tenantProvider =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<InMemoryTenantProvider>();

            ITenant enrolledTenant = tenantProvider.GetTenantByName(tenantName)
                ?? throw new TenantNotFoundException($"Could not find tenant with name '{tenantName}'");

            try
            {
                // This should throw. If it doesn't, then the config exists and the test fails.
                enrolledTenant.GetTableStorageConfiguration(configurationKey);
                Assert.Fail($"Did not expect to find table storage configuration in tenant '{tenantName}' for container definition with container name '{configurationKey}', but it was present.");
            }
            catch (InvalidOperationException)
            {
                // This is what's expected - all is well.
            }
        }

        private async Task<ITenant?> GetChildTenantOfServiceTenant(string childTenantName, string serviceTenantName)
        {
            ITenant serviceTenant = await this.tenantStore.GetTenantAsync(this.scenarioContext.Get<string>(serviceTenantName)).ConfigureAwait(false);

            // Normally would have to care about pagination, but under test we only expect a small number of items.
            TenantCollectionResult getChildrenResult = await this.tenantStore.GetChildrenAsync(serviceTenant.Id).ConfigureAwait(false);
            ITenant[] children = await this.tenantStore.GetTenantsAsync(getChildrenResult.Tenants).ConfigureAwait(false);

            return Array.Find(children, x => x.Name == childTenantName);
        }

        private Task CreateTenant(Guid wellKnownGuid, string clientName, string? parentId = null)
        {
            return CatchException.AndStoreInScenarioContextAsync(
                this.scenarioContext,
                async () =>
                {
                    ITenant newTenant = await this.tenantStore.CreateClientTenantWithWellKnownGuidAsync(wellKnownGuid, clientName, parentId).ConfigureAwait(false);
                    this.scenarioContext.Set(newTenant.Id, clientName);
                });
        }
    }
}