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
public class ServiceManifestLegacyV2CosmosDbConfigurationEntry : ServiceManifestRequiredConfigurationEntry
{
    /// <summary>
    /// The content type of the configuration entry.
    /// </summary>
    public const string RegisteredContentType = BaseContentType + "cosmosdb";

    /// <inheritdoc/>
    public override string ContentType => RegisteredContentType;

    /// <inheritdoc/>
    public override string ExpectedConfigurationItemContentType =>
        CosmosConfigurationItem.RegisteredContentType;

    /// <summary>
    /// Gets or sets the container definition that the service is expecting to use with this configuration.
    /// </summary>
#nullable disable annotations
    public LegacyV2CosmosContainerDefinition ContainerDefinition { get; set; }
#nullable restore annotations

    /// <summary>
    /// Gets or sets a value indicating whether this service supports legacy V2 style
    /// configuration entries to enable V2 to V3 migration.
    /// </summary>
    public bool SupportsLegacyV2Configuration { get; set; }

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
            this.ContainerDefinition.GetConfigurationKey(),
            cosmosConfigurationItem.Configuration));
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

        if (string.IsNullOrWhiteSpace(this.ContainerDefinition?.DatabaseName))
        {
            results.Add($"{messagePrefix}: ContainerDefinition.DatabaseName must be supplied for configuration entries with content type '{RegisteredContentType}'.");
        }

        if (string.IsNullOrWhiteSpace(this.ContainerDefinition?.ContainerName))
        {
            results.Add($"{messagePrefix}: ContainerDefinition.ContainerName must be supplied for configuration entries with content type '{RegisteredContentType}'.");
        }

        return results;
    }
}