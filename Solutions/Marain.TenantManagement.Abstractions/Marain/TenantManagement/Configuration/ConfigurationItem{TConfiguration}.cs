// <copyright file="ConfigurationItem{TConfiguration}.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Configuration;

using System.Collections.Generic;

/// <summary>
/// Base class for a particular type of configuration item that can be provided with a service enrollment.
/// </summary>
/// <typeparam name="TConfiguration">The type of configuration being used.</typeparam>
public abstract class ConfigurationItem<TConfiguration> : ConfigurationItem
{
    /// <summary>
    /// Gets or sets the storage configuration.
    /// </summary>
#nullable disable annotations
    public TConfiguration Configuration { get; set; }
#nullable restore annotations

    /// <inheritdoc/>
    public override IList<string> Validate()
    {
        var errors = new List<string>();

        if (this.Configuration == null)
        {
            errors.Add("The configuration item does not contain a value for the Configuration property.");
        }

        return errors;
    }
}