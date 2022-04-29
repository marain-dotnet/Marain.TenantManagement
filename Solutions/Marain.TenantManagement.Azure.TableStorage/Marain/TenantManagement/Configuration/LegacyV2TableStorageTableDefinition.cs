// <copyright file="LegacyV2TableStorageTableDefinition.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Configuration;

/// <summary>
/// The structure of a definition used in legacy V2 tenancy to identify a logical table.
/// </summary>
/// <param name="TableName">The table name.</param>
public record LegacyV2TableStorageTableDefinition(string TableName)
{
    /// <summary>
    /// Returns the key used when storing blob container configuration in tenant properties.
    /// </summary>
    /// <returns>The key to use in the tenant property bag.</returns>
    public string GetConfigurationKey() => $"StorageConfiguration__Table__{this.TableName}";
}