// <copyright file="TenantPropertyKeys.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Internal
{
    /// <summary>
    /// Constants and helpers for well-known properties of tenants.
    /// </summary>
    public static class TenantPropertyKeys
    {
        /// <summary>
        /// Key for storing the Service Manifest on a Service Tenant.
        /// </summary>
        public const string ServiceManifest = "Marain:ServiceManifest";

        /// <summary>
        /// Key for storing a tenant's enrollments.
        /// </summary>
        public const string Enrollments = "Marain:Enrollments";

        /// <summary>
        /// Key for storing a tenant's type.
        /// </summary>
        public const string MarainTenantType = "Marain:TenantType";

        /// <summary>
        /// Builds the key to store the Id of a delegated service tenant that will be used by the specified service.
        /// </summary>
        /// <param name="serviceTenantName">
        /// The name of the Service Tenant representing the service that will be using the Delegated Tenant.
        /// </param>
        /// <returns>The key to use when storing the delegated tenant Id in the tenant's properties.</returns>
        public static string DelegatedTenantId(string serviceTenantName)
            => $"Marain:{serviceTenantName}:DelegatedTenantId";
    }
}