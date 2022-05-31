// <copyright file="ServiceManifestCosmosDbConfigurationEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using System;
    using System.Collections.Generic;

    using Corvus.Storage.Azure.Cosmos.Tenancy;

    using Marain.TenantManagement.Configuration;

    /// <summary>
    /// Service manifest configuration entry for CosmosDb.
    /// </summary>
    public class ServiceManifestCosmosDbConfigurationEntry : ServiceManifestRequiredConfigurationEntry
    {
        /// <summary>
        /// The content type of the configuration entry.
        /// </summary>
        public const string RegisteredContentType = BaseContentType + "cosmosdb.v3";

        /// <inheritdoc/>
        public override string ContentType => RegisteredContentType;

        /// <inheritdoc/>
        public override string ExpectedConfigurationItemContentType =>
            CosmosConfigurationItem.RegisteredContentType;

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

            if (enrollmentConfigurationItem is not CosmosConfigurationItem cosmosConfigurationItem)
            {
                throw new ArgumentException(
                    $"The supplied value must be of type {nameof(CosmosConfigurationItem)}",
                    nameof(enrollmentConfigurationItem));
            }

            return existingValues.AddCosmosConfiguration(this.Key, cosmosConfigurationItem.Configuration);
        }
    }
}