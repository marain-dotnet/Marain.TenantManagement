// <copyright file="TenantStoreManagementExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement
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
    using Marain.TenantManagement.Exceptions;
    using Marain.TenantManagement.Internal;
    using Marain.TenantManagement.ServiceManifests;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Management extension methods for the <see cref="ITenantStore"/> interface.
    /// </summary>
    public static class TenantStoreManagementExtensions
    {
        /// <summary>
        /// Creates a new tenant representing a client.
        /// </summary>
        /// <param name="tenantStore">The <see cref="ITenantStore"/>.</param>
        /// <param name="clientName">The name of the new tenant.</param>
        /// <param name="parentId">Optional ID of parent Client tenant.</param>
        /// <param name="wellKnownGuid">Optional well-known GUID to use as the client ID for the new tenant.</param>
        /// <returns>The new tenant.</returns>
        public static Task<ITenant> CreateClientTenantAsync(
            this ITenantStore tenantStore,
            string clientName,
            string? parentId = null,
            Guid? wellKnownGuid = null) => tenantStore.CreateClientTenantWithWellKnownGuidAsync(wellKnownGuid ?? Guid.NewGuid(), clientName, parentId);

        /// <summary>
        /// Retrieves the service tenant with the specified Id.
        /// </summary>
        /// <param name="tenantStore">The <see cref="ITenantStore"/>.</param>
        /// <param name="serviceTenantId">The Id of the service tenant.</param>
        /// <returns>The service tenant.</returns>
        /// <exception cref="TenantNotFoundException">There is no tenant with the specified Id.</exception>
        /// <exception cref="InvalidMarainTenantTypeException">The tenant Id provided is not for a service tenant.</exception>
        public static Task<ITenant> GetServiceTenantAsync(
            this ITenantStore tenantStore,
            string serviceTenantId)
            => tenantStore.GetTenantOfTypeAsync(serviceTenantId, MarainTenantType.Service);

        /// <summary>
        /// Retrieves the client tenant with the specified Id.
        /// </summary>
        /// <param name="tenantStore">The <see cref="ITenantStore"/>.</param>
        /// <param name="clientTenantId">The Id of the client tenant.</param>
        /// <returns>The client tenant.</returns>
        /// <exception cref="TenantNotFoundException">There is no tenant with the specified Id.</exception>
        /// <exception cref="InvalidMarainTenantTypeException">The tenant Id provided is not for a client tenant.</exception>
        public static Task<ITenant> GetClientTenantAsync(
            this ITenantStore tenantStore,
            string clientTenantId)
            => tenantStore.GetTenantOfTypeAsync(clientTenantId, MarainTenantType.Client);

        /// <summary>
        /// Retrieves the delegated tenant with the specified Id.
        /// </summary>
        /// <param name="tenantStore">The <see cref="ITenantStore"/>.</param>
        /// <param name="delegatedTenantId">The Id of the client tenant.</param>
        /// <returns>The client tenant.</returns>
        /// <exception cref="TenantNotFoundException">There is no tenant with the specified Id.</exception>
        /// <exception cref="InvalidMarainTenantTypeException">The tenant Id provided is not for a delegated tenant.</exception>
        public static Task<ITenant> GetDelegatedTenantAsync(
            this ITenantStore tenantStore,
            string delegatedTenantId)
            => tenantStore.GetTenantOfTypeAsync(delegatedTenantId, MarainTenantType.Delegated);

        /// <summary>
        /// Retrieves the delegated tenant for the specified client and service.
        /// </summary>
        /// <param name="tenantStore">The <see cref="ITenantStore"/>.</param>
        /// <param name="clientTenantId">The Id of the client tenant.</param>
        /// <param name="serviceTenantId">
        /// The Id of the service tenant representing the service that needs to make use of the delegated tenant.
        /// </param>
        /// <returns>The client tenant.</returns>
        /// <exception cref="TenantNotFoundException">There is no tenant with the specified client or service tenant Id.</exception>
        /// <exception cref="ArgumentException">There is no delegated tenant for the specified client and service.</exception>
        /// <exception cref="InvalidMarainTenantTypeException">The tenant Ids provided do not match the correct types of tenant.</exception>
        public static async Task<ITenant> GetDelegatedTenantAsync(
            this ITenantStore tenantStore,
            string clientTenantId,
            string serviceTenantId)
        {
            // Whilst we're saying "client tenant", the client here could itself be a delegated tenant:
            ITenant client = await tenantStore.GetTenantOfTypeAsync(
                clientTenantId,
                MarainTenantType.Client,
                MarainTenantType.Delegated).ConfigureAwait(false);

            // This method will throw if there is no delegated tenant.
            string delegatedTenantId = client.GetDelegatedTenantIdForServiceId(serviceTenantId);

            return await tenantStore.GetDelegatedTenantAsync(delegatedTenantId).ConfigureAwait(false);
        }

        /// <summary>
        /// Enrolls the specified tenant in the service.
        /// </summary>
        /// <param name="tenantStore">The <see cref="ITenantStore"/>.</param>
        /// <param name="enrollingTenantId">The Id of the tenant to enroll.</param>
        /// <param name="serviceTenantId">The Id of the service to enroll in.</param>
        /// <param name="configurationItems">Configuration for the enrollment.</param>
        /// <returns>A task which completes when the enrollment has finished.</returns>
        public static async Task EnrollInServiceAsync(
            this ITenantStore tenantStore,
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
            ITenant enrollingTenant = await tenantStore.GetTenantOfTypeAsync(
                enrollingTenantId,
                MarainTenantType.Client,
                MarainTenantType.Delegated).ConfigureAwait(false);

            ITenant serviceTenant = await tenantStore.GetServiceTenantAsync(serviceTenantId).ConfigureAwait(false);

            await tenantStore.EnrollInServiceAsync(enrollingTenant, serviceTenant, configurationItems).ConfigureAwait(false);
        }

        /// <summary>
        /// Enrolls the specified tenant in the service.
        /// </summary>
        /// <param name="tenantStore">The <see cref="ITenantStore"/>.</param>
        /// <param name="enrollingTenant">The tenant to enroll.</param>
        /// <param name="serviceTenantId">The Id of the service to enroll in.</param>
        /// <param name="configurationItems">Configuration for the enrollment.</param>
        /// <returns>A task which completes when the enrollment has finished.</returns>
        public static async Task EnrollInServiceAsync(
            this ITenantStore tenantStore,
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

            ITenant serviceTenant = await tenantStore.GetServiceTenantAsync(serviceTenantId).ConfigureAwait(false);

            await tenantStore.EnrollInServiceAsync(enrollingTenant, serviceTenant, configurationItems).ConfigureAwait(false);
        }

        /// <summary>
        /// Unenrolls the specified tenant from the service.
        /// </summary>
        /// <param name="tenantStore">The <see cref="ITenantStore"/>.</param>
        /// <param name="enrolledTenantId">The Id of the tenant that is currently enrolled.</param>
        /// <param name="serviceTenantId">The Id of the service they need to be unenrolled from.</param>
        /// <returns>A task which completes when the unenrollment has finished.</returns>
        public static async Task UnenrollFromServiceAsync(
            this ITenantStore tenantStore,
            string enrolledTenantId,
            string serviceTenantId)
        {
            if (string.IsNullOrWhiteSpace(enrolledTenantId))
            {
                throw new ArgumentException(nameof(enrolledTenantId));
            }

            ITenant enrolledTenant = await tenantStore.GetTenantOfTypeAsync(
                enrolledTenantId,
                MarainTenantType.Client,
                MarainTenantType.Delegated).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(serviceTenantId))
            {
                throw new ArgumentException(nameof(serviceTenantId));
            }

            ITenant serviceTenant = await tenantStore.GetServiceTenantAsync(serviceTenantId).ConfigureAwait(false);

            await tenantStore.UnenrollFromServiceAsync(enrolledTenant, serviceTenant).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves the complete list of configuration items required in order to enroll a tenant to a service. This will
        /// include configuration entries required by any dependent services.
        /// </summary>
        /// <param name="tenantStore">The <see cref="ITenantStore"/>.</param>
        /// <param name="serviceTenantId">The Id of the service to gather configuration for.</param>
        /// <returns>
        /// A list of <see cref="ServiceManifestRequiredConfigurationEntry"/> representing the configuration requirements.
        /// </returns>
        public static async Task<ServiceManifestRequiredConfigurationEntry[]> GetServiceEnrollmentConfigurationRequirementsAsync(
            this ITenantStore tenantStore,
            string serviceTenantId)
        {
            if (string.IsNullOrWhiteSpace(serviceTenantId))
            {
                throw new ArgumentException(nameof(serviceTenantId));
            }

            ITenant serviceTenant = await tenantStore.GetServiceTenantAsync(serviceTenantId).ConfigureAwait(false);

            return await tenantStore.GetServiceEnrollmentConfigurationRequirementsAsync(serviceTenant).ConfigureAwait(false);
        }

        /// <summary>
        /// Add or updates arbitrary storage configuration for a tenant.
        /// </summary>
        /// <param name="tenantStore">The <see cref="ITenantStore"/>.</param>
        /// <param name="tenantId">The ID of the tenant to enroll.</param>
        /// <param name="configurationItems">Configuration to add.</param>
        /// <returns>A task which completes when the configuration has been added.</returns>
        public static async Task AddOrUpdateStorageConfigurationAsync(
            this ITenantStore tenantStore,
            string tenantId,
            ConfigurationItem[] configurationItems)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                throw new ArgumentException(nameof(tenantId));
            }

            if (configurationItems == null)
            {
                throw new ArgumentNullException(nameof(configurationItems));
            }

            ITenant tenant = await tenantStore.GetTenantOfTypeAsync(
                tenantId,
                MarainTenantType.Client).ConfigureAwait(false);

            await tenantStore.AddOrUpdateStorageConfigurationAsync(tenant, configurationItems).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new tenant representing a client, using a well known Guid.
        /// </summary>
        /// <param name="tenantStore">The tenant store.</param>
        /// <param name="wellKnownGuid">The well known Guid to use when creating the tenant.</param>
        /// <param name="clientName">The name of the new tenant.</param>
        /// <param name="parentId">Optional ID of parent Client tenant.</param>
        /// <param name="logger">Optional logger.</param>
        /// <returns>The new tenant.</returns>
        public static async Task<ITenant> CreateClientTenantWithWellKnownGuidAsync(this ITenantStore tenantStore, Guid wellKnownGuid, string clientName, string? parentId = null, ILogger? logger = null)
        {
            if (string.IsNullOrWhiteSpace(clientName))
            {
                throw new ArgumentException(nameof(clientName));
            }

            ITenant parent;

            if (!string.IsNullOrEmpty(parentId))
            {
                parent = await tenantStore.GetTenantOfTypeAsync(parentId, MarainTenantType.Client).ConfigureAwait(false);
            }
            else
            {
                parent = await tenantStore.GetClientTenantParentAsync().ConfigureAwait(false);
            }

            logger?.LogDebug("Creating new client tenant '{clientName}' with GUID '{wellKnownGuid}'", clientName, wellKnownGuid);
            ITenant newTenant = await tenantStore.CreateWellKnownChildTenantAsync(
                parent.Id,
                wellKnownGuid,
                clientName).ConfigureAwait(false);

            logger?.LogDebug(
                "New client tenant '{clientName}' created with Id '{tenantId}'; updating tenant type.",
                newTenant.Name,
                newTenant.Id);

            await tenantStore.UpdateTenantAsync(
                newTenant.Id,
                propertiesToSetOrAdd: PropertyBagValues.Build(p => p.AddMarainTenantType(MarainTenantType.Client))).ConfigureAwait(false);

            logger?.LogInformation(
                "Created new client tenant '{clientName}' with Id '{tenantId}'.",
                newTenant.Name,
                newTenant.Id);
            return newTenant;
        }

        /// <summary>
        /// Creates a new tenant representing a Marain service that tenants can enroll to use.
        /// </summary>
        /// <param name="tenantStore">The tenant store.</param>
        /// <param name="manifest">
        /// The manifest for the service. The service name in the manifest must be unique across all service tenants.
        /// </param>
        /// <param name="logger">Optional logger.</param>
        /// <returns>The new tenant.</returns>
        public static async Task<ITenant> CreateServiceTenantAsync(this ITenantStore tenantStore, ServiceManifest manifest, ILogger? logger = null)
        {
            if (manifest == null)
            {
                throw new ArgumentNullException(nameof(manifest));
            }

            logger?.LogDebug("Validating service manifest for service creation.");

            await manifest.ValidateAndThrowAsync(tenantStore).ConfigureAwait(false);

            ITenant parent = await tenantStore.GetServiceTenantParentAsync().ConfigureAwait(false);

            logger?.LogDebug(
                            "Creating new service tenant '{serviceName}' with well-known GUID '{wellKnownGuid}'",
                            manifest.ServiceName,
                            manifest.WellKnownTenantGuid);

            ITenant newTenant = await tenantStore.CreateWellKnownChildTenantAsync(
                            parent.Id,
                            manifest.WellKnownTenantGuid,
                            manifest.ServiceName).ConfigureAwait(false);

            logger?.LogDebug(
                            "New service tenant '{serviceName}' created with Id '{tenantId}'; updating tenant type and manifest.",
                            newTenant.Name,
                            newTenant.Id);

            IEnumerable<KeyValuePair<string, object>> properties = PropertyBagValues.Build(p => p
                            .AddServiceManifest(manifest)
                            .AddMarainTenantType(MarainTenantType.Service));

            await tenantStore.UpdateTenantAsync(
            newTenant.Id,
            propertiesToSetOrAdd: properties).ConfigureAwait(false);

            logger?.LogInformation(
                            "Created new service tenant '{serviceName}' with Id '{tenantId}'.",
                            newTenant.Name,
                            newTenant.Id);
            return newTenant;
        }

        /// <summary>
        /// Initialises a tenancy provider to prepare it for use with Marain.
        /// </summary>
        /// <param name="tenantStore">The tenant store.</param>
        /// <param name="force">
        /// If <c>false</c> and the tenant provider contains top-level tenants (other than the expected Client and Service
        /// tenant parents) then an <see cref="InvalidOperationException"/> will be thrown. If <c>true</c>, no exception will
        /// be thrown and the parent tenants will be created regardless.
        /// </param>
        /// <param name="logger">Optional logger.</param>
        /// <remarks>
        /// Invoking this method ensures that the two top-level parent tenants are in place to act as containers for the
        /// Client and Service tenants.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the initialisation operation.</returns>
        public static async Task InitialiseTenancyProviderAsync(this ITenantStore tenantStore, bool force = false, ILogger? logger = null)
        {
            ITenant? existingClientTenantParent = null;
            try
            {
                existingClientTenantParent = await tenantStore.GetClientTenantParentAsync().ConfigureAwait(false);
            }
            catch (InvalidOperationException)
            {
                // No-op - this is expected if the provider isn't yet initialised.
            }

            ITenant? existingServiceTenantParent = null;

            try
            {
                existingServiceTenantParent = await tenantStore.GetServiceTenantParentAsync().ConfigureAwait(false);
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
            TenantCollectionResult existingTopLevelTenantIds = await tenantStore.GetChildrenAsync(
                tenantStore.Root.Id,
                2,
                null).ConfigureAwait(false);

            if (existingTopLevelTenantIds.Tenants.Count > 1 && !force)
            {
                throw new InvalidOperationException("Cannot initialise the tenancy provider for use with Marain because it already contains non-Marain tenants at the root level. If you wish to initialise anyway, re-invoke the method with the 'force' parameter set to true.");
            }

            // Create the tenants
            if (existingClientTenantParent == null)
            {
                await tenantStore.CreateWellKnownChildTenantAsync(
                    tenantStore.Root.Id,
                    WellKnownTenantIds.ClientTenantParentGuid,
                    TenantNames.ClientTenantParent).ConfigureAwait(false);
            }

            if (existingServiceTenantParent == null)
            {
                await tenantStore.CreateWellKnownChildTenantAsync(
                    tenantStore.Root.Id,
                    WellKnownTenantIds.ServiceTenantParentGuid,
                    TenantNames.ServiceTenantParent).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Enrolls the specified tenant in the service.
        /// </summary>
        /// <param name="tenantStore">The tenant store.</param>
        /// <param name="enrollingTenant">The tenant to enroll.</param>
        /// <param name="serviceTenant">The service to enroll in.</param>
        /// <param name="configurationItems">Configuration for the enrollment.</param>
        /// <param name="logger">Optional logger.</param>
        /// <returns>A task which completes when the enrollment has finished.</returns>
        public static async Task EnrollInServiceAsync(
            this ITenantStore tenantStore,
            ITenant enrollingTenant,
            ITenant serviceTenant,
            EnrollmentConfigurationItem[] configurationItems,
            ILogger? logger = null)
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

            logger?.LogDebug(
                "Enrolling tenant '{enrollingTenantName}' with Id '{enrollingTenantId}' from service '{serviceTenantName}' with Id '{serviceTenantId}'",
                enrollingTenant.Name,
                enrollingTenant.Id,
                serviceTenant.Name,
                serviceTenant.Id);

            // First we need to ensure that all the required config items for both the service being enrolled in,
            // as well as any dependent services is provided.
            ServiceManifestRequiredConfigurationEntry[] requiredConfig = await tenantStore.GetServiceEnrollmentConfigurationRequirementsAsync(serviceTenant).ConfigureAwait(false);

            logger?.LogDebug("Validating supplied configuration against required config.");
            configurationItems.ValidateAndThrow(requiredConfig);

            ServiceManifest manifest = serviceTenant.GetServiceManifest();

            // Now, match up the required config items for this service to the relevent supplied config (we may
            // have been supplied with config for dependent services as well, so we can't just attach all
            // of the supplied config items to the enrolling tenant - some of it could belong on delegated
            // tenants.
            logger?.LogDebug(
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
                logger?.LogDebug(
                    "Adding configuration entry '{requiredConfigurationEntryKey}' to tenant '{serviceTenantName}' with Id '{serviceTenantId}'",
                    current.RequiredConfigurationEntry.Key,
                    serviceTenant.Name,
                    serviceTenant.Id);

                propertiesToAddToEnrollingTenant = current.RequiredConfigurationEntry.AddToTenantProperties(
                    propertiesToAddToEnrollingTenant, current.ProvidedConfigurationItem);
            }

            // Add an enrollment entry to the tenant.
            logger?.LogDebug(
                "Adding service enrollment to tenant '{serviceTenantName}' with Id '{serviceTenantId}'",
                serviceTenant.Name,
                serviceTenant.Id);

            propertiesToAddToEnrollingTenant = propertiesToAddToEnrollingTenant.AddServiceEnrollment(
                enrollingTenant, serviceTenant.Id);

            // Update the tenant now, so that the tenant type is correctly set - otherwise
            // recursive enrollments will fail
            logger?.LogDebug(
                "Updating tenant '{enrollingTenantName}' with Id '{enrollingTenantId}'",
                enrollingTenant.Name,
                enrollingTenant.Id);

            enrollingTenant = await tenantStore.UpdateTenantAsync(
                enrollingTenant.Id,
                propertiesToSetOrAdd: propertiesToAddToEnrollingTenant)
                .ConfigureAwait(false);

            propertiesToAddToEnrollingTenant = PropertyBagValues.Empty;

            // If this service has dependencies, we need to create a new delegated tenant for the service to use when
            // accessing those dependencies.
            if (manifest.DependsOnServiceTenants.Count > 0)
            {
                logger?.LogDebug(
                    "Service '{serviceTenantName}' has dependencies. Creating delegated tenant for enrollment.",
                    serviceTenant.Name);

                ITenant delegatedTenant = await tenantStore.CreateDelegatedTenant(enrollingTenant, serviceTenant).ConfigureAwait(false);

                // Now enroll the new delegated tenant for all of the dependent services.
                await Task.WhenAll(manifest.DependsOnServiceTenants.Select(
                    dependsOnService => tenantStore.EnrollInServiceAsync(
                        delegatedTenant,
                        dependsOnService.Id,
                        configurationItems))).ConfigureAwait(false);

                // Add the delegated tenant Id to the enrolling tenant
                logger?.LogDebug(
                    "Setting delegated tenant for client '{enrollingTenantName}' with Id '{enrollingTenantId}' in service '{serviceTenantName}' with Id '{serviceTenantId}' to new tenant '{delegatedTenantName}' with Id '{delegatedTenantId}'.",
                    enrollingTenant.Name,
                    enrollingTenant.Id,
                    serviceTenant.Name,
                    serviceTenant.Id,
                    delegatedTenant.Name,
                    delegatedTenant.Id);

                propertiesToAddToEnrollingTenant = propertiesToAddToEnrollingTenant.SetDelegatedTenantForService(
                    enrollingTenant, serviceTenant, delegatedTenant);

                logger?.LogDebug(
                    "Updating tenant '{enrollingTenantName}' with Id '{enrollingTenantId}'",
                    enrollingTenant.Name,
                    enrollingTenant.Id);

                await tenantStore.UpdateTenantAsync(
                    enrollingTenant.Id,
                    propertiesToSetOrAdd: propertiesToAddToEnrollingTenant)
                    .ConfigureAwait(false);
            }

            logger?.LogInformation(
                "Successfully enrolled tenant '{enrollingTenantName}' with Id '{enrollingTenant.Id}' for service '{serviceTenantName}' with Id '{serviceTenantId}'",
                enrollingTenant.Name,
                enrollingTenant.Id,
                serviceTenant.Name,
                serviceTenant.Id);
        }

        /// <summary>
        /// Unenrolls the specified tenant from the service.
        /// </summary>
        /// <param name="tenantStore">The tenant store.</param>
        /// <param name="enrolledTenant">The tenant that is currently enrolled.</param>
        /// <param name="serviceTenant">The service they need to be unenrolled from.</param>
        /// <param name="logger">Optional logger.</param>
        /// <returns>A task which completes when the unenrollment has finished.</returns>
        public static async Task UnenrollFromServiceAsync(
            this ITenantStore tenantStore,
            ITenant enrolledTenant,
            ITenant serviceTenant,
            ILogger? logger = null)
        {
            if (!enrolledTenant.IsEnrolledForService(serviceTenant.Id))
            {
                throw new InvalidOperationException(
                    $"Cannot unenroll tenant '{enrolledTenant.Name}' with Id '{enrolledTenant.Id}' from service with Id '{serviceTenant.Id}' because it is not currently enrolled");
            }

            logger?.LogDebug(
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
                logger?.LogDebug(
                    "Service '{serviceTenantName}' has dependencies. Retrieving delegated tenant for unenrollment.",
                    serviceTenant.Name);

                string delegatedTenantId = enrolledTenant.GetDelegatedTenantIdForServiceId(serviceTenant.Id);

                logger?.LogDebug(
                    "Retrieved delegated tenant with Id '{delegatedTenantId}. Unenrolling from dependencies.",
                    delegatedTenantId);

                foreach (ServiceDependency current in manifest.DependsOnServiceTenants)
                {
                    await tenantStore.UnenrollFromServiceAsync(delegatedTenantId, current.Id).ConfigureAwait(false);
                }

                // Now delete the delegated tenant.
                logger?.LogDebug(
                    "Deleting delegated tenant with Id '{delegatedTenantId}'.",
                    delegatedTenantId);
                await tenantStore.DeleteTenantAsync(delegatedTenantId).ConfigureAwait(false);

                propertiesToRemove.AddRange(enrolledTenant.GetPropertiesToRemoveDelegatedTenantForService(serviceTenant));
            }

            if (manifest.RequiredConfigurationEntries.Count > 0)
            {
                // Now remove any config for the service that's being unenrolled from.
                logger?.LogDebug(
                    "Removing configuration for service '{serviceTenantName}' from '{enrolledTenantName}'",
                    serviceTenant.Name,
                    enrolledTenant.Name);

                foreach (ServiceManifestRequiredConfigurationEntry current in manifest.RequiredConfigurationEntries)
                {
                    logger?.LogDebug(
                        "Removing configuration item '{requiredConfigurationEntryKey}' for service '{serviceTenantName}' from '{enrolledTenantName}'",
                        current.Key,
                        serviceTenant.Name,
                        enrolledTenant.Name);
                    propertiesToRemove.AddRange(current.GetPropertiesToRemoveFromTenant(enrolledTenant));
                }
            }

            // Finally, remove the enrollment entry for the service.
            logger?.LogDebug(
                "Removing enrollment entry for service '{serviceTenantName}' from '{enrolledTenantName}'",
                serviceTenant.Name,
                enrolledTenant.Name);

            IEnumerable<KeyValuePair<string, object>> propertiesToChange =
                enrolledTenant.GetPropertyUpdatesToRemoveServiceEnrollment(serviceTenant.Id);

            logger?.LogDebug(
                "Updating tenant '{enrolledTenantName}'",
                enrolledTenant.Name);

            await tenantStore.UpdateTenantAsync(
                enrolledTenant.Id,
                propertiesToSetOrAdd: propertiesToChange,
                propertiesToRemove: propertiesToRemove).ConfigureAwait(false);

            logger?.LogInformation(
                "Successfully unenrolled tenant '{enrolledTenantName}' with Id '{enrolledTenantId}' from service '{serviceTenantName}' with Id '{serviceTenantId}'",
                enrolledTenant.Name,
                enrolledTenant.Id,
                serviceTenant.Name,
                serviceTenant.Id);
        }

        /// <summary>
        /// Retrieves the tenant with the specified Id and ensures it is of the correct type.
        /// </summary>
        /// <param name="tenantStore">The tenant store.</param>
        /// <param name="tenantId">The Id of the tenant to retrieve.</param>
        /// <param name="allowableTenantTypes">The list of valid types for the tenant.</param>
        /// <returns>The client tenant.</returns>
        /// <exception cref="TenantNotFoundException">There is no tenant with the specified Id.</exception>
        /// <exception cref="InvalidMarainTenantTypeException">The tenant Id provided is not for a tenant with the specified type.</exception>
        public static async Task<ITenant> GetTenantOfTypeAsync(this ITenantStore tenantStore, string tenantId, params MarainTenantType[] allowableTenantTypes)
        {
            ITenant tenant = await tenantStore.GetTenantAsync(tenantId).ConfigureAwait(false);
            tenant.EnsureTenantIsOfType(allowableTenantTypes);
            return tenant;
        }

        /// <summary>
        /// Retrieves the complete list of configuration items required in order to enroll a tenant to a service. This will
        /// include configuration entries required by any dependent services.
        /// </summary>
        /// <param name="tenantStore">The tenant store.</param>
        /// <param name="serviceTenant">The service to gather configuration for.</param>
        /// <returns>
        /// A list of <see cref="ServiceManifestRequiredConfigurationEntry"/> representing the configuration requirements.
        /// </returns>
        public static async Task<ServiceManifestRequiredConfigurationEntry[]> GetServiceEnrollmentConfigurationRequirementsAsync(this ITenantStore tenantStore, ITenant serviceTenant)
        {
            serviceTenant.EnsureTenantIsOfType(MarainTenantType.Service);

            var requirements = new List<ServiceManifestRequiredConfigurationEntry>();
            ServiceManifest serviceManifest = serviceTenant.GetServiceManifest();
            requirements.AddRange(serviceManifest.RequiredConfigurationEntries);

            ServiceManifestRequiredConfigurationEntry[][] dependentServicesConfigRequirements =
                await Task.WhenAll(
                    serviceManifest.DependsOnServiceTenants.Select(
                        x => tenantStore.GetServiceEnrollmentConfigurationRequirementsAsync(x.Id))).ConfigureAwait(false);

            requirements.AddRange(dependentServicesConfigRequirements.SelectMany(x => x));

            return requirements.ToArray();
        }

        /// <summary>
        /// Add or updates arbitrary storage configuration for a tenant.
        /// </summary>
        /// <param name="tenantStore">The tenant store.</param>
        /// <param name="tenant">The tenant to enroll.</param>
        /// <param name="configurationItems">Configuration to add.</param>
        /// <param name="logger">Optional logger.</param>
        /// <returns>A task which completes when the configuration has been added.</returns>
        public static async Task AddOrUpdateStorageConfigurationAsync(this ITenantStore tenantStore, ITenant tenant, ConfigurationItem[] configurationItems, ILogger? logger = null)
        {
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }

            tenant.EnsureTenantIsOfType(MarainTenantType.Client);

            if (configurationItems == null)
            {
                throw new ArgumentNullException(nameof(configurationItems));
            }

            logger?.LogDebug(
                "Add configuration for tenant '{tenantName}' with Id '{tenantId}'",
                tenant.Name,
                tenant.Id);

            configurationItems.ValidateAndThrow();

            IEnumerable<KeyValuePair<string, object>> propertiesToAddToTenant = PropertyBagValues.Empty;

            foreach (ConfigurationItem configurationItem in configurationItems)
            {
                logger?.LogDebug(
                    "Adding configuration entry to tenant '{tenantName}' with Id '{tenantId}'",
                    tenant.Name,
                    tenant.Id);

                propertiesToAddToTenant = configurationItem.AddConfiguration(propertiesToAddToTenant);
            }

            logger?.LogDebug(
                "Updating tenant '{tenantName}' with Id '{tenantId}'",
                tenant.Name,
                tenant.Id);

            tenant = await tenantStore.UpdateTenantAsync(
                tenant.Id,
                propertiesToSetOrAdd: propertiesToAddToTenant)
                .ConfigureAwait(false);

            logger?.LogInformation(
                "Successfully added configuration to tenant '{tenantName}' with Id '{tenantId}'",
                tenant.Name,
                tenant.Id);
        }

        private static async Task<ITenant> CreateDelegatedTenant(this ITenantStore tenantStore, ITenant accessingTenant, ITenant serviceTenant, ILogger? logger = null)
        {
            string delegatedTenantName = TenantNames.DelegatedTenant(serviceTenant.Name, accessingTenant.Name);

            logger?.LogDebug("Creating new delegated tenant '{delegatedTenantName}'", delegatedTenantName);
            ITenant delegatedTenant = await tenantStore.CreateChildTenantAsync(serviceTenant.Id, delegatedTenantName).ConfigureAwait(false);

            logger?.LogDebug(
                "New delegated tenant '{delegatedTenantName}' created with Id '{tenantId}'; updating tenant type.",
                delegatedTenant.Name,
                delegatedTenant.Id);

            delegatedTenant = await tenantStore.UpdateTenantAsync(
                delegatedTenant.Id,
                propertiesToSetOrAdd: PropertyBagValues.Build(p => p.AddMarainTenantType(MarainTenantType.Delegated)))
                .ConfigureAwait(false);

            logger?.LogInformation(
                "Created new delegated tenant '{delegatedTenantName}' with Id '{tenantId}'.",
                delegatedTenant.Name,
                delegatedTenant.Id);
            return delegatedTenant;
        }

        private static Task<ITenant> GetClientTenantParentAsync(this ITenantStore tenantStore) =>
            tenantStore.GetWellKnownParentTenant(WellKnownTenantIds.ClientTenantParentId);

        private static Task<ITenant> GetServiceTenantParentAsync(this ITenantStore tenantStore) =>
            tenantStore.GetWellKnownParentTenant(WellKnownTenantIds.ServiceTenantParentId);

        private static async Task<ITenant> GetWellKnownParentTenant(this ITenantStore tenantStore, string parentTenantId)
        {
            try
            {
                return await tenantStore.GetTenantAsync(parentTenantId).ConfigureAwait(false);
            }
            catch (TenantNotFoundException)
            {
                throw new InvalidOperationException("The underlying tenant provider has not been initialised for use with Marain. Please call the ITenantManagementService.InitialiseTenancyProviderAsync before attempting to create tenants.");
            }
        }
    }
}
