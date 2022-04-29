// <copyright file="LegacyV2BlobStorageContainerDefinition.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Configuration;

using Corvus.Storage.Azure.BlobStorage.Tenancy;

/// <summary>
/// The structure of a definition used in legacy V2 tenancy to identify a logical blob storage container.
/// </summary>
/// <param name="ContainerName">The container name.</param>
/// <param name="AccessType">The access type for the container.</param>
public record LegacyV2BlobStorageContainerDefinition(
    string ContainerName,
    LegacyV2BlobContainerPublicAccessType AccessType = LegacyV2BlobContainerPublicAccessType.Off)
{
    /// <summary>
    /// Returns the key used when storing blob container configuration in tenant properties.
    /// </summary>
    /// <returns>The key to use in the tenant property bag.</returns>
    public string GetConfigurationKey() => $"StorageConfiguration__{this.ContainerName}";
}