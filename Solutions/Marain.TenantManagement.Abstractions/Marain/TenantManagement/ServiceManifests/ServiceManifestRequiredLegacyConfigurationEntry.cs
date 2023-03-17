// <copyright file="ServiceManifestRequiredLegacyConfigurationEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    /// <summary>
    /// Base class for legacy configuration entry types.
    /// </summary>
    /// <typeparam name="TContainerDefinition">
    /// The type of the container definition class that is used to generate the configuration key when adding configuration
    /// to the target tenant.
    /// </typeparam>
    public abstract class ServiceManifestRequiredLegacyConfigurationEntry<TContainerDefinition> : ServiceManifestRequiredConfigurationEntry
    {
        /// <summary>
        /// Gets or sets the legacy container definition which will be used to generate the configuration key.
        /// </summary>
#nullable disable annotations
        public TContainerDefinition ContainerDefinition { get; set; }
#nullable restore annotations
    }
}