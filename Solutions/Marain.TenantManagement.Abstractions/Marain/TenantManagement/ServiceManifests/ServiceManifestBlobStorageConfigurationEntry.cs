// <copyright file="ServiceManifestBlobStorageConfigurationEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Service manifest configuration entry for blob storage.
    /// </summary>
    public class ServiceManifestBlobStorageConfigurationEntry : ServiceManifestRequiredConfigurationEntry
    {
        /// <summary>
        /// The content type of the configuration entry.
        /// </summary>
        public const string RegisteredContentType = BaseContentType + "azureblobstorage";

        /// <inheritdoc/>
        public override string ContentType => RegisteredContentType;

        /// <summary>
        /// Gets or sets the name of the container that the service is expecting to use with this configuration.
        /// </summary>
#nullable disable annotations
        public string ContainerName { get; set; }
#nullable restore annotations

        /// <inheritdoc/>
        public override IList<string> Validate(string messagePrefix)
        {
            var results = new List<string>();
            results.AddRange(base.Validate(messagePrefix));

            if (string.IsNullOrWhiteSpace(this.ContainerName))
            {
                results.Add($"{messagePrefix}: ContainerName must be supplied for configuration entries with content type '{RegisteredContentType}'.");
            }

            return results;
        }
    }
}
