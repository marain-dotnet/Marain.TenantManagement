// <copyright file="TenantNames.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Internal
{
    /// <summary>
    /// Constants and helpers for tenant names.
    /// </summary>
    public static class TenantNames
    {
        /// <summary>
        /// The name of the parent tenant for all Client tenants.
        /// </summary>
        public const string ClientTenantParent = "Client Tenants";

        /// <summary>
        /// The name of the parent tenant for all Service tenants.
        /// </summary>
        public const string ServiceTenantParent = "Service Tenants";

        /// <summary>
        /// Builds a name for a delegated tenant.
        /// </summary>
        /// <param name="serviceTenantName">The service tenant for the service that will use the delegated tenant.</param>
        /// <param name="accessingTenantName">The tenant who the delegated tenant will be used on behalf of.</param>
        /// <returns>The name for the delegated tenant.</returns>
        public static string DelegatedTenant(string serviceTenantName, string accessingTenantName)
            => $"{serviceTenantName}\\{accessingTenantName}";
    }
}
