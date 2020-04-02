// <copyright file="ServiceManifest.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using System.Collections.Generic;

    /// <summary>
    /// Manifest for a Marain service, needed when onboarding a new tenant to use that service.
    /// </summary>
    public class ServiceManifest
    {
        /// <summary>
        /// The content type of the manifest.
        /// </summary>
        public const string RegisteredContentType = "application/vnd.marain.tenancy.servicemanifests.servicemanifest";

        /// <summary>
        /// Gets the content type of the Service Manifest.
        /// </summary>
        public string ContentType => RegisteredContentType;

        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        public string? ServiceName { get; set; }

        /// <summary>
        /// Gets a list of the names of other Service Tenants whose services this depends upon.
        /// </summary>
        public IList<string> DependsOnServiceNames { get; } = new List<string>();

        /// <summary>
        /// Gets the list of configuration items required when enrolling a tenant to this service.
        /// </summary>
        public IList<ServiceManifestRequiredConfigurationEntry> RequiredConfigurationEntries { get; } = new List<ServiceManifestRequiredConfigurationEntry>();
    }
}
