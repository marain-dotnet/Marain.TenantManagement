// <copyright file="InitialisationSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Specs.Steps
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Corvus.Testing.SpecFlow;
    using Microsoft.Extensions.DependencyInjection;
    using TechTalk.SpecFlow;

    [Binding]
    public class InitialisationSteps
    {
        private readonly ScenarioContext scenarioContext;

        public InitialisationSteps(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }

        [Given("No tenants have been created")]
        public void GivenNoTenantsHaveBeenCreated()
        {
            // No-op
        }

        [Given("the tenancy provider contains (.*) tenants as children of the root tenant")]
        public async Task GivenTheTenancyProviderContainsTenantsAsChildrenOfTheRootTenant(int tenantCount)
        {
            ITenantStore tenantStore = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantStore>();

            for (int i = 0; i < tenantCount; i++)
            {
                await tenantStore.CreateChildTenantAsync(tenantStore.Root.Id, Guid.NewGuid().ToString()).ConfigureAwait(false);
            }
        }

        [When("I use the tenant store to initialise the tenancy provider")]
        public Task WhenIUseTheTenantStoreToInitialiseTheTenancyProvider()
        {
            ITenantStore service = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantStore>();

            return CatchException.AndStoreInScenarioContextAsync(
                this.scenarioContext,
                () => service.InitialiseTenancyProviderAsync());
        }

        [When("I use the tenant store to initialise the tenancy provider using the force option")]
        public Task WhenIUseTheTenantStoreToInitialiseTheTenancyProviderUsingTheForceOption()
        {
            ITenantStore service = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantStore>();

            return CatchException.AndStoreInScenarioContextAsync(
                this.scenarioContext,
                () => service.InitialiseTenancyProviderAsync(true));
        }

        [Given("the tenancy provider has been initialised for use with Marain")]
        public Task GivenTheTenancyProviderHasAlreadyBeenInitialisedForUseWithMarain()
        {
            ITenantStore service = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantStore>();
            return service.InitialiseTenancyProviderAsync();
        }
    }
}