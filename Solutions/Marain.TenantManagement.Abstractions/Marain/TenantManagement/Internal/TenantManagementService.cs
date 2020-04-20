﻿// <copyright file="TenantManagementService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Corvus.Tenancy.Exceptions;
    using Marain.TenantManagement.EnrollmentConfiguration;
    using Marain.TenantManagement.ServiceManifests;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Implementation of <see cref="ITenantManagementService"/> over the Corvus <see cref="ITenantProvider"/>.
    /// </summary>
    public class TenantManagementService : ITenantManagementService
    {
        private readonly ITenantProvider tenantProvider;
        private readonly ILogger<TenantManagementService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantManagementService"/> class.
        /// </summary>
        /// <param name="tenantProvider">The underlying tenant provider used to access tenants.</param>
        /// <param name="logger">The logger.</param>
        public TenantManagementService(
            ITenantProvider tenantProvider,
            ILogger<TenantManagementService> logger)
        {
            this.tenantProvider = tenantProvider;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<ITenant> CreateClientTenantAsync(string clientName)
        {
            if (string.IsNullOrWhiteSpace(clientName))
            {
                throw new ArgumentException(nameof(clientName));
            }

            ITenant parent = await this.GetClientTenantParentAsync().ConfigureAwait(false);

            this.logger.LogDebug("Creating new client tenant '{clientName}'", clientName);
            ITenant newTenant = await this.tenantProvider.CreateChildTenantAsync(parent.Id, clientName).ConfigureAwait(false);

            this.logger.LogDebug(
                "New client tenant '{clientName}' created with Id '{tenantId}'; updating tenant type.",
                newTenant.Name,
                newTenant.Id);

            newTenant.SetMarainTenantType(MarainTenantType.Client);
            await this.tenantProvider.UpdateTenantAsync(newTenant).ConfigureAwait(false);

            this.logger.LogInformation(
                "Created new client tenant '{clientName}' with Id '{tenantId}'.",
                newTenant.Name,
                newTenant.Id);
            return newTenant;
        }

        /// <inheritdoc/>
        public async Task<ITenant> CreateServiceTenantAsync(ServiceManifest manifest)
        {
            if (manifest == null)
            {
                throw new ArgumentNullException(nameof(manifest));
            }

            this.logger.LogDebug("Validating service manifest for service creation.");

            await manifest.ValidateAndThrowAsync(this).ConfigureAwait(false);

            ITenant parent = await this.GetServiceTenantParentAsync().ConfigureAwait(false);

            this.logger.LogDebug(
                "Creating new service tenant '{serviceName}' with well-known GUID '{wellKnownGuid}'",
                manifest.ServiceName,
                manifest.WellKnownTenantGuid);

            ITenant newTenant = await this.tenantProvider.CreateWellKnownChildTenantAsync(
                parent.Id,
                manifest.WellKnownTenantGuid,
                manifest.ServiceName).ConfigureAwait(false);

            this.logger.LogDebug(
                "New service tenant '{serviceName}' created with Id '{tenantId}'; updating tenant type and manifest.",
                newTenant.Name,
                newTenant.Id);

            newTenant.SetServiceManifest(manifest);
            newTenant.SetMarainTenantType(MarainTenantType.Service);

            await this.tenantProvider.UpdateTenantAsync(newTenant).ConfigureAwait(false);

            this.logger.LogInformation(
                "Created new service tenant '{serviceName}' with Id '{tenantId}'.",
                newTenant.Name,
                newTenant.Id);
            return newTenant;
        }

        /// <inheritdoc/>
        public Task<ITenant> GetServiceTenantAsync(string serviceTenantId)
            => this.GetTenantOfTypeAsync(serviceTenantId, MarainTenantType.Service);

        /// <inheritdoc/>
        public Task<ITenant> GetClientTenantAsync(string clientTenantId)
            => this.GetTenantOfTypeAsync(clientTenantId, MarainTenantType.Client);

        /// <inheritdoc/>
        public Task<ITenant> GetDelegatedTenantAsync(string delegatedTenantId)
            => this.GetTenantOfTypeAsync(delegatedTenantId, MarainTenantType.Delegated);

        /// <inheritdoc/>
        public async Task<ITenant> GetDelegatedTenantAsync(string clientTenantId, string serviceTenantId)
        {
            // Whilst we're saying "client tenant", the client here could itself be a delegated tenant:
            ITenant client = await this.GetTenantOfTypeAsync(clientTenantId, MarainTenantType.Client, MarainTenantType.Delegated).ConfigureAwait(false);

            // This method will throw if there is no delegated tenant.
            string delegatedTenantId = client.GetDelegatedTenantIdForServiceId(serviceTenantId);
            return await this.GetDelegatedTenantAsync(delegatedTenantId).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task InitialiseTenancyProviderAsync(bool force = false)
        {
            ITenant? existingClientTenantParent = null;
            try
            {
                existingClientTenantParent = await this.GetClientTenantParentAsync().ConfigureAwait(false);
            }
            catch (InvalidOperationException)
            {
                // No-op - this is expected if the provider isn't yet initialised.
            }

            ITenant? existingServiceTenantParent = null;

            try
            {
                existingServiceTenantParent = await this.GetServiceTenantParentAsync().ConfigureAwait(false);
            }
            catch (InvalidOperationException)
            {
                // No-op - this is expected if the provider isn't yet initialised.
            }

            if (existingClientTenantParent != null && existingServiceTenantParent != null)
            {
                // All done - both parent tenants exist.
                return;
            }

            // At least one of the parent tenants don't exist. If there are other tenants, we're being asked to initialise
            // into a non-empty tenant provider, which we shouldn't do by default. To check this, we only need to retrieve
            // 2 tenants.
            TenantCollectionResult existingTopLevelTenantIds = await this.tenantProvider.GetChildrenAsync(
                this.tenantProvider.Root.Id,
                2,
                null).ConfigureAwait(false);

            if (existingTopLevelTenantIds.Tenants.Count > 1 && !force)
            {
                throw new InvalidOperationException("Cannot initialise the tenancy provider for use with Marain because it already contains non-Marain tenants at the root level. If you wish to initialise anyway, re-invoke the method with the 'force' parameter set to true.");
            }

            // Create the tenants
            if (existingClientTenantParent == null)
            {
                await this.tenantProvider.CreateWellKnownChildTenantAsync(
                    this.tenantProvider.Root.Id,
                    WellKnownTenantIds.ClientTenantParentGuid,
                    TenantNames.ClientTenantParent).ConfigureAwait(false);
            }

            if (existingServiceTenantParent == null)
            {
                await this.tenantProvider.CreateWellKnownChildTenantAsync(
                    this.tenantProvider.Root.Id,
                    WellKnownTenantIds.ServiceTenantParentGuid,
                    TenantNames.ServiceTenantParent).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public async Task EnrollInServiceAsync(
            string enrollingTenantId,
            string serviceTenantId,
            EnrollmentConfigurationItem[] configurationItems)
        {
            if (string.IsNullOrWhiteSpace(enrollingTenantId))
            {
                throw new ArgumentException(nameof(enrollingTenantId));
            }

            if (string.IsNullOrWhiteSpace(serviceTenantId))
            {
                throw new ArgumentException(nameof(serviceTenantId));
            }

            if (configurationItems == null)
            {
                throw new ArgumentNullException(nameof(configurationItems));
            }

            // Enrolling tenant can be either a Client tenant or a Delegated tenant.
            ITenant enrollingTenant = await this.GetTenantOfTypeAsync(
                enrollingTenantId,
                MarainTenantType.Client,
                MarainTenantType.Delegated).ConfigureAwait(false);

            ITenant serviceTenant = await this.GetServiceTenantAsync(serviceTenantId).ConfigureAwait(false);

            await this.EnrollInServiceAsync(enrollingTenant, serviceTenant, configurationItems).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task EnrollInServiceAsync(
            ITenant enrollingTenant,
            string serviceTenantId,
            EnrollmentConfigurationItem[] configurationItems)
        {
            // Enrolling tenant validation will happen when we call through to the next method to do the enrollment, so no
            // need to do it here as well.
            if (string.IsNullOrWhiteSpace(serviceTenantId))
            {
                throw new ArgumentException(nameof(serviceTenantId));
            }

            if (configurationItems == null)
            {
                throw new ArgumentNullException(nameof(configurationItems));
            }

            ITenant serviceTenant = await this.GetServiceTenantAsync(serviceTenantId).ConfigureAwait(false);

            await this.EnrollInServiceAsync(enrollingTenant, serviceTenant, configurationItems).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task EnrollInServiceAsync(
            ITenant enrollingTenant,
            ITenant serviceTenant,
            EnrollmentConfigurationItem[] configurationItems)
        {
            if (enrollingTenant == null)
            {
                throw new ArgumentNullException(nameof(enrollingTenant));
            }

            enrollingTenant.EnsureTenantIsOfType(MarainTenantType.Client, MarainTenantType.Delegated);

            if (serviceTenant == null)
            {
                throw new ArgumentNullException(nameof(serviceTenant));
            }

            serviceTenant.EnsureTenantIsOfType(MarainTenantType.Service);

            if (configurationItems == null)
            {
                throw new ArgumentNullException(nameof(configurationItems));
            }

            this.logger.LogDebug(
                "Enrolling tenant '{enrollingTenantName}' with Id '{enrollingTenantId}' from service '{serviceTenantName}' with Id '{serviceTenantId}'",
                enrollingTenant.Name,
                enrollingTenant.Id,
                serviceTenant.Name,
                serviceTenant.Id);

            // First we need to ensure that all the required config items for both the service being enrolled in,
            // as well as any dependent services is provided.
            ServiceManifestRequiredConfigurationEntry[] requiredConfig = await this.GetServiceEnrollmentConfigurationRequirementsAsync(serviceTenant).ConfigureAwait(false);

            this.logger.LogDebug("Validating supplied configuration against required config.");
            configurationItems.ValidateAndThrow(requiredConfig);

            ServiceManifest manifest = serviceTenant.GetServiceManifest();

            // Now, match up the required config items for this service to the relevent supplied config (we may
            // have been supplied with config for dependent services as well, so we can't just attach all
            // of the supplied config items to the enrolling tenant - some of it could belong on delegated
            // tenants.
            this.logger.LogDebug(
                "Attaching required configuration items to tenant '{serviceTenantName}' with Id '{serviceTenantId}'",
                serviceTenant.Name,
                serviceTenant.Id);

            IEnumerable<(ServiceManifestRequiredConfigurationEntry RequiredConfigurationEntry, EnrollmentConfigurationItem ProvidedConfigurationItem)> matchedConfigItems =
                manifest.RequiredConfigurationEntries.Select(
                    requiredConfigItem =>
                    (requiredConfigItem, configurationItems.Single(item => item.Key == requiredConfigItem.Key)));

            foreach ((ServiceManifestRequiredConfigurationEntry RequiredConfigurationEntry, EnrollmentConfigurationItem ProvidedConfigurationItem) current in matchedConfigItems)
            {
                this.logger.LogDebug(
                    "Adding configuration entry '{requiredConfigurationEntryKey}' to tenant '{serviceTenantName}' with Id '{serviceTenantId}'",
                    current.RequiredConfigurationEntry.Key,
                    serviceTenant.Name,
                    serviceTenant.Id);

                current.RequiredConfigurationEntry.AddToTenant(enrollingTenant, current.ProvidedConfigurationItem);
            }

            // Add an enrollment entry to the tenant.
            this.logger.LogDebug(
                "Adding service enrollment to tenant '{serviceTenantName}' with Id '{serviceTenantId}'",
                serviceTenant.Name,
                serviceTenant.Id);

            enrollingTenant.AddServiceEnrollment(serviceTenant.Id);

            // If this service has dependencies, we need to create a new delegated tenant for the service to use when
            // accessing those dependencies.
            if (manifest.DependsOnServiceTenants.Count > 0)
            {
                this.logger.LogDebug(
                    "Service '{serviceTenantName}' has dependencies. Creating delegated tenant for enrollment.",
                    serviceTenant.Name);

                ITenant delegatedTenant = await this.CreateDelegatedTenant(enrollingTenant, serviceTenant).ConfigureAwait(false);

                // Now enroll the new delegated tenant for all of the dependent services.
                await Task.WhenAll(manifest.DependsOnServiceTenants.Select(
                    dependsOnService => this.EnrollInServiceAsync(
                        delegatedTenant,
                        dependsOnService.Id,
                        configurationItems))).ConfigureAwait(false);

                // Add the delegated tenant Id to the enrolling tenant
                this.logger.LogDebug(
                    "Setting delegated tenant for client '{enrollingTenantName}' with Id '{enrollingTenantId}' in service '{serviceTenantName}' with Id '{serviceTenantId}' to new tenant '{delegatedTenantName}' with Id '{delegatedTenantId}'.",
                    enrollingTenant.Name,
                    enrollingTenant.Id,
                    serviceTenant.Name,
                    serviceTenant.Id,
                    delegatedTenant.Name,
                    delegatedTenant.Id);

                enrollingTenant.SetDelegatedTenantForService(serviceTenant, delegatedTenant);
            }

            this.logger.LogDebug(
                "Updating tenant '{enrollingTenantName}' with Id '{enrollingTenantId}'",
                enrollingTenant.Name,
                enrollingTenant.Id);

            await this.tenantProvider.UpdateTenantAsync(enrollingTenant).ConfigureAwait(false);

            this.logger.LogInformation(
                "Successfully enrolled tenant '{enrollingTenantName}' with Id '{enrollingTenant.Id}' for service '{serviceTenantName}' with Id '{serviceTenantId}'",
                enrollingTenant.Name,
                enrollingTenant.Id,
                serviceTenant.Name,
                serviceTenant.Id);
        }

        /// <inheritdoc/>
        public async Task UnenrollFromServiceAsync(
            string enrolledTenantId,
            string serviceTenantId)
        {
            if (string.IsNullOrWhiteSpace(enrolledTenantId))
            {
                throw new ArgumentException(nameof(enrolledTenantId));
            }

            ITenant enrolledTenant = await this.GetTenantOfTypeAsync(
                enrolledTenantId,
                MarainTenantType.Client,
                MarainTenantType.Delegated).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(serviceTenantId))
            {
                throw new ArgumentException(nameof(serviceTenantId));
            }

            if (!enrolledTenant.IsEnrolledForService(serviceTenantId))
            {
                throw new InvalidOperationException(
                    $"Cannot unenroll tenant '{enrolledTenant.Name}' with Id '{enrolledTenant.Id}' from service with Id '{serviceTenantId}' because it is not currently enrolled");
            }

            ITenant serviceTenant = await this.GetServiceTenantAsync(serviceTenantId).ConfigureAwait(false);

            this.logger.LogDebug(
                "Unenrolling tenant '{enrolledTenantName}' with Id '{enrolledTenantId}' from service '{serviceTenantName}' with Id '{serviceTenantId}'",
                enrolledTenant.Name,
                enrolledTenant.Id,
                serviceTenant.Name,
                serviceTenant.Id);

            ServiceManifest manifest = serviceTenant.GetServiceManifest();

            // If there are dependencies, we first need to unenroll from each of those and then remove the delegated tenant.
            if (manifest.DependsOnServiceTenants.Count > 0)
            {
                this.logger.LogDebug(
                    "Service '{serviceTenantName}' has dependencies. Retrieving delegated tenant for unenrollment.",
                    serviceTenant.Name);

                string delegatedTenantId = enrolledTenant.GetDelegatedTenantIdForServiceId(serviceTenant.Id);

                this.logger.LogDebug(
                    "Retrieved delegated tenant with Id '{delegatedTenantId}. Unenrolling from dependencies.",
                    delegatedTenantId);

                foreach (ServiceDependency current in manifest.DependsOnServiceTenants)
                {
                    await this.UnenrollFromServiceAsync(delegatedTenantId, current.Id).ConfigureAwait(false);
                }

                // Now delete the delegated tenant.
                this.logger.LogDebug(
                    "Deleting delegated tenant with Id '{delegatedTenantId}'.",
                    delegatedTenantId);
                await this.tenantProvider.DeleteTenantAsync(delegatedTenantId).ConfigureAwait(false);

                enrolledTenant.ClearDelegatedTenantForService(serviceTenant);
            }

            if (manifest.RequiredConfigurationEntries.Count > 0)
            {
                // Now remove any config for the service that's being unenrolled from.
                this.logger.LogDebug(
                    "Removing configuration for service '{serviceTenantName}' from '{enrolledTenantName}'",
                    serviceTenant.Name,
                    enrolledTenant.Name);

                foreach (ServiceManifestRequiredConfigurationEntry current in manifest.RequiredConfigurationEntries)
                {
                    this.logger.LogDebug(
                        "Removing configuration item '{requiredConfigurationEntryKey}' for service '{serviceTenantName}' from '{enrolledTenantName}'",
                        current.Key,
                        serviceTenant.Name,
                        enrolledTenant.Name);
                    current.RemoveFromTenant(enrolledTenant);
                }
            }

            // Finally, remove the enrollment entry for the service.
            this.logger.LogDebug(
                "Removing enrollment entry for service '{serviceTenantName}' from '{enrolledTenantName}'",
                serviceTenant.Name,
                enrolledTenant.Name);

            enrolledTenant.RemoveServiceEnrollment(serviceTenant.Id);

            this.logger.LogDebug(
                "Updating tenant '{enrolledTenantName}'",
                enrolledTenant.Name);

            await this.tenantProvider.UpdateTenantAsync(enrolledTenant).ConfigureAwait(false);

            this.logger.LogInformation(
                "Successfully unenrolled tenant '{enrolledTenantName}' with Id '{enrolledTenantId}' from service '{serviceTenantName}' with Id '{serviceTenantId}'",
                enrolledTenant.Name,
                enrolledTenant.Id,
                serviceTenant.Name,
                serviceTenant.Id);
        }

        /// <inheritdoc/>
        public async Task<ServiceManifestRequiredConfigurationEntry[]> GetServiceEnrollmentConfigurationRequirementsAsync(string serviceTenantId)
        {
            if (string.IsNullOrWhiteSpace(serviceTenantId))
            {
                throw new ArgumentException(nameof(serviceTenantId));
            }

            ITenant serviceTenant = await this.GetServiceTenantAsync(serviceTenantId).ConfigureAwait(false);

            return await this.GetServiceEnrollmentConfigurationRequirementsAsync(serviceTenant).ConfigureAwait(false);
        }

        private async Task<ServiceManifestRequiredConfigurationEntry[]> GetServiceEnrollmentConfigurationRequirementsAsync(ITenant serviceTenant)
        {
            serviceTenant.EnsureTenantIsOfType(MarainTenantType.Service);

            var requirements = new List<ServiceManifestRequiredConfigurationEntry>();
            ServiceManifest serviceManifest = serviceTenant.GetServiceManifest();
            requirements.AddRange(serviceManifest.RequiredConfigurationEntries);

            ServiceManifestRequiredConfigurationEntry[][] dependentServicesConfigRequirements =
                await Task.WhenAll(
                    serviceManifest.DependsOnServiceTenants.Select(
                        x => this.GetServiceEnrollmentConfigurationRequirementsAsync(x.Id))).ConfigureAwait(false);

            requirements.AddRange(dependentServicesConfigRequirements.SelectMany(x => x));

            return requirements.ToArray();
        }

        private async Task<ITenant> CreateDelegatedTenant(ITenant accessingTenant, ITenant serviceTenant)
        {
            string delegatedTenantName = TenantNames.DelegatedTenant(serviceTenant.Name, accessingTenant.Name);

            this.logger.LogDebug("Creating new delegated tenant '{delegatedTenantName}'", delegatedTenantName);
            ITenant delegatedTenant = await this.tenantProvider.CreateChildTenantAsync(serviceTenant.Id, delegatedTenantName).ConfigureAwait(false);

            this.logger.LogDebug(
                "New delegated tenant '{delegatedTenantName}' created with Id '{tenantId}'; updating tenant type.",
                delegatedTenant.Name,
                delegatedTenant.Id);

            delegatedTenant.SetMarainTenantType(MarainTenantType.Delegated);
            await this.tenantProvider.UpdateTenantAsync(delegatedTenant).ConfigureAwait(false);

            this.logger.LogInformation(
                "Created new delegated tenant '{delegatedTenantName}' with Id '{tenantId}'.",
                delegatedTenant.Name,
                delegatedTenant.Id);
            return delegatedTenant;
        }

        private async Task<ITenant> GetTenantOfTypeAsync(string tenantId, params MarainTenantType[] allowableTenantTypes)
        {
            ITenant tenant = await this.tenantProvider.GetTenantAsync(tenantId).ConfigureAwait(false);
            tenant.EnsureTenantIsOfType(allowableTenantTypes);
            return tenant;
        }

        private Task<ITenant> GetClientTenantParentAsync() =>
            this.GetWellKnownParentTenant(WellKnownTenantIds.ClientTenantParentId);

        private Task<ITenant> GetServiceTenantParentAsync() =>
            this.GetWellKnownParentTenant(WellKnownTenantIds.ServiceTenantParentId);

        private async Task<ITenant> GetWellKnownParentTenant(string parentTenantId)
        {
            try
            {
                return await this.tenantProvider.GetTenantAsync(parentTenantId).ConfigureAwait(false);
            }
            catch (TenantNotFoundException)
            {
                throw new InvalidOperationException("The underlying tenant provider has not been initialised for use with Marain. Please call the ITenantManagementService.InitialiseTenancyProviderAsync before attempting to create tenants.");
            }
        }
    }
}
