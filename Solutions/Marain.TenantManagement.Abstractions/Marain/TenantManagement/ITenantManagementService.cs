// <copyright file="ITenantManagementService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Tenancy;

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
        /// <param name="serviceName">The name of the service. This must be unique across all Service tenants.</param>
        /// <returns>The new tenant.</returns>
        Task<ITenant> CreateServiceTenantAsync(string serviceName);

        /// <summary>
        /// Retrieves the Service tenant with the given name.
        /// </summary>
        /// <param name="serviceName">The name of the tenant to retrieve.</param>
        /// <returns>The tenant, or null if it does not exist.</returns>
        Task<ITenant> GetServiceTenantByNameAsync(string serviceName);
    }
}
