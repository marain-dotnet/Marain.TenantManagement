// <copyright file="ServiceManifestBlobStorageConfigurationEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using System;
    using System.Collections.Generic;
    using Corvus.Azure.Storage.Tenancy;
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

#nullable disable annotations
        /// <summary>
        /// Gets or sets the container definition that this configuration entry relates to.
        /// </summary>
        public BlobStorageContainerDefinition ContainerDefinition { get; set; }
#nullable restore annotations

        /// <inheritdoc/>
        public override IList<string> Validate(string messagePrefix)
        {
            var results = new List<string>();
            results.AddRange(base.Validate(messagePrefix));

            if (this.ContainerDefinition == null)
            {
                results.Add($"{messagePrefix}: ContainerDefinition must be supplied for configuration entries with content type '{RegisteredContentType}'.");
            }

            if (string.IsNullOrWhiteSpace(this.ContainerDefinition?.ContainerName))
            {
                results.Add($"{messagePrefix}: ContainerDefinition.ContainerName must be supplied for configuration entries with content type '{RegisteredContentType}'.");
            }

            return results;
        }
    }
}
