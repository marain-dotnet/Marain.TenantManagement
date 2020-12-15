// <copyright file="ITenantManagementService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Corvus.Tenancy.Exceptions;
    using Marain.TenantManagement.Configuration;
    using Marain.TenantManagement.EnrollmentConfiguration;
    using Marain.TenantManagement.Exceptions;
    using Marain.TenantManagement.ServiceManifests;

    /// <summary>
    /// Provides standard methods for managing Marain tenants.
    /// </summary>
    public interface ITenantManagementService
    {
        /// <summary>
        /// Initialises a tenancy provider to prepare it for use with Marain.
        /// </summary>
        /// <param name="force">
        /// If <c>false</c> and the tenant provider contains top-level tenants (other than the expected Client and Service
        /// tenant parents) then an <see cref="InvalidOperationException"/> will be thrown. If <c>true</c>, no exception will
        /// be thrown and the parent tenants will be created regardless.
        /// </param>
        /// <remarks>
        /// Invoking this method ensures that the two top-level parent tenants are in place to act as containers for the
        /// Client and Service tenants.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the initialisation operation.</returns>
        Task InitialiseTenancyProviderAsync(bool force = false);

        /// <summary>
        /// Creates a new tenant representing a client, using a well known Guid.
        /// </summary>
        /// <param name="wellKnownGuid">The well known Guid to use when creating the tenant.</param>
        /// <param name="clientName">The name of the new tenant.</param>
        /// <param name="parentId">Optional ID of parent Client tenant.</param>
        /// <returns>The new tenant.</returns>
        Task<ITenant> CreateClientTenantWithWellKnownGuidAsync(Guid wellKnownGuid, string clientName, string? parentId = null);

        /// <summary>
        /// Creates a new tenant representing a Marain service that tenants can enroll to use.
        /// </summary>
        /// <param name="manifest">
        /// The manifest for the service. The service name in the manifest must be unique across all service tenants.
        /// </param>
        /// <returns>The new tenant.</returns>
        Task<ITenant> CreateServiceTenantAsync(ServiceManifest manifest);

        /// <summary>
        /// Retrieves the complete list of configuration items required in order to enroll a tenant to a service. This will
        /// include configuration entries required by any dependent services.
        /// </summary>
        /// <param name="serviceTenant">The service to gather configuration for.</param>
        /// <returns>
        /// A list of <see cref="ServiceManifestRequiredConfigurationEntry"/> representing the configuration requirements.
        /// </returns>
        Task<ServiceManifestRequiredConfigurationEntry[]> GetServiceEnrollmentConfigurationRequirementsAsync(ITenant serviceTenant);

        /// <summary>
        /// Enrolls the specified tenant in the service.
        /// </summary>
        /// <param name="enrollingTenant">The tenant to enroll.</param>
        /// <param name="serviceTenant">The service to enroll in.</param>
        /// <param name="configurationItems">Configuration for the enrollment.</param>
        /// <returns>A task which completes when the enrollment has finished.</returns>
        Task EnrollInServiceAsync(
            ITenant enrollingTenant,
            ITenant serviceTenant,
            EnrollmentConfigurationItem[] configurationItems);

        /// <summary>
        /// Add or updates arbitrary storage configuration for a tenant.
        /// </summary>
        /// <param name="tenant">The tenant to enroll.</param>
        /// <param name="configurationItems">Configuration to add.</param>
        /// <returns>A task which completes when the configuration has been added.</returns>
        Task AddOrUpdateStorageConfigurationAsync(ITenant tenant, ConfigurationItem[] configurationItems);

        /// <summary>
        /// Unenrolls the specified tenant from the service.
        /// </summary>
        /// <param name="enrolledTenant">The tenant that is currently enrolled.</param>
        /// <param name="serviceTenant">The service they need to be unenrolled from.</param>
        /// <returns>A task which completes when the unenrollment has finished.</returns>
        Task UnenrollFromServiceAsync(
            ITenant enrolledTenant,
            ITenant serviceTenant);

        /// <summary>
        /// Retrieves the tenant with the specified Id and ensures it is of the correct type.
        /// </summary>
        /// <param name="tenantId">The Id of the tenant to retrieve.</param>
        /// <param name="allowableTenantTypes">The list of valid types for the tenant.</param>
        /// <returns>The client tenant.</returns>
        /// <exception cref="TenantNotFoundException">There is no tenant with the specified Id.</exception>
        /// <exception cref="InvalidMarainTenantTypeException">The tenant Id provided is not for a tenant with the specified type.</exception>
        Task<ITenant> GetTenantOfTypeAsync(string tenantId, params MarainTenantType[] allowableTenantTypes);

        /// <summary>
        /// Gets the child tenants for a given tenant.
        /// </summary>
        /// <param name="tenantId">The id of the tenant for which to get the direct children.</param>
        /// <param name="limit">The maximum number of children to get in a single request.</param>
        /// <param name="continuationToken">A continuation token to continue reading the next batch.</param>
        /// <returns>The list of tenants who are children of that tenant, and a continuation token if there are more results.</returns>
        Task<(IList<ITenant> tenants, string? continuationToken)> GetChildrenAsync(string tenantId, int limit = 20, string? continuationToken = null);
    }
}
