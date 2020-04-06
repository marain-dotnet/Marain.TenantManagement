// <copyright file="TenantSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Specs.Steps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Marain.TenantManagement;
    using Marain.TenantManagement.ServiceManifests;
    using Marain.TenantManagement.Specs.Mocks;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class TenantSteps
    {
        private readonly ScenarioContext scenarioContext;

        public TenantSteps(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }

        [When("I use the tenant management service to create a new client tenant called '(.*)'")]
        public Task WhenIUseTheTenantManagementServiceToCreateANewClientTenantCalled(string clientName)
        {
            ITenantManagementService service = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantManagementService>();

            return CatchException.AndStoreInScenarioContextAsync(
                this.scenarioContext,
                async () =>
                {
                    ITenant newTenant = await service.CreateClientTenantAsync(clientName).ConfigureAwait(false);
                    this.scenarioContext.Set(newTenant.Id, clientName);
                });
        }

        [Given(@"I have used the tenant management service to create a new client tenant called '(.*)'")]
        public async Task GivenIHaveUsedTheTenantManagementServiceToCreateANewClientTenantCalled(string clientName)
        {
            ITenantManagementService service = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantManagementService>();
            ITenant newTenant = await service.CreateClientTenantAsync(clientName).ConfigureAwait(false);
            this.scenarioContext.Set(newTenant.Id, clientName);
        }

        [Given("I have used the tenant management service to create a service tenant with manifest '(.*)'")]
        public async Task GivenIHaveUsedTheTenantManagementServiceToCreateAServiceTenantWithManifest(string manifestName)
        {
            ServiceManifest manifest = this.scenarioContext.Get<ServiceManifest>(manifestName);

            ITenantManagementService service = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantManagementService>();

            ITenant newTenant = await service.CreateServiceTenantAsync(manifest).ConfigureAwait(false);
            this.scenarioContext.Set(newTenant.Id, manifest.ServiceName);
        }

        [When("I use the tenant management service to create a new service tenant with manifest '(.*)'")]
        public Task WhenIUseTheTenantManagementServiceToCreateANewServiceTenantWithManifest(string manifestName)
        {
            ServiceManifest manifest = this.scenarioContext.Get<ServiceManifest>(manifestName);

            return this.CreateServiceTenantWithExceptionHandlingAsync(manifest);
        }

        [When("I use the tenant management service to create a new service tenant without supplying a manifest")]
        public Task WhenIUseTheTenantManagementServiceToCreateANewServiceTenantWithoutSupplyingAManifest()
        {
            return this.CreateServiceTenantWithExceptionHandlingAsync(null!);
        }

        public Task CreateServiceTenantWithExceptionHandlingAsync(ServiceManifest manifest)
        {
            ITenantManagementService service = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantManagementService>();

            return CatchException.AndStoreInScenarioContextAsync(
                this.scenarioContext,
                async () =>
                {
                    ITenant newTenant = await service.CreateServiceTenantAsync(manifest).ConfigureAwait(false);
                    this.scenarioContext.Set(newTenant.Id, manifest.ServiceName);
                });
        }

        [Then("the tenancy provider contains (.*) tenants as children of the root tenant")]
        public async Task ThenTheTenancyProviderContainsTenantsAsChildrenOfTheRootTenant(int expectedTenantCount)
        {
            ITenantProvider tenantProvider = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantProvider>();
            TenantCollectionResult rootTenantChildren = await tenantProvider.GetChildrenAsync(tenantProvider.Root.Id).ConfigureAwait(false);
            Assert.AreEqual(expectedTenantCount, rootTenantChildren.Tenants.Count);
        }

        [Then("there is a tenant called '(.*)' as a child of the root tenant")]
        public async Task ThenANewTenantCalledIsCreatedAsAChildOfTheRootTenant(string tenantName)
        {
            ITenantProvider tenantProvider = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantProvider>();
            TenantCollectionResult rootTenantChildren = await tenantProvider.GetChildrenAsync(tenantProvider.Root.Id).ConfigureAwait(false);
            ITenant[] tenants = await Task.WhenAll(rootTenantChildren.Tenants.Select(x => tenantProvider.GetTenantAsync(x))).ConfigureAwait(false);
            ITenant? matchingTenant = Array.Find(tenants, x => x.Name == tenantName);

            Assert.IsNotNull(matchingTenant, $"Could not find a child of the root tenant with the name '{tenantName}'");
        }

        [Then("there is no tenant called '(.*)' as a child of the root tenant")]
        public async Task ThenThereIsNoTenantCalledAsAChildOfTheRootTenant(string tenantName)
        {
            ITenantProvider tenantProvider = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantProvider>();
            TenantCollectionResult rootTenantChildren = await tenantProvider.GetChildrenAsync(tenantProvider.Root.Id).ConfigureAwait(false);
            ITenant[] tenants = await Task.WhenAll(rootTenantChildren.Tenants.Select(x => tenantProvider.GetTenantAsync(x))).ConfigureAwait(false);
            ITenant? matchingTenant = Array.Find(tenants, x => x.Name == tenantName);

            Assert.IsNull(matchingTenant, $"Could not find a child of the root tenant with the name '{tenantName}'");
        }

        [Then("there is a tenant called '(.*)' as a child of the tenant called '(.*)'")]
        public void ThenThereIsATenantCalledAsAChildOfTheTenantCalled(string targetTenantName, string parentTenantName)
        {
            InMemoryTenantProvider tenantProvider = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<InMemoryTenantProvider>();

            ITenant? parent = tenantProvider.GetTenantByName(parentTenantName);
            Assert.IsNotNull(parent, $"Could not find a tenant with the name '{parentTenantName}'");

            ITenant? child = tenantProvider.GetTenantByName(targetTenantName);
            Assert.IsNotNull(child, $"Could not find a tenant with the name '{targetTenantName}'");

            List<string> allChildren = tenantProvider.GetChildren(parent!.Id);
            Assert.Contains(child!.Id, allChildren, $"The tenant called '{targetTenantName}' exists but is not a child of the tenant called '{parentTenantName}'");
        }

        [Then("a new child tenant called '(.*)' of the service tenant called '(.*)' has been created")]
        public async Task ThenANewChildTenantCalledOfTheServiceTenantCalledHasBeenCreated(string childTenantName, string serviceTenantName)
        {
            ITenantProvider tenantProvider = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantProvider>();

            ITenant serviceTenant = await tenantProvider.GetTenantAsync(this.scenarioContext.Get<string>(serviceTenantName)).ConfigureAwait(false);

            // Normally would have to care about pagination, but under test we only expect a small number of items.
            TenantCollectionResult getChildrenResult = await tenantProvider.GetChildrenAsync(serviceTenant.Id).ConfigureAwait(false);
            ITenant[] children = await tenantProvider.GetTenantsAsync(getChildrenResult.Tenants).ConfigureAwait(false);

            ITenant? matchingChild = Array.Find(children, x => x.Name == childTenantName);

            Assert.IsNotNull(matchingChild, $"The service tenant '{serviceTenantName}' does not contain a child tenant called '{childTenantName}'");
        }
    }
}
