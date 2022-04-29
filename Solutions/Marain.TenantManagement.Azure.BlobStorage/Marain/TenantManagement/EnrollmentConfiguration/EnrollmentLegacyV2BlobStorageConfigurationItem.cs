// <copyright file="EnrollmentLegacyV2BlobStorageConfigurationItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.EnrollmentConfiguration;

using Corvus.Storage.Azure.BlobStorage.Tenancy;

/// <summary>
/// Enrollment configuration item for tenanted blob storage config.
/// </summary>
public class EnrollmentLegacyV2BlobStorageConfigurationItem : EnrollmentStorageConfigurationItem<LegacyV2BlobStorageConfiguration>
{
    /// <summary>
    /// The content type of the configuration item.
    /// </summary>
    public const string RegisteredContentType = EnrollmentConfigurationItem.BaseContentType + "azureblobstorage";

    /// <inheritdoc/>
    public override string ContentType => RegisteredContentType;
}