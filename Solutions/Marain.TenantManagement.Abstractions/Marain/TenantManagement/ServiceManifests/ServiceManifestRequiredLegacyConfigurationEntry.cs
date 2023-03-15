// <copyright file="ServiceManifestRequiredLegacyConfigurationEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    /// <summary>
    /// Base class for legacy configuration entry types.
    /// </summary>
    public abstract class ServiceManifestRequiredLegacyConfigurationEntry : ServiceManifestRequiredConfigurationEntry
    {
        /// <summary>
        /// The prefix for legacy v2 configuration entries.
        /// </summary>
        private const string LegacyV2StorageConfigurationEntryKeyPrefix = "StorageConfiguration__";

        /// <summary>
        /// Gets the configuration entry key in the style of legacy entries by prefixing it with
        /// <see cref="LegacyV2StorageConfigurationEntryKeyPrefix"/>.
        /// </summary>
        /// <returns>The prefixed key that should be used when adding the configuration to the target tenant.</returns>
        public string LegacyConfigurationEntryKey =>
            LegacyV2StorageConfigurationEntryKeyPrefix + this.Key;
    }
}