// <copyright file="ServiceManifestCosmosDbConfigurationEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using System;

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
        public string? DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the name of the container that the service is expecting to use with this configuration.
        /// </summary>
        public string? ContainerName { get; set; }
    }
}
