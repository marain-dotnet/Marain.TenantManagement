// <copyright file="ServiceManifestCosmosDbConfigurationEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using System;
    using System.Collections.Generic;
    using Corvus.Storage.Azure.Cosmos;
    using Corvus.Storage.Azure.Cosmos.Tenancy;
    using Corvus.Tenancy;
    using Marain.TenantManagement.EnrollmentConfiguration;

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
            EnrollmentCosmosConfigurationItem.RegisteredContentType;

        ///// <summary>
        ///// Gets or sets the container definition that the service is expecting to use with this configuration.
        ///// </summary>
#nullable disable annotations
        ////public CosmosContainerDefinition ContainerDefinition { get; set; }
#nullable restore annotations

        /// <inheritdoc/>
        public override IEnumerable<KeyValuePair<string, object>> AddToTenantProperties(
            IEnumerable<KeyValuePair<string, object>> existingValues,
            EnrollmentConfigurationItem enrollmentConfigurationItem)
        {
            ArgumentNullException.ThrowIfNull(enrollmentConfigurationItem);

            if (enrollmentConfigurationItem is not EnrollmentCosmosConfigurationItem cosmosConfigurationItem)
            {
                throw new ArgumentException(
                    $"The supplied value must be of type {nameof(EnrollmentCosmosConfigurationItem)}",
                    nameof(enrollmentConfigurationItem));
            }

            return existingValues.AddCosmosConfiguration(this.Key, cosmosConfigurationItem.Configuration);
        }
    }
}