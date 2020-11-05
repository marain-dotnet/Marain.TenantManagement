﻿// <copyright file="BlobStorageConfigurationItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Configuration
{
    using System;
    using System.Collections.Generic;
    using Corvus.Azure.Storage.Tenancy;

    /// <summary>
    /// Enrollment configuration item for tenanted blob storage config.
    /// </summary>
    public class BlobStorageConfigurationItem : StorageConfigurationItem<BlobStorageConfiguration>
    {
        /// <summary>
        /// The content type of the configuration item.
        /// </summary>
        public const string RegisteredContentType = ConfigurationItem.BaseContentType + "azureblobstorage";

        /// <inheritdoc/>
        public override string ContentType => RegisteredContentType;

        /// <inheritdoc/>
        public override IEnumerable<KeyValuePair<string, object>> AddConfiguration(IEnumerable<KeyValuePair<string, object>> values)
        {
            if (this.Configuration.Container == null)
            {
                throw new NullReferenceException($"{nameof(this.Configuration.Container)} cannot be null.");
            }

            return values.AddBlobStorageConfiguration(new BlobStorageContainerDefinition(this.Configuration.Container), this.Configuration);
        }
    }
}
