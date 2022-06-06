﻿// <copyright file="ServiceManifestTableStorageConfigurationEntry.cs" company="Endjin Limited">
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
    public class ServiceManifestTableStorageConfigurationEntry :
        ServiceManifestRequiredConfigurationEntryWithV2LegacySupport<
            TableConfiguration, TableStorageConfigurationItem, LegacyV2TableConfiguration, LegacyV2TableStorageConfigurationItem>
    {
        /// <summary>
        /// The content type of the configuration entry.
        /// </summary>
        public const string RegisteredContentType = BaseContentType + "azuretablestorage.v3";

        /// <inheritdoc/>
        public override string ContentType => RegisteredContentType;

        /// <inheritdoc/>
        public override string ExpectedConfigurationItemContentType =>
            TableStorageConfigurationItem.RegisteredContentType;
    }
}