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
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Corvus.Tenancy.Exceptions;
    using Marain.TenantManagement.EnrollmentConfiguration;
    using Marain.TenantManagement.Testing;
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

        [Given("I have used the tenant management service to enroll the tenant called '(.*)' in the service called '(.*)'")]
        [When("I use the tenant management service to enroll the tenant called '(.*)' in the service called '(.*)'")]
        public Task WhenIUseTheTenantManagementServiceToEnrollTheClientTenantCalledInTheServiceCalled(
            string enrollingTenantName,
            string serviceTenantName)
        {
            return this.EnrollTenantForService(enrollingTenantName, serviceTenantName);
        }

        [Given("I have used the tenant management service with the enrollment configuration called '(.*)' to enroll the tenant called '(.*)' in the service called '(.*)'")]
        [When("I use the tenant management service with the enrollment configuration called '(.*)' to enroll the tenant called '(.*)' in the service called '(.*)'")]
        public Task WhenIUseTheTenantManagementServiceToEnrollTheClientTenantCalledInTheServiceCalled(
            string enrollmentConfigurationName,
            string enrollingTenantName,
            string serviceTenantName)
        {
            return this.EnrollTenantForService(enrollingTenantName, serviceTenantName, enrollmentConfigurationName);
        }

        [When("I use the tenant management service to unenroll the tenant called '(.*)' from the service called '(.*)'")]
        public async Task WhenIUseTheTenantManagementServiceToUnenrollTheTenantCalledFromTheServiceCalled(
            string unenrollingTenantName,
            string serviceTenantName)
        {
            ITenantManagementService managementService =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantManagementService>();
            ITenantProvider tenantProvider = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantProvider>();

            ITenant unenrollingTenant = await tenantProvider.GetTenantAsync(this.scenarioContext.Get<string>(unenrollingTenantName)).ConfigureAwait(false);
            ITenant serviceTenant = await tenantProvider.GetTenantAsync(this.scenarioContext.Get<string>(serviceTenantName)).ConfigureAwait(false);

            await CatchException.AndStoreInScenarioContextAsync(
                this.scenarioContext,
                () => managementService.UnenrollFromServiceAsync(
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

        [Then("the tenant called '(.*)' should contain blob storage configuration for a blob storage container definition with container name '(.*)'")]
        public void ThenTheTenantCalledShouldContainBlobStorageConfigurationForABlobStorageContainerDefinitionWithContainerName(
            string enrolledTenantName,
            string containerName)
        {
            InMemoryTenantProvider tenantProvider =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<InMemoryTenantProvider>();

            ITenant enrolledTenant = tenantProvider.GetTenantByName(enrolledTenantName)
                ?? throw new TenantNotFoundException($"Could not find tenant with name '{enrolledTenantName}'");

            var containerDefinition = new BlobStorageContainerDefinition(containerName);

            BlobStorageConfiguration tenantConfigItem = enrolledTenant.GetBlobStorageConfiguration(containerDefinition);

            // GetBlobStorageConfiguration would have thrown an exception if the config didn't exist, but we'll do a
            // not null assertion anyway...
            Assert.IsNotNull(tenantConfigItem);
        }

        [Then("the tenant called '(.*)' should contain Cosmos configuration for a Cosmos container definition with database name '(.*)' and container name '(.*)'")]
        public void ThenTheTenantCalledShouldContainCosmosConfigurationForACosmosContainerDefinitionWithDatabaseNameAndContainerName(
            string enrolledTenantName,
            string databaseName,
            string containerName)
        {
            InMemoryTenantProvider tenantProvider =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<InMemoryTenantProvider>();

            ITenant enrolledTenant = tenantProvider.GetTenantByName(enrolledTenantName)
                ?? throw new TenantNotFoundException($"Could not find tenant with name '{enrolledTenantName}'");

            var containerDefinition = new CosmosContainerDefinition(databaseName, containerName, null);

            CosmosConfiguration tenantConfigItem = enrolledTenant.GetCosmosConfiguration(containerDefinition);

            // GetBlobStorageConfiguration would have thrown an exception if the config didn't exist, but we'll do a
            // not null assertion anyway...
            Assert.IsNotNull(tenantConfigItem);
        }

        [Then("the tenant called '(.*)' should not contain blob storage configuration for a blob storage container definition with container name '(.*)'")]
        public void ThenTheTenantCalledShouldNotContainBlobStorageConfigurationForABlobStorageContainerDefinitionWithContainerName(
            string enrolledTenantName,
            string containerName)
        {
            InMemoryTenantProvider tenantProvider =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<InMemoryTenantProvider>();

            ITenant enrolledTenant = tenantProvider.GetTenantByName(enrolledTenantName)
                ?? throw new TenantNotFoundException($"Could not find tenant with name '{enrolledTenantName}'");

            var containerDefinition = new BlobStorageContainerDefinition(containerName);

            try
            {
                // This should throw. If it doesn't, then the config exists and the test fails.
                enrolledTenant.GetBlobStorageConfiguration(containerDefinition);
                Assert.Fail($"Did not expect to find blob storage configuration in tenant '{enrolledTenantName}' for container definition with container name '{containerName}', but it was present.");
            }
            catch (ArgumentException)
            {
                // This is what's expected - all is well.
            }
        }

        [Then("the tenant called '(.*)' should not contain Cosmos configuration for a Cosmos container definition with database name '(.*)' and container name '(.*)'")]
        public void ThenTheTenantCalledShouldNotContainCosmosConfigurationForACosmosContainerDefinitionWithDatabaseNameAndContainerName(
            string enrolledTenantName,
            string databaseName,
            string containerName)
        {
            InMemoryTenantProvider tenantProvider =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<InMemoryTenantProvider>();

            ITenant enrolledTenant = tenantProvider.GetTenantByName(enrolledTenantName)
                ?? throw new TenantNotFoundException($"Could not find tenant with name '{enrolledTenantName}'");

            var containerDefinition = new CosmosContainerDefinition(databaseName, containerName, null);

            try
            {
                // This should throw. If it doesn't, then the config exists and the test fails.
                enrolledTenant.GetCosmosConfiguration(containerDefinition);
                Assert.Fail($"Did not expect to find Cosmos configuration in tenant '{enrolledTenantName}' for container definition with database name '{databaseName}' and container name '{containerName}', but it was present.");
            }
            catch (ArgumentException)
            {
                // This is what's expected - all is well.
            }
        }

        private async Task EnrollTenantForService(
            string enrollingTenantName,
            string serviceTenantName,
            string? enrollmentConfigurationName = null)
        {
            ITenantManagementService managementService =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantManagementService>();
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
