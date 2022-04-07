// <copyright file="CosmosConfigurationItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Configuration;

using System.Collections.Generic;

using Corvus.Storage.Azure.Cosmos;

/// <summary>
/// Enrollment configuration item for tenanted cosmos storage config.
/// </summary>
public class CosmosConfigurationItem : StorageConfigurationItem<CosmosContainerConfiguration>
{
    /// <summary>
    /// The content type of the configuration item.
    /// </summary>
    public const string RegisteredContentType = ConfigurationItem.BaseContentType + "cosmosdb";

    /// <inheritdoc/>
    public override string ContentType => RegisteredContentType;

    /// <inheritdoc/>
    public override IEnumerable<KeyValuePair<string, object>> AddConfiguration(IEnumerable<KeyValuePair<string, object>> values)
    {
        return values.AddCosmosConfiguration(
            new CosmosContainerDefinition(
                this.Definition.DatabaseName,
                this.Definition.ContainerName,
                this.Definition.PartitionKeyPath),
            this.Configuration);
    }
}
