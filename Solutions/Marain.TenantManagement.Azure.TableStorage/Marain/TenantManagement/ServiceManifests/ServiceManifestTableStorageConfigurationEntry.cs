// <copyright file="ServiceManifestTableStorageConfigurationEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using System;
    using System.Collections.Generic;
    using Corvus.Storage.Azure.TableStorage.Tenancy;
    using Marain.TenantManagement.EnrollmentConfiguration;

    /// <summary>
    /// Service manifest configuration entry for table storage.
    /// </summary>
    public class ServiceManifestTableStorageConfigurationEntry : ServiceManifestRequiredConfigurationEntry
    {
        /// <summary>
        /// The content type of the configuration entry.
        /// </summary>
        public const string RegisteredContentType = BaseContentType + "azuretablestorage.v3";

        /// <inheritdoc/>
        public override string ContentType => RegisteredContentType;

        /// <inheritdoc/>
        public override string ExpectedConfigurationItemContentType =>
            EnrollmentTableStorageConfigurationItem.RegisteredContentType;

        /// <inheritdoc/>
        public override IEnumerable<KeyValuePair<string, object>> AddToTenantProperties(
            IEnumerable<KeyValuePair<string, object>> existingValues,
            EnrollmentConfigurationItem enrollmentConfigurationItem)
        {
            ArgumentNullException.ThrowIfNull(enrollmentConfigurationItem);

            if (enrollmentConfigurationItem is not EnrollmentTableStorageConfigurationItem tableStorageConfigurationItem)
            {
                throw new ArgumentException(
                    $"The supplied value must be of type {nameof(EnrollmentTableStorageConfigurationItem)}",
                    nameof(enrollmentConfigurationItem));
            }

            return existingValues.AddTableStorageConfiguration(this.Key, tableStorageConfigurationItem.Configuration);
        }
    }
}