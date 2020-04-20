// <copyright file="ServiceManifestCosmosDbConfigurationEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using System;
    using System.Collections.Generic;
    using Corvus.Azure.Cosmos.Tenancy;
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
        public const string RegisteredContentType = BaseContentType + "cosmosdb";

        /// <inheritdoc/>
        public override string ContentType => RegisteredContentType;

        /// <inheritdoc/>
        public override string ExpectedConfigurationItemContentType =>
            EnrollmentCosmosConfigurationItem.RegisteredContentType;

        /// <summary>
        /// Gets or sets the container definition that the service is expecting to use with this configuration.
        /// </summary>
#nullable disable annotations
        public CosmosContainerDefinition ContainerDefinition { get; set; }
#nullable restore annotations

        /// <inheritdoc/>
        public override void AddToTenant(ITenant tenant, EnrollmentConfigurationItem enrollmentConfigurationItem)
        {
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }

            if (enrollmentConfigurationItem == null)
            {
                throw new ArgumentNullException(nameof(enrollmentConfigurationItem));
            }

            if (!(enrollmentConfigurationItem is EnrollmentCosmosConfigurationItem cosmosConfigurationItem))
            {
                throw new ArgumentException(
                    $"The supplied value must be of type {nameof(EnrollmentCosmosConfigurationItem)}",
                    nameof(enrollmentConfigurationItem));
            }

            tenant.SetCosmosConfiguration(this.ContainerDefinition, cosmosConfigurationItem.Configuration);
        }

        /// <inheritdoc/>
        public override void RemoveFromTenant(ITenant tenant)
        {
            tenant.ClearCosmosConfiguration(this.ContainerDefinition);
        }

        /// <inheritdoc/>
        public override IList<string> Validate(string messagePrefix)
        {
            if (string.IsNullOrEmpty(messagePrefix))
            {
                throw new ArgumentException(nameof(messagePrefix));
            }

            var results = new List<string>();
            results.AddRange(base.Validate(messagePrefix));

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
}
