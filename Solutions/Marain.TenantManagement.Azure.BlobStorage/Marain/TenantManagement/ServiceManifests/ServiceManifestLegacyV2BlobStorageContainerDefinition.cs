// <copyright file="ServiceManifestLegacyV2BlobStorageContainerDefinition.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
#nullable disable

namespace Marain.TenantManagement.ServiceManifests
{
    /// <summary>
    /// The container definition used to generate tenant property keys for Azure blob storage configuration.
    /// </summary>
    public class ServiceManifestLegacyV2BlobStorageContainerDefinition
    {
        /// <summary>
        /// Gets or sets the logical container name.
        /// </summary>
        public string ContainerName { get; set; }
    }
}