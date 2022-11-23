// <copyright file="LegacyV2BlobStorageConfigurationItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Configuration;

using Corvus.Storage.Azure.BlobStorage.Tenancy;

/// <summary>
/// Enrollment configuration item for tenanted blob storage config.
/// </summary>
public class LegacyV2BlobStorageConfigurationItem : ConfigurationItem<LegacyV2BlobStorageConfiguration>
{
    /// <summary>
    /// The content type of the configuration item.
    /// </summary>
    public const string RegisteredContentType = ConfigurationItem.BaseContentType + "azureblobstorage";

    /// <inheritdoc/>
    public override string ContentType => RegisteredContentType;
}