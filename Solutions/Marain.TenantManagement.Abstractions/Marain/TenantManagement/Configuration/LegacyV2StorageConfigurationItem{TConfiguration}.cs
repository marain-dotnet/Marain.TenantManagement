// <copyright file="LegacyV2StorageConfigurationItem{TConfiguration}.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Configuration;

/// <summary>
/// Configuration item for tenanted storage config.
/// </summary>
/// <typeparam name="TConfiguration">The type of storage configuration being used.</typeparam>
public abstract class LegacyV2StorageConfigurationItem<TConfiguration> : ConfigurationItem
    where TConfiguration : class
{
    /// <summary>
    /// Gets or sets the storage configuration.
    /// </summary>
#nullable disable annotations
    public TConfiguration Configuration { get; set; }
#nullable restore annotations
}