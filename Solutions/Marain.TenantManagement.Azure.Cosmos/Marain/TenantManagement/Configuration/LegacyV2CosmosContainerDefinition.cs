// <copyright file="LegacyV2CosmosContainerDefinition.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Configuration;

/// <summary>
/// The structure of a definition used in legacy V2 tenancy to identify a logical Cosmos container.
/// </summary>
/// <param name="DatabaseName">The database name.</param>
/// <param name="ContainerName">he container name.</param>
/// <param name="PartitionKeyPath">The partition key path.</param>
/// <param name="ContainerThroughput">The container throughput, where container-level throughput is required.</param>
/// <param name="DatabaseThroughput">The database throughput, where database-level throughput is required.</param>
public record LegacyV2CosmosContainerDefinition(
    string DatabaseName,
    string ContainerName,
    string? PartitionKeyPath,
    int? ContainerThroughput,
    int? DatabaseThroughput)
{
    /// <summary>
    /// Returns the key used when storing Cosmos container configuration in tenant properties.
    /// </summary>
    /// <returns>The key to use in the tenant property bag.</returns>
    public string GetConfigurationKey() => $"StorageConfiguration__{this.DatabaseName}__{this.ContainerName}";
}