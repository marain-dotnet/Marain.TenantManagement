// <copyright file="ServiceManifestRequiredConfigurationEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using System;

    /// <summary>
    /// Base class for the different supported types of configuration entry.
    /// </summary>
    public abstract class ServiceManifestRequiredConfigurationEntry
    {
        /// <summary>
        /// Base content type for configuration entries.
        /// </summary>
        public const string BaseContentType = "application/vnd.marain.tenancy.servicemanifests.";

        /// <summary>
        /// Gets the content type for the configuration entry.
        /// </summary>
        public abstract string ContentType { get; }

        /// <summary>
        /// Gets the description of the configuration entry.
        /// </summary>
        public string? Description { get; }
    }
}
