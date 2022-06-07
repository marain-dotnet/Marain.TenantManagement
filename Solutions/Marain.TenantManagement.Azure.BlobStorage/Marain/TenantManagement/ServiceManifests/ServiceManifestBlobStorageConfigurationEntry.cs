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
    public class ServiceManifestBlobStorageConfigurationEntry :
        ServiceManifestRequiredConfigurationEntryWithV2LegacySupport<
            BlobContainerConfiguration, BlobStorageConfigurationItem, LegacyV2BlobStorageConfiguration, LegacyV2BlobStorageConfigurationItem>
    {
        /// <summary>
        /// The content type of the configuration entry.
        /// </summary>
        public const string RegisteredContentType = BaseContentType + "azureblobstorage.v3";

        private static readonly string[] ConfigurationItemContentType = { BlobStorageConfigurationItem.RegisteredContentType };
        private static readonly string[] ConfigurationItemContentTypesWithLegacySupport =
        {
            BlobStorageConfigurationItem.RegisteredContentType,
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