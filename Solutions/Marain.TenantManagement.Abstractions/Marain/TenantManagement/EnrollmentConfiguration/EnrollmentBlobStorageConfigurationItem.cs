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
    }
}
