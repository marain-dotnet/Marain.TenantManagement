// <copyright file="ITenantManagementService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Corvus.Tenancy.Exceptions;
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
        /// Creates a new tenant representing a client.
        /// </summary>
        /// <param name="clientName">The name of the new tenant.</param>
        /// <returns>The new tenant.</returns>
        Task<ITenant> CreateClientTenantAsync(string clientName);

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
    }
}
