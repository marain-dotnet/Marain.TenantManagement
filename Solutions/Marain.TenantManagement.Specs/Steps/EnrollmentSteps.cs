// <copyright file="EnrollmentSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Specs.Steps
{
    using System.Threading.Tasks;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
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
        public Task WhenIUseTheTenantEnrollmentServiceToEnrollTheClientTenantCalledInTheServiceCalled(
            string enrollingTenantName,
            string serviceTenantName)
        {
            ITenantManagementService managementService =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantManagementService>();

            ITenant enrollingTenant = this.scenarioContext.Get<ITenant>(enrollingTenantName);
            ITenant serviceTenant = this.scenarioContext.Get<ITenant>(serviceTenantName);

            return managementService.EnrollInServiceAsync(enrollingTenant, serviceTenant);
        }

        [Then("the tenant called '(.*)' should have the id of the tenant called '(.*)' added to its enrollments")]
        public void ThenTheTenantCalledShouldHaveTheIdOfTheTenantCalledAddedToItsEnrollments(
            string enrollingTenantName,
            string serviceTenantName)
        {
            ITenant enrollingTenant = this.scenarioContext.Get<ITenant>(enrollingTenantName);
            ITenant serviceTenant = this.scenarioContext.Get<ITenant>(serviceTenantName);

            Assert.IsTrue(enrollingTenant.IsEnrolledForService(serviceTenant.Id));
        }
    }
}
