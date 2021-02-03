// <copyright file="EnrollmentSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Specs.Steps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.Azure.Storage.Tenancy;
    using Corvus.Tenancy;
    using Corvus.Tenancy.Exceptions;
    using Marain.TenantManagement.EnrollmentConfiguration;
    using Marain.TenantManagement.Testing;
    using Corvus.Testing.SpecFlow;
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

        [Given("I have enrollment configuration called '(.*)'")]
        public void GivenIHaveEnrollmentConfigurationCalled(string enrollmentConfigurationName)
        {
            this.scenarioContext.Set(new List<EnrollmentConfigurationItem>(), enrollmentConfigurationName);
        }

        [Given("the enrollment configuration called '(.*)' contains the following Blob Storage configuration items")]
        public void GivenTheEnrollmentConfigurationCalledContainsTheFollowingBlobStorageConfigurationItems(
            string enrollmentConfigurationName,
            Table configurationEntries)
        {
            List<EnrollmentConfigurationItem> enrollmentConfiguration =
                this.scenarioContext.Get<List<EnrollmentConfigurationItem>>(enrollmentConfigurationName);

            enrollmentConfiguration.AddRange(
                configurationEntries.Rows.Select(
                    row => new EnrollmentBlobStorageConfigurationItem
                    {
                        Key = row["Key"],
                        Configuration = new BlobStorageConfiguration
                        {
                            AccountName = row["Account Name"],
                            Container = row["Container"],
                        },
                    }));
        }

        [Given("the enrollment configuration called '(.*)' contains the following Table Storage configuration items")]
        public void GivenTheEnrollmentConfigurationCalledContainsTheFollowingTableStorageConfigurationItems(
            string enrollmentConfigurationName,
            Table configurationEntries)
        {
            List<EnrollmentConfigurationItem> enrollmentConfiguration =
                this.scenarioContext.Get<List<EnrollmentConfigurationItem>>(enrollmentConfigurationName);

            enrollmentConfiguration.AddRange(
                configurationEntries.Rows.Select(
                    row => new EnrollmentTableStorageConfigurationItem
                    {
                        Key = row["Key"],
                        Configuration = new TableStorageConfiguration
                        {
                            AccountName = row["Account Name"],
                            TableName = row["Table"],
                        },
                    }));
        }

        [Given("the enrollment configuration called '(.*)' contains the following Cosmos configuration items")]
        public void GivenTheEnrollmentConfigurationCalledContainsTheFollowingCosmosConfigurationItems(
            string enrollmentConfigurationName,
            Table configurationEntries)
        {
            List<EnrollmentConfigurationItem> enrollmentConfiguration =
                this.scenarioContext.Get<List<EnrollmentConfigurationItem>>(enrollmentConfigurationName);

            enrollmentConfiguration.AddRange(
                configurationEntries.Rows.Select(
                    row => new EnrollmentCosmosConfigurationItem
                    {
                        Key = row["Key"],
                        Configuration = new CosmosConfiguration
                        {
                            AccountUri = row["Account Uri"],
                            DatabaseName = row["Database Name"],
                            ContainerName = row["Container Name"],
                        },
                    }));
        }

        [Given("I have used the tenant store to enroll the tenant called '(.*)' in the service called '(.*)'")]
        [When("I use the tenant store to enroll the tenant called '(.*)' in the service called '(.*)'")]
        public Task WhenIUseTheTenantStoreToEnrollTheClientTenantCalledInTheServiceCalled(
            string enrollingTenantName,
            string serviceTenantName)
        {
            return this.EnrollTenantForService(enrollingTenantName, serviceTenantName);
        }

        [Given("I have used the tenant store with the enrollment configuration called '(.*)' to enroll the tenant called '(.*)' in the service called '(.*)'")]
        [When("I use the tenant store with the enrollment configuration called '(.*)' to enroll the tenant called '(.*)' in the service called '(.*)'")]
        public Task WhenIUseTheTenantStoreToEnrollTheClientTenantCalledInTheServiceCalled(
            string enrollmentConfigurationName,
            string enrollingTenantName,
            string serviceTenantName)
        {
            return this.EnrollTenantForService(enrollingTenantName, serviceTenantName, enrollmentConfigurationName);
        }

        [When("I use the tenant store to unenroll the tenant called '(.*)' from the service called '(.*)'")]
        public async Task WhenIUseTheTenantStoreToUnenrollTheTenantCalledFromTheServiceCalled(
            string unenrollingTenantName,
            string serviceTenantName)
        {
            ITenantStore tenantStore =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantStore>();
            ITenantProvider tenantProvider = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantProvider>();

            ITenant unenrollingTenant = await tenantProvider.GetTenantAsync(this.scenarioContext.Get<string>(unenrollingTenantName)).ConfigureAwait(false);
            ITenant serviceTenant = await tenantProvider.GetTenantAsync(this.scenarioContext.Get<string>(serviceTenantName)).ConfigureAwait(false);

            await CatchException.AndStoreInScenarioContextAsync(
                this.scenarioContext,
                () => tenantStore.UnenrollFromServiceAsync(
                    unenrollingTenant.Id,
                    serviceTenant.Id)).ConfigureAwait(false);
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

        [Then("the tenant called '(.*)' should not have the id of the tenant called '(.*)' in its enrollments")]
        public void ThenTheTenantCalledShouldNotHaveTheIdOfTheTenantCalledInItsEnrollments(
            string enrolledTenantName,
            string serviceTenantName)
        {
            InMemoryTenantProvider tenantProvider =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<InMemoryTenantProvider>();

            ITenant enrollingTenant = tenantProvider.GetTenantByName(enrolledTenantName)
                ?? throw new TenantNotFoundException($"Could not find tenant with name '{enrolledTenantName}'");
            ITenant serviceTenant = tenantProvider.GetTenantByName(serviceTenantName)
                ?? throw new TenantNotFoundException($"Could not find tenant with name '{serviceTenantName}'");

            Assert.IsFalse(enrollingTenant.IsEnrolledForService(serviceTenant.Id));
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

            Assert.AreEqual(onBehalfOfTenant.Id, enrolledTenant.GetDelegatedTenantIdForServiceId(serviceTenant.Id));
        }

        [Then("the tenant called '(.*)' should not have a delegated tenant for the service called '(.*)'")]
        public void ThenTheTenantCalledShouldNotHaveADelegatedTenantForTheServiceCalled(
            string enrolledTenantName,
            string serviceTenantName)
        {
            InMemoryTenantProvider tenantProvider =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<InMemoryTenantProvider>();

            ITenant enrolledTenant = tenantProvider.GetTenantByName(enrolledTenantName)
                ?? throw new TenantNotFoundException($"Could not find tenant with name '{enrolledTenantName}'");

            ITenant serviceTenant = tenantProvider.GetTenantByName(serviceTenantName)
                ?? throw new TenantNotFoundException($"Could not find tenant with name '{serviceTenantName}'");

            try
            {
                enrolledTenant.GetDelegatedTenantIdForServiceId(serviceTenant.Id);
                Assert.Fail($"Did not expect tenant with name '{enrolledTenant}' to have a delegated tenant for service '{serviceTenantName}', but one exists.");
            }
            catch (ArgumentException)
            {
            }
        }

        private async Task EnrollTenantForService(
            string enrollingTenantName,
            string serviceTenantName,
            string? enrollmentConfigurationName = null)
        {
            ITenantStore managementService =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantStore>();
            ITenantProvider tenantProvider = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantProvider>();

            ITenant enrollingTenant = await tenantProvider.GetTenantAsync(this.scenarioContext.Get<string>(enrollingTenantName)).ConfigureAwait(false);
            ITenant serviceTenant = await tenantProvider.GetTenantAsync(this.scenarioContext.Get<string>(serviceTenantName)).ConfigureAwait(false);

            List<EnrollmentConfigurationItem> enrollmentConfiguration =
                string.IsNullOrEmpty(enrollmentConfigurationName)
                ? new List<EnrollmentConfigurationItem>()
                : this.scenarioContext.Get<List<EnrollmentConfigurationItem>>(enrollmentConfigurationName);

            await CatchException.AndStoreInScenarioContextAsync(
                this.scenarioContext,
                () => managementService.EnrollInServiceAsync(
                    enrollingTenant,
                    serviceTenant,
                    enrollmentConfiguration.ToArray())).ConfigureAwait(false);
        }
    }
}
