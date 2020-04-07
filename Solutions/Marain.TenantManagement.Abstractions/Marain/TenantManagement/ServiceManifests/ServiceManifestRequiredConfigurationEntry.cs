// <copyright file="ServiceManifestRequiredConfigurationEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using System.Collections.Generic;
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
        /// Validates the configuration entry.
        /// </summary>
        /// <param name="messagePrefix">A string to prefix all validation errors from this entry with.</param>
        /// <returns>A list of validation errors. If the entry is valid, the list is empty.</returns>
        public virtual IList<string> Validate(string messagePrefix)
        {
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
