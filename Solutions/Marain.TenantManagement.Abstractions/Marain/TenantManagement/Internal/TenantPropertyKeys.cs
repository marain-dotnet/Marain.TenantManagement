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
    }
}
