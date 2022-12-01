// <copyright file="ServiceManifestRequiredConfigurationEntryIncludingDescendants.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using System.Collections.Generic;

    /// <summary>
    /// A node in a complete tree of configuration items required in order to enroll a tenant in service.
    /// </summary>
    public class ServiceManifestRequiredConfigurationEntryIncludingDescendants
    {
        /// <summary>
        /// Creates a <see cref="ServiceManifestRequiredConfigurationEntryIncludingDescendants"/>.
        /// </summary>
        /// <param name="requiredConfigurationEntries">The <see cref="RequiredConfigurationEntries"/>.</param>
        /// <param name="dependencies">The <see cref="Dependencies"/>.</param>
        internal ServiceManifestRequiredConfigurationEntryIncludingDescendants(
            IList<ServiceManifestRequiredConfigurationEntry> requiredConfigurationEntries,
            IDictionary<string, ServiceManifestRequiredConfigurationEntryIncludingDescendants> dependencies)
        {
            this.RequiredConfigurationEntries = requiredConfigurationEntries;
            this.Dependencies = dependencies;
        }

        /// <summary>
        /// Gets the <see cref="ServiceManifestRequiredConfigurationEntry"/> items describing the
        /// direct requirements of this service.
        /// </summary>
        public IList<ServiceManifestRequiredConfigurationEntry> RequiredConfigurationEntries { get; }

        /// <summary>
        /// Gets a dictionary in which the keys are the ids of the services on which this depends,
        /// and the values are the configuration entries each dependency requires.
        /// </summary>
        public IDictionary<string, ServiceManifestRequiredConfigurationEntryIncludingDescendants> Dependencies { get; }
    }
}