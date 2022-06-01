// <copyright file="EnrollmentSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Specs.Steps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Corvus.Storage.Azure.BlobStorage;
    using Corvus.Storage.Azure.Cosmos;
    using Corvus.Storage.Azure.TableStorage;
    using Corvus.Tenancy;
    using Corvus.Tenancy.Exceptions;
    using Corvus.Testing.SpecFlow;

    using Marain.TenantManagement.Configuration;
    using Marain.TenantManagement.EnrollmentConfiguration;
    using Marain.TenantManagement.Testing;

    using Microsoft.Extensions.DependencyInjection;

    using NUnit.Framework;

    using TechTalk.SpecFlow;

    [Binding]
    public class EnrollmentSteps
    {
        private readonly ScenarioContext scenarioContext;
        private readonly Dictionary<string, EnrollmentConfigurationEntryInputs> enrollmentConfigurationEntries = new();

        public EnrollmentSteps(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }

        [Given("I have enrollment configuration called '(.*)'")]
        public void GivenIHaveEnrollmentConfigurationCalled(string enrollmentConfigurationName)
        {
            this.enrollmentConfigurationEntries.Add(
                enrollmentConfigurationName,
                new EnrollmentConfigurationEntryInputs(
                    new Dictionary<string, ConfigurationItem>(),
                    new Dictionary<string, EnrollmentConfigurationEntryInputs>()));
        }

        [Given("the '([^']*)' enrollment has a dependency on the service tenant called '([^']*)' using configuration '([^']*)'")]
        public void GivenTheEnrollmentHasADependencyOnTheServiceTenantCalledUsingConfiguration(
            string parentEnrollmentConfigurationName,
            string dependencyServiceTenantName,
            string dependencyEnrollmentConfigurationName)
        {
            EnrollmentConfigurationEntryInputs parentEnrollmentConfiguration =
                this.enrollmentConfigurationEntries[parentEnrollmentConfigurationName];
            EnrollmentConfigurationEntryInputs dependencyEnrollmentConfiguration =
                this.enrollmentConfigurationEntries[dependencyEnrollmentConfigurationName];

            InMemoryTenantProvider tenantProvider =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<InMemoryTenantProvider>();
            ITenant serviceTenant = tenantProvider.GetTenantByName(dependencyServiceTenantName)
                ?? throw new TenantNotFoundException($"Could not find tenant with name '{dependencyServiceTenantName}'");

            parentEnrollmentConfiguration.Dependencies.Add(serviceTenant.Id, dependencyEnrollmentConfiguration);
        }

        [Given("the enrollment configuration called '(.*)' contains the following Blob Storage configuration items")]
        public void GivenTheEnrollmentConfigurationCalledContainsTheFollowingBlobStorageConfigurationItems(
            string enrollmentConfigurationName,
            Table configurationEntries)
        {
            EnrollmentConfigurationEntryInputs enrollmentConfigurationSet =
                this.enrollmentConfigurationEntries[enrollmentConfigurationName];

            foreach (TableRow? row in configurationEntries.Rows)
            {
                enrollmentConfigurationSet.ConfigurationItems.Add(
                    row["Key"],
                    new BlobStorageConfigurationItem
                    {
                        Configuration = new BlobContainerConfiguration
                        {
                            AccountName = row["Account Name"],
                            Container = row["Container"],
                        },
                    });
            }
        }

        [Given("the enrollment configuration called '(.*)' contains the following legacy V2 Blob Storage configuration items")]
        public void GivenTheEnrollmentConfigurationCalledContainsTheFollowingLegacyV2BlobStorageConfigurationItems(
            string enrollmentConfigurationName,
            Table configurationEntries)
        {
            EnrollmentConfigurationEntryInputs enrollmentConfigurationSet =
                this.enrollmentConfigurationEntries[enrollmentConfigurationName];

            foreach (TableRow? row in configurationEntries.Rows)
            {
                enrollmentConfigurationSet.ConfigurationItems.Add(
                    row["Key"],
                    new LegacyV2BlobStorageConfigurationItem
                    {
                        Configuration = new()
                        {
                            AccountName = row["Account Name"],
                            Container = row["Container"],
                        },
                    });
            }
        }

        [Given("the enrollment configuration called '(.*)' contains the following Table Storage configuration items")]
        public void GivenTheEnrollmentConfigurationCalledContainsTheFollowingTableStorageConfigurationItems(
            string enrollmentConfigurationName,
            Table configurationEntries)
        {
            EnrollmentConfigurationEntryInputs enrollmentConfigurationSet =
                this.enrollmentConfigurationEntries[enrollmentConfigurationName];

            foreach (TableRow? row in configurationEntries.Rows)
            {
                enrollmentConfigurationSet.ConfigurationItems.Add(
                    row["Key"],
                    new TableStorageConfigurationItem
                    {
                        Configuration = new TableConfiguration
                        {
                            AccountName = row["Account Name"],
                            TableName = row["Table"],
                        },
                    });
            }
        }

        [Given("the enrollment configuration called '(.*)' contains the following legacy V2 Table Storage configuration items")]
        public void GivenTheEnrollmentConfigurationCalledContainsTheFollowingLegacyV2TableStorageConfigurationItems(
            string enrollmentConfigurationName,
            Table configurationEntries)
        {
            EnrollmentConfigurationEntryInputs enrollmentConfigurationSet =
                this.enrollmentConfigurationEntries[enrollmentConfigurationName];

            foreach (TableRow? row in configurationEntries.Rows)
            {
                enrollmentConfigurationSet.ConfigurationItems.Add(
                    row["Key"],
                    new LegacyV2TableStorageConfigurationItem
                    {
                        Configuration = new()
                        {
                            AccountName = row["Account Name"],
                            TableName = row["Table"],
                        },
                    });
            }
        }

        [Given("the enrollment configuration called '(.*)' contains the following Cosmos configuration items")]
        public void GivenTheEnrollmentConfigurationCalledContainsTheFollowingCosmosConfigurationItems(
            string enrollmentConfigurationName,
            Table configurationEntries)
        {
            EnrollmentConfigurationEntryInputs enrollmentConfigurationSet =
                this.enrollmentConfigurationEntries[enrollmentConfigurationName];

            foreach (TableRow? row in configurationEntries.Rows)
            {
                enrollmentConfigurationSet.ConfigurationItems.Add(
                    row["Key"],
                    new CosmosConfigurationItem
                    {
                        Configuration = new CosmosContainerConfiguration
                        {
                            AccountUri = row["Account Uri"],
                            Database = row["Database Name"],
                            Container = row["Container Name"],
                        },
                    });
            }
        }

        [Given("the enrollment configuration called '(.*)' contains the following legacy V2 Cosmos configuration items")]
        public void GivenTheEnrollmentConfigurationCalledContainsTheFollowingLegacyV2CosmosConfigurationItems(
            string enrollmentConfigurationName,
            Table configurationEntries)
        {
            EnrollmentConfigurationEntryInputs enrollmentConfigurationSet =
                this.enrollmentConfigurationEntries[enrollmentConfigurationName];

            foreach (TableRow? row in configurationEntries.Rows)
            {
                enrollmentConfigurationSet.ConfigurationItems.Add(
                    row["Key"],
                    new LegacyV2CosmosConfigurationItem
                    {
                        Configuration = new()
                        {
                            AccountUri = row["Account Uri"],
                            DatabaseName = row["Database Name"],
                            ContainerName = row["Container Name"],
                        },
                    });
            }
        }

        [Given("I have used the tenant store to enroll the tenant called '([^']*)' in the service called '([^']*)'")]
        [When("I use the tenant store to enroll the tenant called '([^']*)' in the service called '([^']*)'")]
        public Task WhenIUseTheTenantStoreToEnrollTheClientTenantCalledInTheServiceCalled(
            string enrollingTenantName,
            string serviceTenantName)
        {
            return this.EnrollTenantForService(enrollingTenantName, serviceTenantName);
        }

        [When("I use the tenant store to enroll the tenant called '([^']*)' in the service called '([^']*)' anticipating an exception")]
        public Task WhenIUseTheTenantStoreToEnrollTheTenantCalledInTheServiceCalledAnticipatingAnException(
            string enrollingTenantName,
            string serviceTenantName)
        {
            return this.EnrollTenantForService(
                enrollingTenantName,
                serviceTenantName,
                catchExceptions: true);
        }

        [Given("I have used the tenant store with the enrollment configuration called '([^']*)' to enroll the tenant called '([^']*)' in the service called '([^']*)'")]
        [When("I use the tenant store with the enrollment configuration called '([^']*)' to enroll the tenant called '([^']*)' in the service called '([^']*)'")]
        public Task WhenIUseTheTenantStoreToEnrollTheClientTenantCalledInTheServiceCalled(
            string enrollmentConfigurationName,
            string enrollingTenantName,
            string serviceTenantName)
        {
            return this.EnrollTenantForService(enrollingTenantName, serviceTenantName, enrollmentConfigurationName);
        }

        [When("I use the tenant store with the enrollment configuration called '([^']*)' to enroll the tenant called '([^']*)' in the service called '([^']*)' anticipating an exception")]
        public Task WhenIUseTheTenantStoreWithTheEnrollmentConfigurationCalledToEnrollTheTenantCalledInTheServiceCalledAnticipatingAnException(
            string enrollmentConfigurationName,
            string enrollingTenantName,
            string serviceTenantName)
        {
            return this.EnrollTenantForService(
                enrollingTenantName,
                serviceTenantName,
                enrollmentConfigurationName,
                catchExceptions: true);
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

            Assert.IsTrue(
                enrollingTenant.IsEnrolledForService(serviceTenant.Id),
                $"Tenant {enrollingTenant.Id} ('{enrollingTenantName}') should be enrolled for {serviceTenant.Id} ('{serviceTenantName}')");
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

            Assert.IsFalse(
                enrollingTenant.IsEnrolledForService(serviceTenant.Id),
                $"Tenant {enrollingTenant.Id} ('{enrolledTenantName}') should not be enrolled for {serviceTenant.Id} ('{serviceTenantName}')");
        }

        [Then("the tenant called '(.*)' should have the id of the tenant called '(.*)' set as the delegated tenant for the service called '(.*)'")]
        public void ThenTheTenantCalledShouldHaveTheIdOfTheTenantCalledSetAsTheOn_Behalf_Of_TenantForTheServiceCalled(
            string consumingTenantName,
            string delegatedTenantName,
            string serviceTenantName)
        {
            InMemoryTenantProvider tenantProvider =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<InMemoryTenantProvider>();

            // The consuming tenant will either be a client tenant or a delegated tenant. In cases where
            // the dependencies only go one level deep, it will always be the client tenant. In cases where
            // a client is using a service with dependencies with dependencies, then it depends. If we have:
            //  Client -> Upper -> Middle -> Lower
            // then when the Upper Service uses Middle on Client's behalf, consumingTenantName here will be "Client".
            // But when the Middle service uses Lower on behalf of Upper (with Upper in turn acting on behalf of
            // Client), then the consumingTenantName will actually be "Upper\Client", which is the delegated tenant
            // that Upper uses when acting on behalf of Client when it talks to dependent services such as Lower.
            ITenant enrolledTenant = tenantProvider.GetTenantByName(consumingTenantName)
                ?? throw new TenantNotFoundException($"Could not find tenant with name '{consumingTenantName}'");

            ITenant onBehalfOfTenant = tenantProvider.GetTenantByName(delegatedTenantName)
                ?? throw new TenantNotFoundException($"Could not find tenant with name '{delegatedTenantName}'");

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
            string? enrollmentConfigurationName = null,
            bool catchExceptions = false)
        {
            ITenantStore managementService =
                ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantStore>();
            ITenantProvider tenantProvider = ContainerBindings.GetServiceProvider(this.scenarioContext).GetRequiredService<ITenantProvider>();

            ITenant enrollingTenant = await tenantProvider.GetTenantAsync(this.scenarioContext.Get<string>(enrollingTenantName)).ConfigureAwait(false);
            ITenant serviceTenant = await tenantProvider.GetTenantAsync(this.scenarioContext.Get<string>(serviceTenantName)).ConfigureAwait(false);

            EnrollmentConfigurationEntry MakeEnrollmentConfiguration(EnrollmentConfigurationEntryInputs inputs)
            {
                return new EnrollmentConfigurationEntry(
                    inputs.ConfigurationItems,
                    inputs.Dependencies.ToDictionary(
                        d => d.Key,
                        d => MakeEnrollmentConfiguration(d.Value)));
            }

            EnrollmentConfigurationEntry enrollmentConfigurationEntry =
                string.IsNullOrEmpty(enrollmentConfigurationName)
                ? new EnrollmentConfigurationEntry(new Dictionary<string, ConfigurationItem>(), new Dictionary<string, EnrollmentConfigurationEntry>())
                : MakeEnrollmentConfiguration(this.enrollmentConfigurationEntries[enrollmentConfigurationName]);

            Dictionary<string, EnrollmentConfigurationEntry> enrollmentConfiguration = new()
            {
                [serviceTenant.Id] = enrollmentConfigurationEntry,
            };

            Task Enroll() => managementService.EnrollInServiceAsync(
                    enrollingTenant,
                    serviceTenant,
                    enrollmentConfiguration);

            if (catchExceptions)
            {
                await CatchException.AndStoreInScenarioContextAsync(
                    this.scenarioContext,
                    Enroll).ConfigureAwait(false);
            }
            else
            {
                await Enroll();
            }
        }

        private record EnrollmentConfigurationEntryInputs(
            Dictionary<string, ConfigurationItem> ConfigurationItems,
            Dictionary<string, EnrollmentConfigurationEntryInputs> Dependencies);
    }
}