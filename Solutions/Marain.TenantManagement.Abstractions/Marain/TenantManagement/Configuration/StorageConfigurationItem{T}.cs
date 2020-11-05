// <copyright file="StorageConfigurationItem{T}.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// Enrollment configuration item for tenanted storage config.
    /// </summary>
    /// <typeparam name="T">The type of storage configuration being used.</typeparam>
    public abstract class StorageConfigurationItem<T> : ConfigurationItem
        where T : class
    {
        /// <summary>
        /// Gets or sets the storage configueation.
        /// </summary>
#nullable disable annotations
        public T Configuration { get; set; }
#nullable restore annotations

        /// <summary>
        /// Validates the configuration item.
        /// </summary>
        /// <returns>A list of validation errors. An empty list means that the item is valid.</returns>
        public virtual IList<string> Validate()
        {
            var errors = new List<string>();

            if (this.Configuration == null)
            {
                errors.Add($"The configuration item with key '{this.Key}' does not contain a value for the Configuration property.");
            }

            return errors;
        }
    }
}
