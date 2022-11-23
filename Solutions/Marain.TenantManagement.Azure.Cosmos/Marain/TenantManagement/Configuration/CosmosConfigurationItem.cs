// <copyright file="CosmosConfigurationItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Configuration;

using Corvus.Storage.Azure.Cosmos;

/// <summary>
/// Enrollment configuration item for tenanted cosmos storage config.
/// </summary>
/// <remarks>
/// TODO: We've got some naming inconsistencies - CosmosContainerConfiguration mentions the
/// Container, but CosmosConfigurationItem does not. There are similar inconsistencies for
/// blob and table storage.
/// </remarks>
public class CosmosConfigurationItem : StorageConfigurationItem<CosmosContainerConfiguration>
{
    /// <summary>
    /// The content type of the configuration item.
    /// </summary>
    public const string RegisteredContentType = ConfigurationItem.BaseContentType + "cosmosdb.v3";

    /// <inheritdoc/>
    public override string ContentType => RegisteredContentType;
}