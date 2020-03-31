// <copyright file="InitialisationSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Specs.Steps
{
    using System;
    using System.Threading.Tasks;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
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
            ITenantProvider tenantProvider = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantProvider>();

            for (int i = 0; i < tenantCount; i++)
            {
                await tenantProvider.CreateChildTenantAsync(tenantProvider.Root.Id, Guid.NewGuid().ToString()).ConfigureAwait(false);
            }
        }

        [When("I use the tenant management service to initialise the tenancy provider")]
        public async Task WhenIUseTheTenantManagementServiceToInitialiseTheTenancyProvider()
        {
            ITenantManagementService service = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantManagementService>();

            try
            {
                await service.InitialiseTenancyProviderAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.scenarioContext.Set(ex);
            }
        }

        [When("I use the tenant management service to initialise the tenancy provider using the force option")]
        public async Task WhenIUseTheTenantManagementServiceToInitialiseTheTenancyProviderUsingTheForceOption()
        {
            ITenantManagementService service = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantManagementService>();

            try
            {
                await service.InitialiseTenancyProviderAsync(true).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.scenarioContext.Set(ex);
            }
        }

        [Given("the tenancy provider has been initialised for use with Marain")]
        public Task GivenTheTenancyProviderHasAlreadyBeenInitialisedForUseWithMarain()
        {
            ITenantManagementService service = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantManagementService>();
            return service.InitialiseTenancyProviderAsync();
        }
    }
}
