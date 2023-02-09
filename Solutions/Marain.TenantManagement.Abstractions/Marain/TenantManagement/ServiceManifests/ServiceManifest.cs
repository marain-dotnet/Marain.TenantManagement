// <copyright file="ServiceManifest.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Corvus.Tenancy.Exceptions;

    /// <summary>
    /// Manifest for a Marain service, needed when onboarding a new tenant to use that service.
    /// </summary>
    /// <param name="WellKnownTenantGuid">
    /// The well known Guid that will be used when creating the service tenant.
    /// </param>
    /// <param name="ServiceName">
    /// The name of the service.
    /// </param>
    /// <param name="DependsOnServiceTenants">
    /// The list of other Service Tenants whose services this depends upon.
    /// </param>
    /// <param name="RequiredConfigurationEntries">
    /// The list of configuration items required when enrolling a tenant to this service.
    /// </param>
    public record ServiceManifest(
        Guid WellKnownTenantGuid,
        string ServiceName,
        IList<ServiceDependency>? DependsOnServiceTenants = null,
        IList<ServiceManifestRequiredConfigurationEntry>? RequiredConfigurationEntries = null)
    {
        /// <summary>
        /// The content type of the manifest.
        /// </summary>
        public const string RegisteredContentType = "application/vnd.marain.tenancy.servicemanifests.servicemanifest";

        /// <summary>
        /// Gets the content type of the Service Manifest.
        /// </summary>
#pragma warning disable CA1822 // Mark members as static - discovered by Content Handler via reflection, unfortunately
        public string ContentType => RegisteredContentType;
#pragma warning restore CA1822 // Mark members as static

        /// <summary>
        /// Gets a list of other Service Tenants whose services this depends upon.
        /// </summary>
        public IList<ServiceDependency> DependsOnServiceTenants { get; init; }
            = DependsOnServiceTenants ?? new List<ServiceDependency>();

        /// <summary>
        /// Gets the list of configuration items required when enrolling a tenant to this service.
        /// </summary>
        public IList<ServiceManifestRequiredConfigurationEntry> RequiredConfigurationEntries { get; init; }
            = RequiredConfigurationEntries ?? new List<ServiceManifestRequiredConfigurationEntry>();
    }
}