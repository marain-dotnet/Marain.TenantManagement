// <copyright file="EnrollmentLegacyV2CosmosConfigurationItem.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.EnrollmentConfiguration;

using Corvus.Storage.Azure.Cosmos.Tenancy;

/// <summary>
/// Enrollment configuration item for tenanted cosmos storage config.
/// </summary>
public class EnrollmentLegacyV2CosmosConfigurationItem : EnrollmentStorageConfigurationItem<LegacyV2CosmosContainerConfiguration>
{
    /// <summary>
    /// The content type of the configuration item.
    /// </summary>
    public const string RegisteredContentType = EnrollmentConfigurationItem.BaseContentType + "cosmosdb";

    /// <inheritdoc/>
    public override string ContentType => RegisteredContentType;
}