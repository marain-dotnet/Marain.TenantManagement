// <copyright file="CosmosContainerConfigurationItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Configuration;

using Corvus.Storage.Azure.Cosmos;

/// <summary>
/// Enrollment configuration item for tenanted cosmos storage config.
/// </summary>
public class CosmosContainerConfigurationItem : ConfigurationItem<CosmosContainerConfiguration>
{
    /// <summary>
    /// The content type of the configuration item.
    /// </summary>
    public const string RegisteredContentType = BaseContentType + "cosmosdb.v3";

    /// <inheritdoc/>
    public override string ContentType => RegisteredContentType;
}