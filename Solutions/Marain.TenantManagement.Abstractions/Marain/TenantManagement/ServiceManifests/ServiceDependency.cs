// <copyright file="ServiceDependency.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    /// <summary>
    /// Represents a dependency of a service on another service.
    /// </summary>
    /// <param name="Id">The Id of the dependency's Service Tenant.</param>
    /// <param name="ExpectedName">
    /// The expected name of the dependency's Service Tenant. This is not mandatory, but if present
    /// it is expected to match the name of the tenant whose Id is specified in the <see cref="Id"/>
    /// property.
    /// </param>
    public record ServiceDependency(
        string Id,
        string? ExpectedName = null);
}