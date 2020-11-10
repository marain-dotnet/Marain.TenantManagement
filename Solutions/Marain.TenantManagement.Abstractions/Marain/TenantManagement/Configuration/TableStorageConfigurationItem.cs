// <copyright file="TableStorageConfigurationItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Configuration
{
    using System;
    using System.Collections.Generic;
    using Corvus.Azure.Storage.Tenancy;

    /// <summary>
    /// Enrollment configuration item for tenanted table storage config.
    /// </summary>
    public class TableStorageConfigurationItem : StorageConfigurationItem<TableStorageTableDefinition, TableStorageConfiguration>
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
            if (this.Configuration.TableName == null)
            {
                throw new NullReferenceException($"{nameof(this.Configuration.TableName)} cannot be null.");
            }

            return values.AddTableStorageConfiguration(new TableStorageTableDefinition(this.Definition.TableName), this.Configuration);
        }
    }
}
