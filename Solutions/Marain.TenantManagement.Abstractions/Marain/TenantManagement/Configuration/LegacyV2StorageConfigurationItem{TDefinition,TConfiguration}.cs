// <copyright file="LegacyV2StorageConfigurationItem{TDefinition,TConfiguration}.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Configuration;

using System.Collections.Generic;

/// <summary>
/// Configuration item for tenanted storage config.
/// </summary>
/// <typeparam name="TDefinition">The type of storage definition being used.</typeparam>
/// <typeparam name="TConfiguration">The type of storage configuration being used.</typeparam>
public abstract class LegacyV2StorageConfigurationItem<TDefinition, TConfiguration> : ConfigurationItem
    where TDefinition : class
    where TConfiguration : class
{
    /// <summary>
    /// Gets or sets the storage definition.
    /// </summary>
#nullable disable annotations
    public TDefinition Definition { get; set; }
#nullable restore annotations

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

        if (this.Definition == null)
        {
            errors.Add("The configuration item does not contain a value for the Definition property.");
        }

        if (this.Configuration == null)
        {
            errors.Add("The configuration item does not contain a value for the Configuration property.");
        }

        return errors;
    }
}