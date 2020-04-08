// <copyright file="EnrollmentCosmosConfigurationItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.EnrollmentConfiguration
{
    using System;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.Tenancy;
    using Marain.TenantManagement.ServiceManifests;

    /// <summary>
    /// Enrollment configuration item for tenanted cosmos storage config.
    /// </summary>
    public class EnrollmentCosmosConfigurationItem : EnrollmentStorageConfigurationItem<CosmosConfiguration>
    {
        /// <summary>
        /// The content type of the configuration item.
        /// </summary>
        public const string RegisteredContentType = EnrollmentConfigurationItem.BaseContentType + "cosmosdb";

        /// <inheritdoc/>
        public override string ContentType => RegisteredContentType;

        /// <inheritdoc/>
        public override void AddToTenant(ITenant tenant, ServiceManifestRequiredConfigurationEntry requiredConfigurationEntry)
        {
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }

            if (requiredConfigurationEntry == null)
            {
                throw new ArgumentNullException(nameof(requiredConfigurationEntry));
            }

            if (!(requiredConfigurationEntry is ServiceManifestCosmosDbConfigurationEntry cosmosConfigurationEntry))
            {
                throw new ArgumentException(
                    $"The supplied value must be of type {nameof(ServiceManifestCosmosDbConfigurationEntry)}",
                    nameof(requiredConfigurationEntry));
            }

            tenant.SetCosmosConfiguration(cosmosConfigurationEntry.ContainerDefinition, this.Configuration);
        }
    }
}
