// <copyright file="ServiceManifestCosmosDbConfigurationEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Service manifest configuration entry for CosmosDb.
    /// </summary>
    public class ServiceManifestCosmosDbConfigurationEntry : ServiceManifestRequiredConfigurationEntry
    {
        /// <summary>
        /// The content type of the configuration entry.
        /// </summary>
        public const string RegisteredContentType = BaseContentType + "cosmosdbconfigurationentry";

        /// <inheritdoc/>
        public override string ContentType => RegisteredContentType;

        /// <summary>
        /// Gets or sets the name of the database that the service is expecting to use with this configuration.
        /// </summary>
#nullable disable annotations
        public string DatabaseName { get; set; }
#nullable restore annotations

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

            if (string.IsNullOrWhiteSpace(this.Description))
            {
                results.Add($"{messagePrefix}: DatabaseName must be supplied for configuration entries with content type '{RegisteredContentType}'.");
            }

            if (string.IsNullOrWhiteSpace(this.ContainerName))
            {
                results.Add($"{messagePrefix}: ContainerName must be supplied for configuration entries with content type '{RegisteredContentType}'.");
            }

            return results;
        }
    }
}
