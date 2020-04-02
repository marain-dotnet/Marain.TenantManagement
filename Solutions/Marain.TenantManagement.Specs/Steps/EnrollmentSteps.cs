// <copyright file="EnrollmentSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Specs.Steps
{
    using System.Threading.Tasks;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Corvus.Tenancy.Exceptions;
    using Marain.TenantManagement.Specs.Mocks;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class EnrollmentSteps
    {
        private readonly ScenarioContext scenarioContext;

        public EnrollmentSteps(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }

        [When("I use the tenant enrollment service to enroll the tenant called '(.*)' in the service called '(.*)'")]
        public async Task WhenIUseTheTenantEnrollmentServiceToEnrollTheClientTenantCalledInTheServiceCalled(
            string enrollingTenantName,
            string serviceTenantName)
        {
            ITenantManagementService managementService =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantManagementService>();
            ITenantProvider tenantProvider = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantProvider>();

            ITenant enrollingTenant = await tenantProvider.GetTenantAsync(this.scenarioContext.Get<string>(enrollingTenantName)).ConfigureAwait(false);
            ITenant serviceTenant = await tenantProvider.GetTenantAsync(this.scenarioContext.Get<string>(serviceTenantName)).ConfigureAwait(false);

            await managementService.EnrollInServiceAsync(enrollingTenant, serviceTenant).ConfigureAwait(false);
        }

        [Then("the tenant called '(.*)' should have the id of the tenant called '(.*)' added to its enrollments")]
        public void ThenTheTenantCalledShouldHaveTheIdOfTheTenantCalledAddedToItsEnrollments(
            string enrollingTenantName,
            string serviceTenantName)
        {
            InMemoryTenantProvider tenantProvider =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<InMemoryTenantProvider>();

            ITenant enrollingTenant = tenantProvider.GetTenantByName(enrollingTenantName)
                ?? throw new TenantNotFoundException($"Could not find tenant with name '{enrollingTenantName}'");
            ITenant serviceTenant = tenantProvider.GetTenantByName(serviceTenantName)
                ?? throw new TenantNotFoundException($"Could not find tenant with name '{serviceTenantName}'");

            Assert.IsTrue(enrollingTenant.IsEnrolledForService(serviceTenant.Id));
        }

        [Then("the tenant called '(.*)' should have the id of the tenant called '(.*)' set as the delegated tenant for the service called '(.*)'")]
        public void ThenTheTenantCalledShouldHaveTheIdOfTheTenantCalledSetAsTheOn_Behalf_Of_TenantForTheServiceCalled(
            string enrolledTenantName,
            string onBehalfOfTenantName,
            string serviceTenantName)
        {
            InMemoryTenantProvider tenantProvider =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<InMemoryTenantProvider>();

            ITenant enrolledTenant = tenantProvider.GetTenantByName(enrolledTenantName)
                ?? throw new TenantNotFoundException($"Could not find tenant with name '{enrolledTenantName}'");

            ITenant onBehalfOfTenant = tenantProvider.GetTenantByName(onBehalfOfTenantName)
                ?? throw new TenantNotFoundException($"Could not find tenant with name '{onBehalfOfTenantName}'");

            ITenant serviceTenant = tenantProvider.GetTenantByName(serviceTenantName)
                ?? throw new TenantNotFoundException($"Could not find tenant with name '{serviceTenantName}'");

            Assert.AreEqual(onBehalfOfTenant.Id, enrolledTenant.GetDelegatedTenantIdForService(serviceTenant.Id));
        }
    }
}
