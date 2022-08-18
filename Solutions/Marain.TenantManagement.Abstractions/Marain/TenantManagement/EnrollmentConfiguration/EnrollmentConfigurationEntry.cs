// <copyright file="EnrollmentConfigurationEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.EnrollmentConfiguration
{
    using System.Collections.Generic;
    using System.Collections.Immutable;

    using Marain.TenantManagement.Configuration;

    /// <summary>
    /// Configuration for a service enrollment.
    /// </summary>
    public class EnrollmentConfigurationEntry
    {
        /// <summary>
        /// Creates an <see cref="EnrollmentConfigurationEntry"/>.
        /// </summary>
        /// <param name="configurationItems">The <see cref="ConfigurationItems"/>.</param>
        /// <param name="dependencies">The <see cref="Dependencies"/>.</param>
        public EnrollmentConfigurationEntry(
            IReadOnlyDictionary<string, ConfigurationItem> configurationItems,
            IReadOnlyDictionary<string, EnrollmentConfigurationEntry>? dependencies)
        {
            this.ConfigurationItems = configurationItems;
            this.Dependencies = dependencies ?? ImmutableDictionary<string, EnrollmentConfigurationEntry>.Empty;
        }

        /// <summary>
        /// Gets this enrollment's configuration items for the service being enrolled.
        /// </summary>
        public IReadOnlyDictionary<string, ConfigurationItem> ConfigurationItems { get; }

        /// <summary>
        /// Gets a dictionary with dependency configuration.
        /// </summary>
        /// <remarks>
        /// If the service being enrolled depends on other services, this dictionary contains their
        /// configuration. The key is the service tenant id.
        /// </remarks>
        public IReadOnlyDictionary<string, EnrollmentConfigurationEntry> Dependencies { get; }

        /// <summary>
        /// Gets or sets the optional service name.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The service is identified by the tenant id, so we don't strictly need to specify the
        /// name, but configuration files can include this, partly to make it easier to see which
        /// services the file is trying to configure, but also to enable the tools to detect when
        /// there is a mismatch between the tenant id and service name.
        /// </para>
        /// </remarks>
        public string? ServiceName { get; set; }

        /// <summary>
        /// Gets an empty <see cref="EnrollmentConfigurationEntry"/>.
        /// </summary>
        /// <remarks>
        /// In cases where dependencies have no configuration requirements, enrollment
        /// configuration can omit the relevant section (so that we don't oblige enrollment files
        /// to have lots of named, empty sections in them), but internally it's useful to be able
        /// to presume that there is always configuration, so we plug this in for cases where no
        /// configuration has been supplied.
        /// </remarks>
        internal static EnrollmentConfigurationEntry Empty { get; } = new(
            ImmutableDictionary<string, ConfigurationItem>.Empty,
            ImmutableDictionary<string, EnrollmentConfigurationEntry>.Empty);
    }
}