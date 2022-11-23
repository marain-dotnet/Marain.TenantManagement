// <copyright file="ServiceManifestRequiredConfigurationEntryIncludingDescendantsExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using System.Collections.Generic;
    using System.Linq;

    using Marain.TenantManagement.Configuration;
    using Marain.TenantManagement.EnrollmentConfiguration;
    using Marain.TenantManagement.Exceptions;

    /// <summary>
    /// Extension methods for the <see cref="EnrollmentConfigurationEntry"/> class.
    /// </summary>
    public static class ServiceManifestRequiredConfigurationEntryIncludingDescendantsExtensions
    {
        /// <summary>
        /// Validates the list of <see cref="EnrollmentConfigurationEntry"/> against a list of
        /// <see cref="ServiceManifestRequiredConfigurationEntry"/> and throws an
        /// <see cref="InvalidEnrollmentConfigurationException"/> if any errors are encountered.
        /// </summary>
        /// <param name="requiredConfiguration">The <see cref="ServiceManifestRequiredConfigurationEntry"/> to
        /// validate against.</param>
        /// <param name="providedConfiguration">The configuration to validate.</param>
        /// <exception cref="InvalidEnrollmentConfigurationException">The configuration items contain one or more errors.</exception>
        internal static void ValidateAndThrow(
            this ServiceManifestRequiredConfigurationEntryIncludingDescendants requiredConfiguration,
            EnrollmentConfigurationEntry providedConfiguration)
        {
            var errors = new List<string>();

            void ValidateAndThrowCore(
                EnrollmentConfigurationEntry currentProvidedConfiguration,
                ServiceManifestRequiredConfigurationEntryIncludingDescendants currentRequiredConfiguration)
            {
                // First, use the keys to pair up the required items with the provided.
                (ServiceManifestRequiredConfigurationEntry RequiredConfigurationEntry, ConfigurationItem? ProvidedConfigurationItem)[] pairedConfigurationEntries =
                    currentRequiredConfiguration.RequiredConfigurationEntries.Select(
                        requiredConfigEntry => (
                            requiredConfigEntry,
                            currentProvidedConfiguration.ConfigurationItems.TryGetValue(requiredConfigEntry.Key, out ConfigurationItem? r) ? r : null)).ToArray();

                // Find required config entry without corresponding config item.
                foreach ((ServiceManifestRequiredConfigurationEntry currentRequiredConfigurationEntry, ConfigurationItem? providedConfigurationItem) in pairedConfigurationEntries)
                {
                    if (providedConfigurationItem is null)
                    {
                        errors.Add($"No configuration was supplied for the required configuration entry with key '{currentRequiredConfigurationEntry.Key}' and description '{currentRequiredConfigurationEntry.Description}'");
                    }
                    else
                    {
                        if (!currentRequiredConfigurationEntry.ExpectedConfigurationItemContentTypes.Contains(providedConfigurationItem.ContentType))
                        {
                            string contentTypeText = currentRequiredConfigurationEntry.ExpectedConfigurationItemContentTypes.Length == 1
                                ? $"content type {currentRequiredConfigurationEntry.ExpectedConfigurationItemContentTypes[0]}"
                                : $"one of these content types: {string.Join(",", currentRequiredConfigurationEntry.ExpectedConfigurationItemContentTypes)}";
                            errors.Add($"The configuration supplied for the required configuration entry with key '{currentRequiredConfigurationEntry.Key}' and description '{currentRequiredConfigurationEntry.Description}' should have had {contentTypeText} but was {providedConfigurationItem.ContentType}");
                        }
                        else
                        {
                            errors.AddRange(providedConfigurationItem.Validate());
                        }
                    }
                }

                foreach ((string dependencyKey, ServiceManifestRequiredConfigurationEntryIncludingDescendants? dependencyRequiredConfig) in currentRequiredConfiguration.Dependencies)
                {
                    if (!currentProvidedConfiguration.Dependencies.TryGetValue(dependencyKey, out EnrollmentConfigurationEntry? dependencyConfiguration))
                    {
                        // It's not necessarily an error when the supplied configuration contains
                        // nothing for a dependency - it might be that neither this dependency nor
                        // any of its transitive dependencies need any configuration. We still have
                        // to walk the whole dependency tree, but we don't raise an error unless we
                        // find a case where a particular configuration entry was required and not
                        // supplied.
                        dependencyConfiguration = EnrollmentConfigurationEntry.Empty;
                    }

                    ValidateAndThrowCore(dependencyConfiguration, dependencyRequiredConfig);
                }
            }

            ValidateAndThrowCore(providedConfiguration, requiredConfiguration);

            if (errors.Count > 0)
            {
                throw new InvalidEnrollmentConfigurationException(errors);
            }
        }
    }
}