// <copyright file="ServiceManifestLegacyV2BlobStorageConfigurationEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests;

using System;
using System.Collections.Generic;
using Corvus.Storage.Azure.BlobStorage.Tenancy;
using Corvus.Tenancy;

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
public record ServiceManifestLegacyV2BlobStorageConfigurationEntry(
        string Key,
        string Description)
    : ServiceManifestRequiredLegacyConfigurationEntry<ServiceManifestLegacyV2BlobStorageContainerDefinition>(Key, Description)
{
    /// <summary>
    /// The content type of the configuration entry.
    /// </summary>
    public const string RegisteredContentType = BaseContentType + "azureblobstorage";

    private static readonly string[] ConfigurationItemContentTypes = { LegacyV2BlobStorageConfigurationItem.RegisteredContentType };

    /// <inheritdoc/>
    public override string ContentType => RegisteredContentType;

    /// <inheritdoc/>
    public override string[] ExpectedConfigurationItemContentTypes => ConfigurationItemContentTypes;

    /// <inheritdoc/>
    public override IEnumerable<KeyValuePair<string, object>> AddToTenantProperties(
        IEnumerable<KeyValuePair<string, object>> existingValues,
        ConfigurationItem enrollmentConfigurationItem)
    {
        ArgumentNullException.ThrowIfNull(enrollmentConfigurationItem);

        if (enrollmentConfigurationItem is not LegacyV2BlobStorageConfigurationItem blobStorageConfigurationItem)
        {
            throw new ArgumentException(
                $"The supplied value must be of type {nameof(LegacyV2BlobStorageConfigurationItem)}",
                nameof(enrollmentConfigurationItem));
        }

        return existingValues.Append(new KeyValuePair<string, object>(
            LegacyV2BlobConfigurationKeyNaming.TenantPropertyKeyForLogicalContainer(this.ContainerDefinition.ContainerName),
            blobStorageConfigurationItem.Configuration));
    }

    /// <inheritdoc/>
    public override IEnumerable<string> GetPropertiesToRemoveFromTenant(ITenant tenant)
    {
        return new string[] { LegacyV2BlobConfigurationKeyNaming.TenantPropertyKeyForLogicalContainer(this.ContainerDefinition.ContainerName) };
    }
}