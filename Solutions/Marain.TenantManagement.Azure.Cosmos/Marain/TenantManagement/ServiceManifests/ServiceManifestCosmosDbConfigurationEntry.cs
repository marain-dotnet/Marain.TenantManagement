// <copyright file="ServiceManifestCosmosDbConfigurationEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using Corvus.Storage.Azure.Cosmos;
    using Corvus.Storage.Azure.Cosmos.Tenancy;

    using Marain.TenantManagement.Configuration;

    /// <summary>
    /// Service manifest configuration entry for CosmosDb.
    /// </summary>
    public class ServiceManifestCosmosDbConfigurationEntry :
        ServiceManifestRequiredConfigurationEntryWithV2LegacySupport<
            CosmosContainerConfiguration, CosmosConfigurationItem, LegacyV2CosmosContainerConfiguration, LegacyV2CosmosConfigurationItem>
    {
        /// <summary>
        /// The content type of the configuration entry.
        /// </summary>
        public const string RegisteredContentType = BaseContentType + "cosmosdb.v3";

        private static readonly string[] ConfigurationItemContentType = { CosmosConfigurationItem.RegisteredContentType };
        private static readonly string[] ConfigurationItemContentTypesWithLegacySupport =
        {
            CosmosConfigurationItem.RegisteredContentType,
            LegacyV2CosmosConfigurationItem.RegisteredContentType,
        };

        /// <inheritdoc/>
        public override string ContentType => RegisteredContentType;

        /// <inheritdoc/>
        public override string[] ExpectedConfigurationItemContentTypes => this.LegacyV2Key is null
            ? ConfigurationItemContentType
            : ConfigurationItemContentTypesWithLegacySupport;
    }
}