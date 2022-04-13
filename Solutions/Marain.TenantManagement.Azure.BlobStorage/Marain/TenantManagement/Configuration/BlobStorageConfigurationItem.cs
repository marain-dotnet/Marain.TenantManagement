// <copyright file="BlobStorageConfigurationItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Configuration;

using System.Collections.Generic;

using Corvus.Storage.Azure.BlobStorage;
using Corvus.Storage.Azure.BlobStorage.Tenancy;

/// <summary>
/// Enrollment configuration item for tenanted blob storage config.
/// </summary>
public class BlobStorageConfigurationItem : StorageConfigurationItem<BlobContainerConfiguration>
{
    /// <summary>
    /// The content type of the configuration item.
    /// </summary>
    public const string RegisteredContentType = ConfigurationItem.BaseContentType + "azureblobstorage";

    /// <inheritdoc/>
    public override string ContentType => RegisteredContentType;

    /// <inheritdoc/>
    public override IEnumerable<KeyValuePair<string, object>> AddConfiguration(IEnumerable<KeyValuePair<string, object>> values)
    {
        return values.AddBlobStorageConfiguration(this.ConfigurationKey, this.Configuration);
    }
}