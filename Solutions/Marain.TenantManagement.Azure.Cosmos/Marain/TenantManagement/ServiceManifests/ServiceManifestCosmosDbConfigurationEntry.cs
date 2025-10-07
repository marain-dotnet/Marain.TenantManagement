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
    /// <param name="Key">
    /// The configuration entry key. This is used to match configuration supplied as part of
    /// enrollment with the configuration entry it relates to.
    /// </param>
    /// <param name="Description">
    /// The description of the configuration entry.
    /// </param>
    public record ServiceManifestCosmosDbConfigurationEntry(
        string Key,
        string Description) :
        ServiceManifestRequiredConfigurationEntryWithV2LegacySupport<
            CosmosContainerConfiguration, CosmosContainerConfigurationItem, LegacyV2CosmosContainerConfiguration, LegacyV2CosmosConfigurationItem>(
            Key, Description)
    {
        /// <summary>
        /// The content type of the configuration entry.
        /// </summary>
        public const string RegisteredContentType = BaseContentType + "cosmosdb.v3";

        private static readonly string[] ConfigurationItemContentType = { CosmosContainerConfigurationItem.RegisteredContentType };
        private static readonly string[] ConfigurationItemContentTypesWithLegacySupport =
        {
            CosmosContainerConfigurationItem.RegisteredContentType,
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