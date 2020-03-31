// <copyright file="TenantExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement
{
    using System;
    using Corvus.Tenancy;
    using Marain.TenantManagement.Internal;

    /// <summary>
    /// Extension methods for <see cref="ITenant"/>s.
    /// </summary>
    public static class TenantExtensions
    {
        /// <summary>
        /// Returns true if the tenant is a Service tenant, otherwise returns false.
        /// </summary>
        /// <param name="tenant">The tenant to check.</param>
        /// <returns>A value indicating whether or not the tenant is a Service tenant.</returns>
        public static bool IsServiceTenant(this ITenant tenant)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if the tenant is a Client tenant, otherwise returns false.
        /// </summary>
        /// <param name="tenant">The tenant to check.</param>
        /// <returns>A value indicating whether or not the tenant is a Client tenant.</returns>
        public static bool IsClientTenant(this ITenant tenant)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds the given <see cref="ServiceManifest"/> to the tenant's property bag.
        /// </summary>
        /// <param name="tenant">The tenant to add to.</param>
        /// <param name="manifest">The manifest to add.</param>
        public static void SetServiceManifest(this ITenant tenant, ServiceManifest manifest)
        {
            tenant.Properties.Set(TenantPropertyKeys.ServiceManifest, manifest);
        }
    }
}
