// <copyright file="TableStorageConfigurationItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Configuration;

using Corvus.Storage.Azure.TableStorage;

/// <summary>
/// Enrollment configuration item for tenanted table storage config.
/// </summary>
public class TableStorageConfigurationItem : StorageConfigurationItem<TableConfiguration>
{
    /// <summary>
    /// The content type of the configuration item.
    /// </summary>
    public const string RegisteredContentType = ConfigurationItem.BaseContentType + "azuretablestorage.v3";

    /// <inheritdoc/>
    public override string ContentType => RegisteredContentType;
}