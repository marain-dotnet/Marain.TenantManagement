// <copyright file="ServiceManifestBlobStorageConfigurationEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using Corvus.Storage.Azure.BlobStorage;
    using Corvus.Storage.Azure.BlobStorage.Tenancy;

    using Marain.TenantManagement.Configuration;

    /// <summary>
    /// Service manifest configuration entry for blob storage.
    /// </summary>
    /// <param name="Key">
    /// The configuration entry key. This is used to match configuration supplied as part of
    /// enrollment with the configuration entry it relates to.
    /// </param>
    /// <param name="Description">
    /// The description of the configuration entry.
    /// </param>
    public record ServiceManifestBlobStorageConfigurationEntry(
        string Key,
        string Description) :
        ServiceManifestRequiredConfigurationEntryWithV2LegacySupport<
            BlobContainerConfiguration, BlobContainerConfigurationItem, LegacyV2BlobStorageConfiguration, LegacyV2BlobStorageConfigurationItem>(
            Key, Description)
    {
        /// <summary>
        /// The content type of the configuration entry.
        /// </summary>
        public const string RegisteredContentType = BaseContentType + "azureblobstorage.v3";

        private static readonly string[] ConfigurationItemContentType = { BlobContainerConfigurationItem.RegisteredContentType };
        private static readonly string[] ConfigurationItemContentTypesWithLegacySupport =
        {
            BlobContainerConfigurationItem.RegisteredContentType,
            LegacyV2BlobStorageConfigurationItem.RegisteredContentType,
        };

        /// <inheritdoc/>
        public override string ContentType => RegisteredContentType;

        /// <inheritdoc/>
        public override string[] ExpectedConfigurationItemContentTypes => this.LegacyV2Key is null
            ? ConfigurationItemContentType
            : ConfigurationItemContentTypesWithLegacySupport;
    }
}