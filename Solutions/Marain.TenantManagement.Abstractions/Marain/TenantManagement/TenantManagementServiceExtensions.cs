// <copyright file="TenantManagementServiceExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Corvus.Tenancy.Exceptions;
    using Marain.TenantManagement.Configuration;
    using Marain.TenantManagement.EnrollmentConfiguration;
    using Marain.TenantManagement.Exceptions;
    using Marain.TenantManagement.ServiceManifests;

    /// <summary>
    /// Extension methods for the <see cref="ITenantManagementService"/> interface.
    /// </summary>
    public static class TenantManagementServiceExtensions
    {
        /// <summary>
        /// Creates a new tenant representing a client.
        /// </summary>
        /// <param name="tenantManagementService">The <see cref="ITenantManagementService"/>.</param>
        /// <param name="clientName">The name of the new tenant.</param>
        /// <param name="parentId">Optional ID of parent Client tenant.</param>
        /// <param name="wellKnownGuid">Optional well-known GUID to use as the client ID for the new tenant.</param>
        /// <returns>The new tenant.</returns>
        public static Task<ITenant> CreateClientTenantAsync(
            this ITenantManagementService tenantManagementService,
            string clientName,
            string? parentId = null,
            Guid? wellKnownGuid = null) => tenantManagementService.CreateClientTenantWithWellKnownGuidAsync(wellKnownGuid ?? Guid.NewGuid(), clientName, parentId);

        /// <summary>
        /// Retrieves the service tenant with the specified Id.
        /// </summary>
        /// <param name="tenantManagementService">The <see cref="ITenantManagementService"/>.</param>
        /// <param name="serviceTenantId">The Id of the service tenant.</param>
        /// <returns>The service tenant.</returns>
        /// <exception cref="TenantNotFoundException">There is no tenant with the specified Id.</exception>
        /// <exception cref="InvalidMarainTenantTypeException">The tenant Id provided is not for a service tenant.</exception>
        public static Task<ITenant> GetServiceTenantAsync(
            this ITenantManagementService tenantManagementService,
            string serviceTenantId)
            => tenantManagementService.GetTenantOfTypeAsync(serviceTenantId, MarainTenantType.Service);

        /// <summary>
        /// Retrieves the client tenant with the specified Id.
        /// </summary>
        /// <param name="tenantManagementService">The <see cref="ITenantManagementService"/>.</param>
        /// <param name="clientTenantId">The Id of the client tenant.</param>
        /// <returns>The client tenant.</returns>
        /// <exception cref="TenantNotFoundException">There is no tenant with the specified Id.</exception>
        /// <exception cref="InvalidMarainTenantTypeException">The tenant Id provided is not for a client tenant.</exception>
        public static Task<ITenant> GetClientTenantAsync(
            this ITenantManagementService tenantManagementService,
            string clientTenantId)
            => tenantManagementService.GetTenantOfTypeAsync(clientTenantId, MarainTenantType.Client);

        /// <summary>
        /// Retrieves the delegated tenant with the specified Id.
        /// </summary>
        /// <param name="tenantManagementService">The <see cref="ITenantManagementService"/>.</param>
        /// <param name="delegatedTenantId">The Id of the client tenant.</param>
        /// <returns>The client tenant.</returns>
        /// <exception cref="TenantNotFoundException">There is no tenant with the specified Id.</exception>
        /// <exception cref="InvalidMarainTenantTypeException">The tenant Id provided is not for a delegated tenant.</exception>
        public static Task<ITenant> GetDelegatedTenantAsync(
            this ITenantManagementService tenantManagementService,
            string delegatedTenantId)
            => tenantManagementService.GetTenantOfTypeAsync(delegatedTenantId, MarainTenantType.Delegated);

        /// <summary>
        /// Retrieves the delegated tenant for the specified client and service.
        /// </summary>
        /// <param name="tenantManagementService">The <see cref="ITenantManagementService"/>.</param>
        /// <param name="clientTenantId">The Id of the client tenant.</param>
        /// <param name="serviceTenantId">
        /// The Id of the service tenant representing the service that needs to make use of the delegated tenant.
        /// </param>
        /// <returns>The client tenant.</returns>
        /// <exception cref="TenantNotFoundException">There is no tenant with the specified client or service tenant Id.</exception>
        /// <exception cref="ArgumentException">There is no delegated tenant for the specified client and service.</exception>
        /// <exception cref="InvalidMarainTenantTypeException">The tenant Ids provided do not match the correct types of tenant.</exception>
        public static async Task<ITenant> GetDelegatedTenantAsync(
            this ITenantManagementService tenantManagementService,
            string clientTenantId,
            string serviceTenantId)
        {
            // Whilst we're saying "client tenant", the client here could itself be a delegated tenant:
            ITenant client = await tenantManagementService.GetTenantOfTypeAsync(
                clientTenantId,
                MarainTenantType.Client,
                MarainTenantType.Delegated).ConfigureAwait(false);

            // This method will throw if there is no delegated tenant.
            string delegatedTenantId = client.GetDelegatedTenantIdForServiceId(serviceTenantId);

            return await tenantManagementService.GetDelegatedTenantAsync(delegatedTenantId).ConfigureAwait(false);
        }

        /// <summary>
        /// Enrolls the specified tenant in the service.
        /// </summary>
        /// <param name="tenantManagementService">The <see cref="ITenantManagementService"/>.</param>
        /// <param name="enrollingTenantId">The Id of the tenant to enroll.</param>
        /// <param name="serviceTenantId">The Id of the service to enroll in.</param>
        /// <param name="configurationItems">Configuration for the enrollment.</param>
        /// <returns>A task which completes when the enrollment has finished.</returns>
        public static async Task EnrollInServiceAsync(
            this ITenantManagementService tenantManagementService,
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
            ITenant enrollingTenant = await tenantManagementService.GetTenantOfTypeAsync(
                enrollingTenantId,
                MarainTenantType.Client,
                MarainTenantType.Delegated).ConfigureAwait(false);

            ITenant serviceTenant = await tenantManagementService.GetServiceTenantAsync(serviceTenantId).ConfigureAwait(false);

            await tenantManagementService.EnrollInServiceAsync(enrollingTenant, serviceTenant, configurationItems).ConfigureAwait(false);
        }

        /// <summary>
        /// Enrolls the specified tenant in the service.
        /// </summary>
        /// <param name="tenantManagementService">The <see cref="ITenantManagementService"/>.</param>
        /// <param name="enrollingTenant">The tenant to enroll.</param>
        /// <param name="serviceTenantId">The Id of the service to enroll in.</param>
        /// <param name="configurationItems">Configuration for the enrollment.</param>
        /// <returns>A task which completes when the enrollment has finished.</returns>
        public static async Task EnrollInServiceAsync(
            this ITenantManagementService tenantManagementService,
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

            ITenant serviceTenant = await tenantManagementService.GetServiceTenantAsync(serviceTenantId).ConfigureAwait(false);

            await tenantManagementService.EnrollInServiceAsync(enrollingTenant, serviceTenant, configurationItems).ConfigureAwait(false);
        }

        /// <summary>
        /// Unenrolls the specified tenant from the service.
        /// </summary>
        /// <param name="tenantManagementService">The <see cref="ITenantManagementService"/>.</param>
        /// <param name="enrolledTenantId">The Id of the tenant that is currently enrolled.</param>
        /// <param name="serviceTenantId">The Id of the service they need to be unenrolled from.</param>
        /// <returns>A task which completes when the unenrollment has finished.</returns>
        public static async Task UnenrollFromServiceAsync(
            this ITenantManagementService tenantManagementService,
            string enrolledTenantId,
            string serviceTenantId)
        {
            if (string.IsNullOrWhiteSpace(enrolledTenantId))
            {
                throw new ArgumentException(nameof(enrolledTenantId));
            }

            ITenant enrolledTenant = await tenantManagementService.GetTenantOfTypeAsync(
                enrolledTenantId,
                MarainTenantType.Client,
                MarainTenantType.Delegated).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(serviceTenantId))
            {
                throw new ArgumentException(nameof(serviceTenantId));
            }

            ITenant serviceTenant = await tenantManagementService.GetServiceTenantAsync(serviceTenantId).ConfigureAwait(false);

            await tenantManagementService.UnenrollFromServiceAsync(enrolledTenant, serviceTenant).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves the complete list of configuration items required in order to enroll a tenant to a service. This will
        /// include configuration entries required by any dependent services.
        /// </summary>
        /// <param name="tenantManagementService">The <see cref="ITenantManagementService"/>.</param>
        /// <param name="serviceTenantId">The Id of the service to gather configuration for.</param>
        /// <returns>
        /// A list of <see cref="ServiceManifestRequiredConfigurationEntry"/> representing the configuration requirements.
        /// </returns>
        public static async Task<ServiceManifestRequiredConfigurationEntry[]> GetServiceEnrollmentConfigurationRequirementsAsync(
            this ITenantManagementService tenantManagementService,
            string serviceTenantId)
        {
            if (string.IsNullOrWhiteSpace(serviceTenantId))
            {
                throw new ArgumentException(nameof(serviceTenantId));
            }

            ITenant serviceTenant = await tenantManagementService.GetServiceTenantAsync(serviceTenantId).ConfigureAwait(false);

            return await tenantManagementService.GetServiceEnrollmentConfigurationRequirementsAsync(serviceTenant).ConfigureAwait(false);
        }

        /// <summary>
        /// Add or updates arbitrary storage configuration for a tenant.
        /// </summary>
        /// <param name="tenantManagementService">The <see cref="ITenantManagementService"/>.</param>
        /// <param name="tenantId">The ID of the tenant to enroll.</param>
        /// <param name="configurationItems">Configuration to add.</param>
        /// <returns>A task which completes when the configuration has been added.</returns>
        public static async Task AddOrUpdateStorageConfigurationAsync(
            this ITenantManagementService tenantManagementService,
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

            ITenant tenant = await tenantManagementService.GetTenantOfTypeAsync(
                tenantId,
                MarainTenantType.Client,
                MarainTenantType.Delegated,
                MarainTenantType.Service).ConfigureAwait(false);

            await tenantManagementService.AddOrUpdateStorageConfigurationAsync(tenant, configurationItems).ConfigureAwait(false);
        }
    }
}
