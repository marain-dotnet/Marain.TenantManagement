// <copyright file="ConfigurationItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// Base class for the different types of configuration item that can be provided with a service enrollment.
    /// </summary>
    public abstract class ConfigurationItem
    {
        /// <summary>
        /// Base content type for configuration items.
        /// </summary>
        public const string BaseContentType = "application/vnd.marain.tenancy.configurationitem.";

        /// <summary>
        /// Gets the content type for the configuration entry.
        /// </summary>
        public abstract string ContentType { get; }

        /// <summary>
        /// Validates the configuration item.
        /// </summary>
        /// <returns>A list of validation errors. An empty list means that the item is valid.</returns>
        public abstract IList<string> Validate();
    }
}