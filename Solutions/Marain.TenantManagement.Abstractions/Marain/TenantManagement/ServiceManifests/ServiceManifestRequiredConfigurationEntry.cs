// <copyright file="ServiceManifestRequiredConfigurationEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using System;
    using System.Collections.Generic;

    using Corvus.Tenancy;

    using Marain.TenantManagement.Configuration;

    /// <summary>
    /// Base type for the different supported types of configuration entry.
    /// </summary>
    /// <param name="Key">
    /// Key of the configuration entry. This is used to match configuration supplied as part of
    /// enrollment with the configuration entry it relates to.
    /// </param>
    /// <param name="Description">Description of the configuration entry.</param>
    public abstract record ServiceManifestRequiredConfigurationEntry(
        string Key,
        string Description)
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
        /// Gets the expected content type of the <see cref="ConfigurationItem"/> that should be provided for
        /// this configuration entry.
        /// </summary>
        public abstract string[] ExpectedConfigurationItemContentTypes { get; }

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
        public abstract IEnumerable<KeyValuePair<string, object>> AddToTenantProperties(
            IEnumerable<KeyValuePair<string, object>> existingValues,
            ConfigurationItem enrollmentConfigurationItem);

        /// <summary>
        /// Removes any data associated with this required configuration entry from the specified tenant.
        /// </summary>
        /// <param name="tenant">The tenant to remove configuration from.</param>
        /// <returns>
        /// A list of properties that can be passed to
        /// <see cref="ITenantStore.UpdateTenantAsync(string, string?, IEnumerable{KeyValuePair{string, object}}?, IEnumerable{string}?)"/>
        /// to remove the storage configuration.
        /// </returns>
        public virtual IEnumerable<string> GetPropertiesToRemoveFromTenant(ITenant tenant)
        {
            return new[] { this.Key };
        }

        /// <summary>
        /// Validates the configuration entry.
        /// </summary>
        /// <param name="messagePrefix">A string to prefix all validation errors from this entry with.</param>
        /// <returns>A list of validation errors. If the entry is valid, the list is empty.</returns>
        public virtual IList<string> Validate(string messagePrefix)
        {
            ArgumentNullException.ThrowIfNull(messagePrefix);
            if (string.IsNullOrEmpty(messagePrefix))
            {
                throw new ArgumentException("Message prefix must be non-empty", nameof(messagePrefix));
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