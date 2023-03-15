// <copyright file="ServiceManifestLegacyV2CosmosDbConfigurationEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests;

using System;
using System.Collections.Generic;

using Corvus.Tenancy;

using Marain.TenantManagement.Configuration;

/// <summary>
/// Service manifest configuration entry for CosmosDb.
/// </summary>
public class ServiceManifestLegacyV2CosmosDbConfigurationEntry : ServiceManifestRequiredLegacyConfigurationEntry
{
    /// <summary>
    /// The content type of the configuration entry.
    /// </summary>
    public const string RegisteredContentType = BaseContentType + "cosmosdb";

    private static readonly string[] ConfigurationItemContentTypes = { LegacyV2CosmosConfigurationItem.RegisteredContentType };

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

        if (enrollmentConfigurationItem is not LegacyV2CosmosConfigurationItem cosmosConfigurationItem)
        {
            throw new ArgumentException(
                $"The supplied value must be of type {nameof(LegacyV2CosmosConfigurationItem)}",
                nameof(enrollmentConfigurationItem));
        }

        return existingValues.Append(new KeyValuePair<string, object>(
            this.LegacyConfigurationEntryKey,
            cosmosConfigurationItem.Configuration));
    }

    /// <inheritdoc/>
    public override IEnumerable<string> GetPropertiesToRemoveFromTenant(ITenant tenant)
    {
        return new string[] { this.LegacyConfigurationEntryKey };
    }
}