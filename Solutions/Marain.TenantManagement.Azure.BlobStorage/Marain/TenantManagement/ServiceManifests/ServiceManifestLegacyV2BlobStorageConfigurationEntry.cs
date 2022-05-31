// <copyright file="ServiceManifestLegacyV2BlobStorageConfigurationEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests;

using System;
using System.Collections.Generic;

using Corvus.Tenancy;

using Marain.TenantManagement.Configuration;
using Marain.TenantManagement.EnrollmentConfiguration;

/// <summary>
/// Service manifest configuration entry for blob storage.
/// </summary>
public class ServiceManifestLegacyV2BlobStorageConfigurationEntry : ServiceManifestRequiredConfigurationEntry
{
    /// <summary>
    /// The content type of the configuration entry.
    /// </summary>
    public const string RegisteredContentType = BaseContentType + "azureblobstorage";

    /// <inheritdoc/>
    public override string ContentType => RegisteredContentType;

    /// <inheritdoc/>
    public override string ExpectedConfigurationItemContentType =>
        LegacyV2BlobStorageConfigurationItem.RegisteredContentType;

    /// <summary>
    /// Gets or sets the container definition that this configuration entry relates to.
    /// </summary>
#nullable disable annotations
    public LegacyV2BlobStorageContainerDefinition ContainerDefinition { get; set; }
#nullable restore annotations

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

        // Do we need to add a V3 version? I guess not since if this class comes into play it's
        // because the service's manifest declares only knowledge of V2 items.
        return existingValues.Append(new KeyValuePair<string, object>(
            this.ContainerDefinition.GetConfigurationKey(),
            blobStorageConfigurationItem.Configuration));
    }

    /// <inheritdoc/>
    public override IEnumerable<string> GetPropertiesToRemoveFromTenant(ITenant tenant)
    {
        return new string[] { this.ContainerDefinition.GetConfigurationKey() };
    }

    /// <inheritdoc/>
    public override IList<string> Validate(string messagePrefix)
    {
        IList<string> results = base.Validate(messagePrefix);

        if (this.ContainerDefinition == null)
        {
            results.Add($"{messagePrefix}: ContainerDefinition must be supplied for configuration entries with content type '{RegisteredContentType}'.");
        }
        else if (string.IsNullOrWhiteSpace(this.ContainerDefinition.ContainerName))
        {
            results.Add($"{messagePrefix}: ContainerDefinition.ContainerName must be supplied for configuration entries with content type '{RegisteredContentType}'.");
        }

        return results;
    }
}