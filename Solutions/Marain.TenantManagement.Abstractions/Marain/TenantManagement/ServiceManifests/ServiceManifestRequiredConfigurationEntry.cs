// <copyright file="ServiceManifestRequiredConfigurationEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using System;
    using System.Collections.Generic;
    using Corvus.Tenancy;
    using Marain.TenantManagement.EnrollmentConfiguration;

    /// <summary>
    /// Base class for the different supported types of configuration entry.
    /// </summary>
    public abstract class ServiceManifestRequiredConfigurationEntry
    {
        /// <summary>
        /// Base content type for configuration entries.
        /// </summary>
        public const string BaseContentType = "application/vnd.marain.tenancy.servicemanifests.requiredconfigurationentries.";

        /// <summary>
        /// Gets the content type for the configuration entry.
        /// </summary>
        public abstract string ContentType { get; }

        /// <summary>
        /// Gets the expected content type of the <see cref="EnrollmentConfigurationItem"/> that should be provided for
        /// this configuration entry.
        /// </summary>
        public abstract string ExpectedConfigurationItemContentType { get; }

        /// <summary>
        /// Gets or sets the key of the configuration entry. This is used to match configuration supplied as part of
        /// enrollment with the configuration entry it relates to.
        /// </summary>
#nullable disable annotations
        public string Key { get; set; }
#nullable restore annotations

        /// <summary>
        /// Gets or sets the description of the configuration entry.
        /// </summary>
#nullable disable annotations
        public string Description { get; set; }
#nullable restore annotations

        /// <summary>
        /// Adds the configuration entry to the given tenant, using the supplied enrollment configuration item to provide the
        /// data.
        /// </summary>
        /// <param name="existingValues">
        /// The tenant's existing properties.
        /// </param>
        /// <param name="enrollmentConfigurationItem">
        /// The configuration item that contains the actual configuration to add to the tenant.
        /// </param>
        /// <returns>
        /// Properties to pass to
        /// <see cref="ITenantStore.UpdateTenantAsync(string, string?, IEnumerable{KeyValuePair{string, object}}?, IEnumerable{string}?)"/>.
        /// </returns>
        public abstract IEnumerable<KeyValuePair<string, object>> AddToTenantProperties(IEnumerable<KeyValuePair<string, object>> existingValues, EnrollmentConfigurationItem enrollmentConfigurationItem);

        /// <summary>
        /// Removes any data associated with this required configuration entry from the specified tenant.
        /// </summary>
        /// <param name="tenant">The tenant to remove configuration from.</param>
        /// <returns>
        /// A list of properties that can be passed to
        /// <see cref="ITenantStore.UpdateTenantAsync(string, string?, IEnumerable{KeyValuePair{string, object}}?, IEnumerable{string}?)"/>
        /// to remove the storage configuration.
        /// </returns>
        public abstract IEnumerable<string> GetPropertiesToRemoveFromTenant(ITenant tenant);

        /// <summary>
        /// Validates the configuration entry.
        /// </summary>
        /// <param name="messagePrefix">A string to prefix all validation errors from this entry with.</param>
        /// <returns>A list of validation errors. If the entry is valid, the list is empty.</returns>
        public virtual IList<string> Validate(string messagePrefix)
        {
            if (string.IsNullOrEmpty(messagePrefix))
            {
                throw new ArgumentException(nameof(messagePrefix));
            }

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(this.Key))
            {
                errors.Add($"{messagePrefix}: A Key must be supplied for each configuration entry.");
            }

            if (string.IsNullOrWhiteSpace(this.Description))
            {
                errors.Add($"{messagePrefix}: Description must be supplied for each configuration entry.");
            }

            return errors;
        }
    }
}
