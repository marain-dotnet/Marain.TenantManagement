// <copyright file="EnrollmentTableStorageConfigurationItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.EnrollmentConfiguration
{
    using Corvus.Azure.Storage.Tenancy;

    /// <summary>
    /// Enrollment configuration item for tenanted table storage config.
    /// </summary>
    public class EnrollmentTableStorageConfigurationItem : EnrollmentStorageConfigurationItem<TableStorageConfiguration>
    {
        /// <summary>
        /// The content type of the configuration item.
        /// </summary>
        public const string RegisteredContentType = EnrollmentConfigurationItem.BaseContentType + "azuretablestorage";

        /// <inheritdoc/>
        public override string ContentType => RegisteredContentType;
    }
}