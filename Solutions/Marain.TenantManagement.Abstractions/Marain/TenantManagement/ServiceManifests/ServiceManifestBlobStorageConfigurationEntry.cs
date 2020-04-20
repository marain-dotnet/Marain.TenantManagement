// <copyright file="ServiceManifestBlobStorageConfigurationEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using System;
    using System.Collections.Generic;
    using Corvus.Azure.Storage.Tenancy;
    using Corvus.Tenancy;
    using Marain.TenantManagement.EnrollmentConfiguration;

    /// <summary>
    /// Service manifest configuration entry for blob storage.
    /// </summary>
    public class ServiceManifestBlobStorageConfigurationEntry : ServiceManifestRequiredConfigurationEntry
    {
        /// <summary>
        /// The content type of the configuration entry.
        /// </summary>
        public const string RegisteredContentType = BaseContentType + "azureblobstorage";

        /// <inheritdoc/>
        public override string ContentType => RegisteredContentType;

        /// <inheritdoc/>
        public override string ExpectedConfigurationItemContentType =>
            EnrollmentBlobStorageConfigurationItem.RegisteredContentType;

        /// <summary>
        /// Gets or sets the container definition that this configuration entry relates to.
        /// </summary>
#nullable disable annotations
        public BlobStorageContainerDefinition ContainerDefinition { get; set; }
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

            if (!(enrollmentConfigurationItem is EnrollmentBlobStorageConfigurationItem blobStorageConfigurationItem))
            {
                throw new ArgumentException(
                    $"The supplied value must be of type {nameof(EnrollmentBlobStorageConfigurationItem)}",
                    nameof(enrollmentConfigurationItem));
            }

            tenant.SetBlobStorageConfiguration(this.ContainerDefinition, blobStorageConfigurationItem.Configuration);
        }

        /// <inheritdoc/>
        public override void RemoveFromTenant(ITenant tenant)
        {
            tenant.ClearBlobStorageConfiguration(this.ContainerDefinition);
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
            else if (string.IsNullOrWhiteSpace(this.ContainerDefinition.ContainerName))
            {
                results.Add($"{messagePrefix}: ContainerDefinition.ContainerName must be supplied for configuration entries with content type '{RegisteredContentType}'.");
            }

            return results;
        }
    }
}
