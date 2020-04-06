// <copyright file="ServiceManifestRequiredConfigurationEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Base class for the different supported types of configuration entry.
    /// </summary>
    public abstract class ServiceManifestRequiredConfigurationEntry
    {
        /// <summary>
        /// Base content type for configuration entries.
        /// </summary>
        public const string BaseContentType = "application/vnd.marain.tenancy.servicemanifests.";

        /// <summary>
        /// Gets the content type for the configuration entry.
        /// </summary>
        public abstract string ContentType { get; }

        /// <summary>
        /// Gets or sets the description of the configuration entry.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Validates the configuration entry.
        /// </summary>
        /// <param name="messagePrefix">A string to prefix all validation errors from this entry with.</param>
        /// <returns>A list of validation errors. If the entry is valid, the list is empty.</returns>
        public virtual IList<string> Validate(string messagePrefix)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(this.Description))
            {
                errors.Add($"{messagePrefix}: Description must be supplied for each configuration entry.");
            }

            return errors;
        }
    }
}
