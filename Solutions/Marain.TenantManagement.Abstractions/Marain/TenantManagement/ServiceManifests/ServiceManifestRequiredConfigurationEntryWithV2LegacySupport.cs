// <copyright file="ServiceManifestRequiredConfigurationEntryWithV2LegacySupport.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests;

using System;
using System.Collections.Generic;
using System.Linq;

using Marain.TenantManagement.Configuration;

/// <summary>
/// Base class of required configuration entry definitions where fallback V2 legacy
/// configuration is supported.
/// </summary>
/// <typeparam name="TConfiguration">
/// The configuration type when not using legacy configuration.
/// </typeparam>
/// <typeparam name="TConfigurationItem">
/// The configuration item type when not using legacy configuration.
/// </typeparam>
/// <typeparam name="TV2LegacyConfiguration">
/// The V2 legacy configuration type.
/// </typeparam>
/// <typeparam name="TV2LegacyConfigurationItem">
/// The V2 legacy configuration item type.
/// </typeparam>
public abstract class ServiceManifestRequiredConfigurationEntryWithV2LegacySupport<
    TConfiguration,
    TConfigurationItem,
    TV2LegacyConfiguration,
    TV2LegacyConfigurationItem>
    : ServiceManifestRequiredConfigurationEntry
    where TConfiguration : class
    where TConfigurationItem : ConfigurationItem<TConfiguration>
    where TV2LegacyConfiguration : class
    where TV2LegacyConfigurationItem : ConfigurationItem<TV2LegacyConfiguration>
{
    /// <summary>
    /// Gets or sets the key under which this service supports legacy V2 style configuration
    /// entries to enable V2 to V3 migration, or null if the service does not support this.
    /// </summary>
    public string? LegacyV2Key { get; set; }

    /// <inheritdoc/>
    public override IEnumerable<KeyValuePair<string, object>> AddToTenantProperties(
        IEnumerable<KeyValuePair<string, object>> existingValues,
        ConfigurationItem enrollmentConfigurationItem)
    {
        ArgumentNullException.ThrowIfNull(enrollmentConfigurationItem);

        if (enrollmentConfigurationItem is not TConfigurationItem tableStorageConfigurationItem)
        {
            if (this.LegacyV2Key is string legacyV2Key &&
                enrollmentConfigurationItem is TV2LegacyConfigurationItem v2TableStorageConfigurationItem)
            {
                return existingValues.Append(new KeyValuePair<string, object>(
                    legacyV2Key,
                    v2TableStorageConfigurationItem.Configuration));
            }

            throw new ArgumentException(
                $"The supplied value must be of type {typeof(TConfigurationItem).Name} or {typeof(TV2LegacyConfigurationItem)}",
                nameof(enrollmentConfigurationItem));
        }

        return existingValues.Append(new KeyValuePair<string, object>(
            this.Key,
            tableStorageConfigurationItem.Configuration));
    }
}