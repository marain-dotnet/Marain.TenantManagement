// <copyright file="EnrollmentStorageConfigurationItem{T}.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.EnrollmentConfiguration
{
    using System.Collections.Generic;
    using Marain.TenantManagement.ServiceManifests;

    /// <summary>
    /// Enrollment configuration item for tenanted storage config.
    /// </summary>
    /// <typeparam name="T">The type of storage configuration being used.</typeparam>
    public abstract class EnrollmentStorageConfigurationItem<T> : EnrollmentConfigurationItem
        where T : class
    {
        /// <summary>
        /// Gets or sets the storage configueation.
        /// </summary>
#nullable disable annotations
        public T Configuration { get; set; }
#nullable restore annotations

        /// <inheritdoc/>
        public override IList<string> Validate(ServiceManifestRequiredConfigurationEntry requiredConfigurationEntry)
        {
            IList<string> result = base.Validate(requiredConfigurationEntry);

            if (this.Configuration == null)
            {
                result.Add($"The configuration item with key '{this.Key}' does not contain a value for the Configuration property.");
            }

            return result;
        }
    }
}