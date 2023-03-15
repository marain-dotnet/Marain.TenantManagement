// <copyright file="ServiceManifestLegacyV2TableStorageConfigurationEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests;

using System;
using System.Collections.Generic;

using Corvus.Tenancy;

using Marain.TenantManagement.Configuration;

/// <summary>
/// Service manifest configuration entry for table storage.
/// </summary>
public class ServiceManifestLegacyV2TableStorageConfigurationEntry : ServiceManifestRequiredLegacyConfigurationEntry
{
    /// <summary>
    /// The content type of the configuration entry.
    /// </summary>
    public const string RegisteredContentType = BaseContentType + "azuretablestorage";

    private static readonly string[] ConfigurationItemContentTypes = { LegacyV2TableStorageConfigurationItem.RegisteredContentType };

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

        if (enrollmentConfigurationItem is not LegacyV2TableStorageConfigurationItem tableStorageConfigurationItem)
        {
            throw new ArgumentException(
                $"The supplied value must be of type {nameof(LegacyV2TableStorageConfigurationItem)}",
                nameof(enrollmentConfigurationItem));
        }

        return existingValues.Append(new KeyValuePair<string, object>(
            this.LegacyConfigurationEntryKey,
            tableStorageConfigurationItem.Configuration));
    }

    /// <inheritdoc/>
    public override IEnumerable<string> GetPropertiesToRemoveFromTenant(ITenant tenant)
    {
        return new string[] { this.LegacyConfigurationEntryKey };
    }
}