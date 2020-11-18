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

        /// <summary>
        /// Adds the configuration to a property set.
        /// </summary>
        /// <remarks>
        /// This is called by the tenant management service when adding configuration to the tenant.
        /// The implementations of this method are expected to update <paramref name="values"/> with the
        /// required key/value pair for this configuration item, then return the updated <paramref name="values"/>
        /// object.
        /// </remarks>
        /// <param name="values">The property set.</param>
        /// <returns>The updated property set.</returns>
        public abstract IEnumerable<KeyValuePair<string, object>> AddConfiguration(IEnumerable<KeyValuePair<string, object>> values);
    }
}
