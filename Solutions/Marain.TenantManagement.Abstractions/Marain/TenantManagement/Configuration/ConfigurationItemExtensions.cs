// <copyright file="ConfigurationItemExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using Marain.TenantManagement.Exceptions;
    using Marain.TenantManagement.ServiceManifests;

    /// <summary>
    /// Extension methods for the <see cref="ConfigurationItem"/> class.
    /// </summary>
    public static class ConfigurationItemExtensions
    {
        /// <summary>
        /// Validates the list of <see cref="ConfigurationItem"/> and throws an
        /// <see cref="InvalidConfigurationException"/> if any errors are encountered.
        /// </summary>
        /// <param name="configurationItems">The configuration items to validate.</param>
        /// <exception cref="InvalidConfigurationException">The configuration items contain one or more errors.</exception>
        public static void ValidateAndThrow(
            this IEnumerable<ConfigurationItem> configurationItems)
        {
            var errors = new List<string>();

            foreach (ConfigurationItem configurationItem in configurationItems)
            {
                errors.AddRange(configurationItem.Validate());
            }

            if (errors.Count > 0)
            {
                throw new InvalidConfigurationException(errors);
            }
        }
    }
}
