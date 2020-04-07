// <copyright file="EnrollmentConfigurationItemExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.EnrollmentConfiguration
{
    using System.Collections.Generic;
    using System.Linq;
    using Marain.TenantManagement.Exceptions;
    using Marain.TenantManagement.ServiceManifests;

    /// <summary>
    /// Extension methods for the <see cref="EnrollmentConfigurationItem"/> class.
    /// </summary>
    public static class EnrollmentConfigurationItemExtensions
    {
        /// <summary>
        /// Validates the list of <see cref="EnrollmentConfigurationItem"/> against a list of
        /// <see cref="ServiceManifestRequiredConfigurationEntry"/> and throws an
        /// <see cref="InvalidEnrollmentConfigurationException"/> if any errors are encountered.
        /// </summary>
        /// <param name="providedConfigurationItems">The configuration items to validate.</param>
        /// <param name="requiredConfigurationEntries">The list of <see cref="ServiceManifestRequiredConfigurationEntry"/> to
        /// validate against.</param>
        /// <exception cref="InvalidEnrollmentConfigurationException">The configuration items contain one or more errors.</exception>
        public static void ValidateAndThrow(
            this EnrollmentConfigurationItem[] providedConfigurationItems,
            ServiceManifestRequiredConfigurationEntry[] requiredConfigurationEntries)
        {
            // First, use the keys to pair up the required items with the provided.
            (ServiceManifestRequiredConfigurationEntry RequiredConfigurationEntry, EnrollmentConfigurationItem[] ProvidedConfigurationItems)[] pairedConfigurationEntries =
                requiredConfigurationEntries.Select(
                    requiredConfigEntry => (
                        requiredConfigEntry,
                        providedConfigurationItems.Where(
                            providedItem => providedItem.Key == requiredConfigEntry.Key).ToArray())).ToArray();

            var errors = new List<string>();

            // Find required config entry without corresponding config item.
            foreach ((ServiceManifestRequiredConfigurationEntry RequiredConfigurationEntry, EnrollmentConfigurationItem[] ProvidedConfigurationItems) current in pairedConfigurationEntries.Where(x => x.ProvidedConfigurationItems.Length == 0))
            {
                errors.Add($"No configuration was supplied for the required configuration entry with key '{current.RequiredConfigurationEntry.Key}' and description '{current.RequiredConfigurationEntry.Description}'");
            }

            // Find required config entry with multiple corresponding config item
            foreach ((ServiceManifestRequiredConfigurationEntry RequiredConfigurationEntry, EnrollmentConfigurationItem[] ProvidedConfigurationItems) current in pairedConfigurationEntries.Where(x => x.ProvidedConfigurationItems.Length > 1))
            {
                errors.Add($"Multiple configuration items were supplied for the required configuration entry with key '{current.RequiredConfigurationEntry.Key}' and description '{current.RequiredConfigurationEntry.Description}'. Only a single item should be supplied for each required configuration entry.");
            }

            // Now, foreach config item, validate it using the supplied configuration entry
            errors.AddRange(pairedConfigurationEntries.Where(
                pair => pair.ProvidedConfigurationItems.Length == 1)
                .SelectMany(pair => pair.ProvidedConfigurationItems[0].Validate(pair.RequiredConfigurationEntry)));

            if (errors.Count > 0)
            {
                throw new InvalidEnrollmentConfigurationException(errors);
            }
        }
    }
}
