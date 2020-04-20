﻿// <copyright file="TenantSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Specs.Steps
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Corvus.Tenancy.Exceptions;
    using Marain.TenantManagement;
    using Marain.TenantManagement.ServiceManifests;
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

        [Given("I have used the tenant management service to create a new client tenant called '(.*)'")]
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
            ITenantProvider tenantProvider = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantProvider>();
            await foreach (string clientTenantId in tenantProvider.EnumerateAllChildrenAsync(WellKnownTenantIds.ClientTenantParentId))
            {
                ITenant tenant = await tenantProvider.GetTenantAsync(clientTenantId).ConfigureAwait(false);
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

        private async Task<ITenant?> GetChildTenantOfServiceTenant(string childTenantName, string serviceTenantName)
        {
            ITenantProvider tenantProvider = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantProvider>();

            ITenant serviceTenant = await tenantProvider.GetTenantAsync(this.scenarioContext.Get<string>(serviceTenantName)).ConfigureAwait(false);

            // Normally would have to care about pagination, but under test we only expect a small number of items.
            TenantCollectionResult getChildrenResult = await tenantProvider.GetChildrenAsync(serviceTenant.Id).ConfigureAwait(false);
            ITenant[] children = await tenantProvider.GetTenantsAsync(getChildrenResult.Tenants).ConfigureAwait(false);

            return Array.Find(children, x => x.Name == childTenantName);
        }
    }
}
