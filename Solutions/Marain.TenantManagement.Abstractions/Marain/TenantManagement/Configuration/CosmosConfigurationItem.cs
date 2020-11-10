// <copyright file="CosmosConfigurationItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Configuration
{
    using System;
    using System.Collections.Generic;
    using Corvus.Azure.Cosmos.Tenancy;

    /// <summary>
    /// Enrollment configuration item for tenanted cosmos storage config.
    /// </summary>
    public class CosmosConfigurationItem : StorageConfigurationItem<CosmosContainerDefinition, CosmosConfiguration>
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
            if (this.Configuration.DatabaseName == null)
            {
                throw new NullReferenceException($"{nameof(this.Configuration.DatabaseName)} cannot be null.");
            }

            if (this.Configuration.ContainerName == null)
            {
                throw new NullReferenceException($"{nameof(this.Configuration.DatabaseName)} cannot be null.");
            }

            return values.AddCosmosConfiguration(
                new CosmosContainerDefinition(
                    this.Definition.DatabaseName,
                    this.Definition.ContainerName,
                    this.Definition.PartitionKeyPath),
                this.Configuration);
        }
    }
}
