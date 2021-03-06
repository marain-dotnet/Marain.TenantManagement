﻿// <copyright file="EnrollmentCosmosConfigurationItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.EnrollmentConfiguration
{
    using Corvus.Azure.Cosmos.Tenancy;

    /// <summary>
    /// Enrollment configuration item for tenanted cosmos storage config.
    /// </summary>
    public class EnrollmentCosmosConfigurationItem : EnrollmentStorageConfigurationItem<CosmosConfiguration>
    {
        /// <summary>
        /// The content type of the configuration item.
        /// </summary>
        public const string RegisteredContentType = EnrollmentConfigurationItem.BaseContentType + "cosmosdb";

        /// <inheritdoc/>
        public override string ContentType => RegisteredContentType;
    }
}
