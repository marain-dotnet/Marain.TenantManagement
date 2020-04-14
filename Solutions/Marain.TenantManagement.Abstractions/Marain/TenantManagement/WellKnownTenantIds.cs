// <copyright file="WellKnownTenantIds.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement
{
    using System;
    using Corvus.Tenancy;

    /// <summary>
    /// Ids for well known tenants in the Marain tenancy hierarchy.
    /// </summary>
    public static class WellKnownTenantIds
    {
        /// <summary>
        /// Gets the Id of the client tenant parent.
        /// </summary>
        /// <remarks>This evaluates to "75b9261673c2714681f14c97bc0439fb".</remarks>
        public static string ClientTenantParentId { get; } = RootTenant.RootTenantId.CreateChildId(ClientTenantParentGuid);

        /// <summary>
        /// Gets the Id of the service tenant parent.
        /// </summary>
        /// <remarks>This evaluates to "3633754ac4c9be44b55bfe791b1780f1".</remarks>
        public static string ServiceTenantParentId { get; } = RootTenant.RootTenantId.CreateChildId(ServiceTenantParentGuid);

        /// <summary>
        /// Gets the Guid to use when creating the client tenant parent.
        /// </summary>
        internal static Guid ClientTenantParentGuid => Guid.Parse("1626b975-c273-4671-81f1-4c97bc0439fb");

        /// <summary>
        /// Gets the Guid to use when creating the service tenant parent.
        /// </summary>
        internal static Guid ServiceTenantParentGuid => Guid.Parse("4a753336-c9c4-44be-b55b-fe791b1780f1");
    }
}
