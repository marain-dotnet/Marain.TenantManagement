// <copyright file="ConfigurationItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Configuration;

using System.Collections.Generic;

using Marain.TenantManagement.EnrollmentConfiguration;

/// <summary>
/// Base class for the different types of configuration item that can be provided with a service enrollment.
/// </summary>
/// <remarks>
/// <para>
/// In practice, configuration types derive from <see cref="ConfigurationItem{TConfiguration}"/>,
/// but there are places where we need to be able to hold any configuration entry without knowing
/// its type at compile time (e.g. <see cref="EnrollmentConfigurationEntry.ConfigurationItems"/>).
/// </para>
/// </remarks>
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
}