// <copyright file="ServiceManifestLegacyV2TableStorageTableDefinition.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
#nullable disable

namespace Marain.TenantManagement.ServiceManifests
{
    /// <summary>
    /// The container definition used to generate tenant property keys for Azure table storage configuration.
    /// </summary>
    public class ServiceManifestLegacyV2TableStorageTableDefinition
    {
        /// <summary>
        /// Gets or sets the logical table name.
        /// </summary>
        public string TableName { get; set; }
    }
}