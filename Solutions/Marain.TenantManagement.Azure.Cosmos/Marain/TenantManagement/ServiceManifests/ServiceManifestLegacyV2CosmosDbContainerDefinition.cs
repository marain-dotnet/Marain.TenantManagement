// <copyright file="ServiceManifestLegacyV2CosmosDbContainerDefinition.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
#nullable disable

namespace Marain.TenantManagement.ServiceManifests
{
    /// <summary>
    /// The container definition used to generate tenant property keys for Cosmos storage configuration.
    /// </summary>
    public class ServiceManifestLegacyV2CosmosDbContainerDefinition
    {
        /// <summary>
        /// Gets or sets the logical database name.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the logical container name.
        /// </summary>
        public string ContainerName { get; set; }
    }
}