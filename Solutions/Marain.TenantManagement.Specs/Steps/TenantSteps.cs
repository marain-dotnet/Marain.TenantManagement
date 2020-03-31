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
        public async Task WhenIUseTheTenantManagementServiceToCreateANewClientTenantCalled(string clientName)
        {
            ITenantManagementService service = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantManagementService>();

            try
            {
                await service.CreateClientTenantAsync(clientName).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.scenarioContext.Set(ex);
            }
        }

        [Given("I have used the tenant management service to create a service tenant with manifest '(.*)'")]
        public Task GivenIHaveUsedTheTenantManagementServiceToCreateAServiceTenantWithManifest(string manifestName)
        {
            ServiceManifest manifest = this.scenarioContext.Get<ServiceManifest>(manifestName);

            ITenantManagementService service = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantManagementService>();

            return service.CreateServiceTenantAsync(manifest);
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

        public async Task CreateServiceTenantWithExceptionHandlingAsync(ServiceManifest manifest)
        {
            ITenantManagementService service = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantManagementService>();

            try
            {
                await service.CreateServiceTenantAsync(manifest).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.scenarioContext.Set(ex);
            }
        }

        [Then("the tenancy provider contains (.*) tenants as children of the root tenant")]
        public async Task ThenTheTenancyProviderContainsTenantsAsChildrenOfTheRootTenant(int expectedTenantCount)
        {
            ITenantProvider service = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantProvider>();
            TenantCollectionResult rootTenantChildren = await service.GetChildrenAsync(service.Root.Id).ConfigureAwait(false);
            Assert.AreEqual(expectedTenantCount, rootTenantChildren.Tenants.Count);
        }

        [Then("there is a tenant called '(.*)' as a child of the root tenant")]
        public async Task ThenANewTenantCalledIsCreatedAsAChildOfTheRootTenant(string tenantName)
        {
            ITenantProvider service = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantProvider>();
            TenantCollectionResult rootTenantChildren = await service.GetChildrenAsync(service.Root.Id).ConfigureAwait(false);
            ITenant[] tenants = await Task.WhenAll(rootTenantChildren.Tenants.Select(x => service.GetTenantAsync(x))).ConfigureAwait(false);
            ITenant? matchingTenant = Array.Find(tenants, x => x.Name == tenantName);

            Assert.IsNotNull(matchingTenant, $"Could not find a child of the root tenant with the name '{tenantName}'");
        }

        [Then("there is no tenant called '(.*)' as a child of the root tenant")]
        public async Task ThenThereIsNoTenantCalledAsAChildOfTheRootTenant(string tenantName)
        {
            ITenantProvider service = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantProvider>();
            TenantCollectionResult rootTenantChildren = await service.GetChildrenAsync(service.Root.Id).ConfigureAwait(false);
            ITenant[] tenants = await Task.WhenAll(rootTenantChildren.Tenants.Select(x => service.GetTenantAsync(x))).ConfigureAwait(false);
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

            List<ITenant> allChildren = tenantProvider.GetChildren(parent!);
            Assert.Contains(child, allChildren, $"The tenant called '{targetTenantName}' exists but is not a child of the tenant called '{parentTenantName}'");
        }
    }
}
