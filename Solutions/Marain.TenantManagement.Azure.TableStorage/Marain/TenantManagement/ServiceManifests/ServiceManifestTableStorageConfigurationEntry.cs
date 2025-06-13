// <copyright file="ServiceManifestTableStorageConfigurationEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using Corvus.Storage.Azure.TableStorage;
    using Corvus.Storage.Azure.TableStorage.Tenancy;

    using Marain.TenantManagement.Configuration;

    /// <summary>
    /// Service manifest configuration entry for table storage.
    /// </summary>
    /// <param name="Key">
    /// The configuration entry key. This is used to match configuration supplied as part of
    /// enrollment with the configuration entry it relates to.
    /// </param>
    /// <param name="Description">
    /// The description of the configuration entry.
    /// </param>
    public record ServiceManifestTableStorageConfigurationEntry(
        string Key,
        string Description) :
        ServiceManifestRequiredConfigurationEntryWithV2LegacySupport<
            TableConfiguration, TableConfigurationItem, LegacyV2TableConfiguration, LegacyV2TableStorageConfigurationItem>(Key, Description)
    {
        /// <summary>
        /// The content type of the configuration entry.
        /// </summary>
        public const string RegisteredContentType = BaseContentType + "azuretablestorage.v3";

        private static readonly string[] ConfigurationItemContentType = { TableConfigurationItem.RegisteredContentType };
        private static readonly string[] ConfigurationItemContentTypesWithLegacySupport =
        {
            TableConfigurationItem.RegisteredContentType,
            LegacyV2TableStorageConfigurationItem.RegisteredContentType,
        };

        /// <inheritdoc/>
        public override string ContentType => RegisteredContentType;

        /// <inheritdoc/>
        public override string[] ExpectedConfigurationItemContentTypes => this.LegacyV2Key is null
            ? ConfigurationItemContentType
            : ConfigurationItemContentTypesWithLegacySupport;
    }
}