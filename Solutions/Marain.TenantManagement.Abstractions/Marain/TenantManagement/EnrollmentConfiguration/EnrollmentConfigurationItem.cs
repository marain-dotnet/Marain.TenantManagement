﻿// <copyright file="EnrollmentConfigurationItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.EnrollmentConfiguration
{
    using System;
    using System.Collections.Generic;
    using Marain.TenantManagement.ServiceManifests;

    /// <summary>
    /// Base class for the different types of configuration item that can be provided with a service enrollment.
    /// </summary>
    public abstract class EnrollmentConfigurationItem
    {
        /// <summary>
        /// Base content type for configuration items.
        /// </summary>
        public const string BaseContentType = "application/vnd.marain.tenancy.enrollment.configurationitem.";

        /// <summary>
        /// Gets the content type for the configuration entry.
        /// </summary>
        public abstract string ContentType { get; }

        /// <summary>
        /// Gets or sets the key of the configuration entry. This is used to match configuration supplied as part of
        /// enrollment with the configuration entry it relates to.
        /// </summary>
#nullable disable annotations
        public string Key { get; set; }
#nullable restore annotations

        /// <summary>
        /// Validates the configuration item against it's corresponding required configuration entry.
        /// </summary>
        /// <param name="requiredConfigurationEntry">The corresponding required configuration entry.</param>
        /// <returns>A list of validation errors. An empty list means that the item is valid.</returns>
        public virtual IList<string> Validate(ServiceManifestRequiredConfigurationEntry requiredConfigurationEntry)
        {
            if (requiredConfigurationEntry == null)
            {
                throw new ArgumentNullException(nameof(requiredConfigurationEntry));
            }

            var errors = new List<string>();

            if (this.Key != requiredConfigurationEntry.Key)
            {
                errors.Add($"The provided ServiceManifestRequiredConfigurationEntry has a different key ('{requiredConfigurationEntry.Key}') to this configuration item. In order to validate the configuration item, the correct corresponding required configuration entry should be supplied.");
            }

            if (this.ContentType != requiredConfigurationEntry.ExpectedConfigurationItemContentType)
            {
                errors.Add($"The ContentType of this configuration item, '{this.ContentType}' does not match the expected content type of the required configuration entry, '{requiredConfigurationEntry.ExpectedConfigurationItemContentType}'");
            }

            return errors;
        }
    }
}
