// <copyright file="CosmosConfigurationItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Configuration
{
    using Corvus.Azure.Cosmos.Tenancy;

    /// <summary>
    /// Enrollment configuration item for tenanted cosmos storage config.
    /// </summary>
    public class CosmosConfigurationItem : StorageConfigurationItem<CosmosConfiguration>
    {
        /// <summary>
        /// The content type of the configuration item.
        /// </summary>
        public const string RegisteredContentType = ConfigurationItem.BaseContentType + "cosmosdb";

        /// <inheritdoc/>
        public override string ContentType => RegisteredContentType;
    }
}
