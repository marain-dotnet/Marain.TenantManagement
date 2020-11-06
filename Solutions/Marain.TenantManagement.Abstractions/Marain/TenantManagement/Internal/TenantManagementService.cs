// <copyright file="TenantManagementService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Json;
    using Corvus.Tenancy;
    using Corvus.Tenancy.Exceptions;
    using Marain.TenantManagement.Configuration;
    using Marain.TenantManagement.EnrollmentConfiguration;
    using Marain.TenantManagement.ServiceManifests;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Implementation of <see cref="ITenantManagementService"/> over the Corvus <see cref="ITenantProvider"/>.
    /// </summary>
    public class TenantManagementService : ITenantManagementService
    {
        private readonly ITenantStore tenantStore;
        private readonly ILogger<TenantManagementService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantManagementService"/> class.
        /// </summary>
        /// <param name="tenantStore">The underlying tenant store used to access tenants.</param>
        /// <param name="logger">The logger.</param>
        public TenantManagementService(
            ITenantStore tenantStore,
            ILogger<TenantManagementService> logger)
        {
            this.tenantStore = tenantStore;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<ITenant> CreateClientTenantWithWellKnownGuidAsync(Guid wellKnownGuid, string clientName, string? parentId = null)
        {
            if (string.IsNullOrWhiteSpace(clientName))
            {
                throw new ArgumentException(nameof(clientName));
            }

            ITenant parent;

            if (!string.IsNullOrEmpty(parentId))
            {
                parent = await this.GetTenantOfTypeAsync(parentId, MarainTenantType.Client).ConfigureAwait(false);
            }
            else
            {
                parent = await this.GetClientTenantParentAsync().ConfigureAwait(false);
            }

            this.logger.LogDebug("Creating new client tenant '{clientName}' with GUID '{wellKnownGuid}'", clientName, wellKnownGuid);
            ITenant newTenant = await this.tenantStore.CreateWellKnownChildTenantAsync(
                parent.Id,
                wellKnownGuid,
                clientName).ConfigureAwait(false);

            this.logger.LogDebug(
                "New client tenant '{clientName}' created with Id '{tenantId}'; updating tenant type.",
                newTenant.Name,
                newTenant.Id);

            await this.tenantStore.UpdateTenantAsync(
                newTenant.Id,
                propertiesToSetOrAdd: PropertyBagValues.Build(p => p.AddMarainTenantType(MarainTenantType.Client))).ConfigureAwait(false);

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

            ITenant newTenant = await this.tenantStore.CreateWellKnownChildTenantAsync(
                parent.Id,
                manifest.WellKnownTenantGuid,
                manifest.ServiceName).ConfigureAwait(false);

            this.logger.LogDebug(
                "New service tenant '{serviceName}' created with Id '{tenantId}'; updating tenant type and manifest.",
                newTenant.Name,
                newTenant.Id);

            IEnumerable<KeyValuePair<string, object>> properties = PropertyBagValues.Build(p => p
                .AddServiceManifest(manifest)
                .AddMarainTenantType(MarainTenantType.Service));

            await this.tenantStore.UpdateTenantAsync(
                newTenant.Id,
                propertiesToSetOrAdd: properties).ConfigureAwait(false);

            this.logger.LogInformation(
                "Created new service tenant '{serviceName}' with Id '{tenantId}'.",
                newTenant.Name,
                newTenant.Id);
            return newTenant;
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
            TenantCollectionResult existingTopLevelTenantIds = await this.tenantStore.GetChildrenAsync(
                this.tenantStore.Root.Id,
                2,
                null).ConfigureAwait(false);

            if (existingTopLevelTenantIds.Tenants.Count > 1 && !force)
            {
                throw new InvalidOperationException("Cannot initialise the tenancy provider for use with Marain because it already contains non-Marain tenants at the root level. If you wish to initialise anyway, re-invoke the method with the 'force' parameter set to true.");
            }

            // Create the tenants
            if (existingClientTenantParent == null)
            {
                await this.tenantStore.CreateWellKnownChildTenantAsync(
                    this.tenantStore.Root.Id,
                    WellKnownTenantIds.ClientTenantParentGuid,
                    TenantNames.ClientTenantParent).ConfigureAwait(false);
            }

            if (existingServiceTenantParent == null)
            {
                await this.tenantStore.CreateWellKnownChildTenantAsync(
                    this.tenantStore.Root.Id,
                    WellKnownTenantIds.ServiceTenantParentGuid,
                    TenantNames.ServiceTenantParent).ConfigureAwait(false);
            }
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

            IEnumerable<KeyValuePair<string, object>> propertiesToAddToEnrollingTenant = PropertyBagValues.Empty;

            foreach ((ServiceManifestRequiredConfigurationEntry RequiredConfigurationEntry, EnrollmentConfigurationItem ProvidedConfigurationItem) current in matchedConfigItems)
            {
                this.logger.LogDebug(
                    "Adding configuration entry '{requiredConfigurationEntryKey}' to tenant '{serviceTenantName}' with Id '{serviceTenantId}'",
                    current.RequiredConfigurationEntry.Key,
                    serviceTenant.Name,
                    serviceTenant.Id);

                propertiesToAddToEnrollingTenant = current.RequiredConfigurationEntry.AddToTenantProperties(
                    propertiesToAddToEnrollingTenant, current.ProvidedConfigurationItem);
            }

            // Add an enrollment entry to the tenant.
            this.logger.LogDebug(
                "Adding service enrollment to tenant '{serviceTenantName}' with Id '{serviceTenantId}'",
                serviceTenant.Name,
                serviceTenant.Id);

            propertiesToAddToEnrollingTenant = propertiesToAddToEnrollingTenant.AddServiceEnrollment(
                enrollingTenant, serviceTenant.Id);

            // Update the tenant now, so that the tenant type is correctly set - otherwise
            // recursive enrollments will fail
            this.logger.LogDebug(
                "Updating tenant '{enrollingTenantName}' with Id '{enrollingTenantId}'",
                enrollingTenant.Name,
                enrollingTenant.Id);

            enrollingTenant = await this.tenantStore.UpdateTenantAsync(
                enrollingTenant.Id,
                propertiesToSetOrAdd: propertiesToAddToEnrollingTenant)
                .ConfigureAwait(false);

            propertiesToAddToEnrollingTenant = PropertyBagValues.Empty;

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

                propertiesToAddToEnrollingTenant = propertiesToAddToEnrollingTenant.SetDelegatedTenantForService(
                    enrollingTenant, serviceTenant, delegatedTenant);

                this.logger.LogDebug(
                    "Updating tenant '{enrollingTenantName}' with Id '{enrollingTenantId}'",
                    enrollingTenant.Name,
                    enrollingTenant.Id);

                await this.tenantStore.UpdateTenantAsync(
                    enrollingTenant.Id,
                    propertiesToSetOrAdd: propertiesToAddToEnrollingTenant)
                    .ConfigureAwait(false);
            }

            this.logger.LogInformation(
                "Successfully enrolled tenant '{enrollingTenantName}' with Id '{enrollingTenant.Id}' for service '{serviceTenantName}' with Id '{serviceTenantId}'",
                enrollingTenant.Name,
                enrollingTenant.Id,
                serviceTenant.Name,
                serviceTenant.Id);
        }

        /// <inheritdoc/>
        public async Task UnenrollFromServiceAsync(
            ITenant enrolledTenant,
            ITenant serviceTenant)
        {
            if (!enrolledTenant.IsEnrolledForService(serviceTenant.Id))
            {
                throw new InvalidOperationException(
                    $"Cannot unenroll tenant '{enrolledTenant.Name}' with Id '{enrolledTenant.Id}' from service with Id '{serviceTenant.Id}' because it is not currently enrolled");
            }

            this.logger.LogDebug(
                "Unenrolling tenant '{enrolledTenantName}' with Id '{enrolledTenantId}' from service '{serviceTenantName}' with Id '{serviceTenantId}'",
                enrolledTenant.Name,
                enrolledTenant.Id,
                serviceTenant.Name,
                serviceTenant.Id);

            var propertiesToRemove = new List<string>();

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
                await this.tenantStore.DeleteTenantAsync(delegatedTenantId).ConfigureAwait(false);

                propertiesToRemove.AddRange(enrolledTenant.GetPropertiesToRemoveDelegatedTenantForService(serviceTenant));
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
                    propertiesToRemove.AddRange(current.GetPropertiesToRemoveFromTenant(enrolledTenant));
                }
            }

            // Finally, remove the enrollment entry for the service.
            this.logger.LogDebug(
                "Removing enrollment entry for service '{serviceTenantName}' from '{enrolledTenantName}'",
                serviceTenant.Name,
                enrolledTenant.Name);

            IEnumerable<KeyValuePair<string, object>> propertiesToChange =
                enrolledTenant.GetPropertyUpdatesToRemoveServiceEnrollment(serviceTenant.Id);

            this.logger.LogDebug(
                "Updating tenant '{enrolledTenantName}'",
                enrolledTenant.Name);

            await this.tenantStore.UpdateTenantAsync(
                enrolledTenant.Id,
                propertiesToSetOrAdd: propertiesToChange,
                propertiesToRemove: propertiesToRemove).ConfigureAwait(false);

            this.logger.LogInformation(
                "Successfully unenrolled tenant '{enrolledTenantName}' with Id '{enrolledTenantId}' from service '{serviceTenantName}' with Id '{serviceTenantId}'",
                enrolledTenant.Name,
                enrolledTenant.Id,
                serviceTenant.Name,
                serviceTenant.Id);
        }

        /// <inheritdoc/>
        public async Task<ITenant> GetTenantOfTypeAsync(string tenantId, params MarainTenantType[] allowableTenantTypes)
        {
            ITenant tenant = await this.tenantStore.GetTenantAsync(tenantId).ConfigureAwait(false);
            tenant.EnsureTenantIsOfType(allowableTenantTypes);
            return tenant;
        }

        /// <inheritdoc/>
        public async Task<ServiceManifestRequiredConfigurationEntry[]> GetServiceEnrollmentConfigurationRequirementsAsync(ITenant serviceTenant)
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

        /// <inheritdoc/>
        public async Task AddConfigurationAsync(ITenant tenant, ConfigurationItem[] configurationItems)
        {
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }

            tenant.EnsureTenantIsOfType(MarainTenantType.Client, MarainTenantType.Delegated, MarainTenantType.Service);

            if (configurationItems == null)
            {
                throw new ArgumentNullException(nameof(configurationItems));
            }

            this.logger.LogDebug(
                "Add configuration for tenant '{tenantName}' with Id '{tenantId}'",
                tenant.Name,
                tenant.Id);

            configurationItems.ValidateAndThrow();

            IEnumerable<KeyValuePair<string, object>> propertiesToAddToTenant = PropertyBagValues.Empty;

            foreach (ConfigurationItem configurationItem in configurationItems)
            {
                this.logger.LogDebug(
                    "Adding configuration entry '{configurationKey}' to tenant '{tenantName}' with Id '{tenantId}'",
                    configurationItem.Key,
                    tenant.Name,
                    tenant.Id);

                propertiesToAddToTenant = configurationItem.AddConfiguration(propertiesToAddToTenant);
            }

            this.logger.LogDebug(
                "Updating tenant '{tenantName}' with Id '{tenantId}'",
                tenant.Name,
                tenant.Id);

            tenant = await this.tenantStore.UpdateTenantAsync(
                tenant.Id,
                propertiesToSetOrAdd: propertiesToAddToTenant)
                .ConfigureAwait(false);

            this.logger.LogInformation(
                "Successfully added configuration to tenant '{tenantName}' with Id '{tenantId}'",
                tenant.Name,
                tenant.Id);
        }

        private async Task<ITenant> CreateDelegatedTenant(ITenant accessingTenant, ITenant serviceTenant)
        {
            string delegatedTenantName = TenantNames.DelegatedTenant(serviceTenant.Name, accessingTenant.Name);

            this.logger.LogDebug("Creating new delegated tenant '{delegatedTenantName}'", delegatedTenantName);
            ITenant delegatedTenant = await this.tenantStore.CreateChildTenantAsync(serviceTenant.Id, delegatedTenantName).ConfigureAwait(false);

            this.logger.LogDebug(
                "New delegated tenant '{delegatedTenantName}' created with Id '{tenantId}'; updating tenant type.",
                delegatedTenant.Name,
                delegatedTenant.Id);

            delegatedTenant = await this.tenantStore.UpdateTenantAsync(
                delegatedTenant.Id,
                propertiesToSetOrAdd: PropertyBagValues.Build(p => p.AddMarainTenantType(MarainTenantType.Delegated)))
                .ConfigureAwait(false);

            this.logger.LogInformation(
                "Created new delegated tenant '{delegatedTenantName}' with Id '{tenantId}'.",
                delegatedTenant.Name,
                delegatedTenant.Id);
            return delegatedTenant;
        }

        private Task<ITenant> GetClientTenantParentAsync() =>
            this.GetWellKnownParentTenant(WellKnownTenantIds.ClientTenantParentId);

        private Task<ITenant> GetServiceTenantParentAsync() =>
            this.GetWellKnownParentTenant(WellKnownTenantIds.ServiceTenantParentId);

        private async Task<ITenant> GetWellKnownParentTenant(string parentTenantId)
        {
            try
            {
                return await this.tenantStore.GetTenantAsync(parentTenantId).ConfigureAwait(false);
            }
            catch (TenantNotFoundException)
            {
                throw new InvalidOperationException("The underlying tenant provider has not been initialised for use with Marain. Please call the ITenantManagementService.InitialiseTenancyProviderAsync before attempting to create tenants.");
            }
        }
    }
}
