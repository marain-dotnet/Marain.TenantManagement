// <copyright file="TableStorageConfigurationItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Configuration;

using System.Collections.Generic;

using Corvus.Storage.Azure.TableStorage;
using Corvus.Storage.Azure.TableStorage.Tenancy;

/// <summary>
/// Enrollment configuration item for tenanted table storage config.
/// </summary>
public class TableStorageConfigurationItem : StorageConfigurationItem<TableConfiguration>
{
    /// <summary>
    /// The content type of the configuration item.
    /// </summary>
    public const string RegisteredContentType = ConfigurationItem.BaseContentType + "azuretablestorage";

    /// <inheritdoc/>
    public override string ContentType => RegisteredContentType;

    /// <inheritdoc/>
    public override IEnumerable<KeyValuePair<string, object>> AddConfiguration(IEnumerable<KeyValuePair<string, object>> values)
    {
        return values.AddTableStorageConfiguration(this.ConfigurationKey, this.Configuration);
    }
}