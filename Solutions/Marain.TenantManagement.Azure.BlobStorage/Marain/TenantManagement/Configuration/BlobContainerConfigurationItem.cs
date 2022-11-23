// <copyright file="BlobContainerConfigurationItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Configuration;

using Corvus.Storage.Azure.BlobStorage;

/// <summary>
/// Enrollment configuration item for tenanted blob storage config.
/// </summary>
public class BlobContainerConfigurationItem : ConfigurationItem<BlobContainerConfiguration>
{
    /// <summary>
    /// The content type of the configuration item.
    /// </summary>
    public const string RegisteredContentType = BaseContentType + "azureblobstorage.v3";

    /// <inheritdoc/>
    public override string ContentType => RegisteredContentType;
}