// <copyright file="MarainTenantType.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement
{
    /// <summary>
    /// The different possible types of Marain tenants.
    /// </summary>
    public enum MarainTenantType
    {
        /// <summary>
        /// No tenant type has been specified.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// The tenant represents a client.
        /// </summary>
        /// <remarks>
        /// All service tenants should exist in a hierarchy starting under the well known "Client Tenants" parent.
        /// </remarks>
        Client = 1,

        /// <summary>
        /// The tenant represents a Marain service.
        /// </summary>
        /// <remarks>
        /// All service tenants should exist under the well known "Service Tenants" parent.
        /// </remarks>
        Service = 2,

        /// <summary>
        /// The tenant has been created to allow one service to call another on behalf of a client.
        /// </summary>
        /// <remarks>
        /// All delegated tenants should be children of the Service tenant that represents the service they will be used by.
        /// </remarks>
        Delegated = 3,
    }
}
