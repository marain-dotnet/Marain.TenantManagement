// <copyright file="EnrollmentBlobStorageConfigurationItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.EnrollmentConfiguration
{
    using System;
    using Corvus.Azure.Storage.Tenancy;
    using Corvus.Tenancy;
    using Marain.TenantManagement.ServiceManifests;

    /// <summary>
    /// Enrollment configuration item for tenanted blob storage config.
    /// </summary>
    public class EnrollmentBlobStorageConfigurationItem : EnrollmentStorageConfigurationItem<BlobStorageConfiguration>
    {
        /// <summary>
        /// The content type of the configuration item.
        /// </summary>
        public const string RegisteredContentType = EnrollmentConfigurationItem.BaseContentType + "azureblobstorage";

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

            if (!(requiredConfigurationEntry is ServiceManifestBlobStorageConfigurationEntry blobStorageConfigurationEntry))
            {
                throw new ArgumentException(
                    $"The supplied value must be of type {nameof(ServiceManifestBlobStorageConfigurationEntry)}",
                    nameof(requiredConfigurationEntry));
            }

            tenant.SetBlobStorageConfiguration(blobStorageConfigurationEntry.ContainerDefinition, this.Configuration);
        }
    }
}
