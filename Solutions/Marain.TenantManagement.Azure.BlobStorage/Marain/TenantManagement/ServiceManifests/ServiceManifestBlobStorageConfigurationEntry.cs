// <copyright file="ServiceManifestBlobStorageConfigurationEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using System;
    using System.Collections.Generic;

    using Corvus.Storage.Azure.BlobStorage.Tenancy;
    using Corvus.Tenancy;
    using Marain.TenantManagement.EnrollmentConfiguration;

    /// <summary>
    /// Service manifest configuration entry for blob storage.
    /// </summary>
    public class ServiceManifestBlobStorageConfigurationEntry : ServiceManifestRequiredConfigurationEntry
    {
        /// <summary>
        /// The content type of the configuration entry.
        /// </summary>
        public const string RegisteredContentType = BaseContentType + "azureblobstorage.v3";

        /// <inheritdoc/>
        public override string ContentType => RegisteredContentType;

        /// <inheritdoc/>
        public override string ExpectedConfigurationItemContentType =>
            EnrollmentBlobStorageConfigurationItem.RegisteredContentType;

        ///// <summary>
        ///// Gets or sets the container definition that this configuration entry relates to.
        ///// </summary>
        ///// TODO: this was a ContainerDefinition with "ContainerName" and "AccessType" properties.
        ///// We need to think through the migration for this.
        ///// With the V2 stuff, we have the rather unfortunate situation that the tenant property
        ///// containing the storage config corresponding to this required entry would have the
        ///// name "StorageConfiguration__{containerDefinition.ContainerName}", e.g.:
        /////     StorageConfiguration__corvustenancy
        /////     StorageConfiguration__operations
        /////     StorageConfiguration__claimpermissions
        /////     StorageConfiguration__resourceaccessrulesets
        ///// This meant that the logical container name had to be globally unique across all the
        ///// services a tenant is enrolled in.
        ///// Where do we want to get to? I think we want a property name structure that starts with
        ///// the service name to ensure uniqueness, e.g.:
        /////     MarainTenancy:BlobContainerConfiguration:corvustenancy
        /////     MarainOperations:BlobContainerConfiguration:operations
        /////     MarainClaims:BlobContainerConfiguration:claimpermissions
        /////     MarainClaims:BlobContainerConfiguration:resourceaccessrulesets
        ///// We could conceivably get away with something simpler:
        /////     MarainTenancy:corvustenancy
        /////     MarainOperations:operations
        /////     MarainClaims:claimpermissions
        /////     MarainClaims:resourceaccessrulesets
        ///// but what I dislike about that is that it's less clear what we're looking at when we
        ///// peer into a tenant's properties, and also it makes it harder to deal with change over
        ///// time. (E.g., if we end up needing to modify storage configuration for some technology
        ///// again in the future, or if a service changes the underlying storage technology that
        ///// it's using, it's not obvious from something like MarainOperations:operations that
        ///// it refers to a tenancy-V3-era Blob Storage configuration. So I prefer the idea of
        ///// {ServiceName}:{StorageConfigType}:{LogicalName}
        ///// For migration purposes, we need to be able to ensure that both V2 and V3 configuration
        ///// entries are present in the tenant for any services that have at some point been deployed
        ///// on V2. Brand new services don't want to be encumbered by this though. So maybe we need
        ///// some "V2ConfigurationRequired" setting.
        ///// So for V3, we need to know:
        /////     the property prefix (essentially the service name - CorvusTenancy, CorvusClaims etc)
        /////     the storage configuration type (e.g. BlobContainerConfiguration, TableConfiguration)
        ///// Question: do these storage config types need more qualification? Should it be
        ///// Corvus.Storage.Azure.TableStorage.Tenancy.TableConfiguration for example? Or perhaps
        ///// AzureStorageBlobContainerConfiguration, AzureStorageTableConfiguration?
        ///// But perhaps this can be convention, and we just use whatever's in the "key".
#nullable disable annotations
        ////public string ContainerDefinition { get; set; }
#nullable restore annotations

        /// <inheritdoc/>
        public override IEnumerable<KeyValuePair<string, object>> AddToTenantProperties(
            IEnumerable<KeyValuePair<string, object>> existingValues,
            EnrollmentConfigurationItem enrollmentConfigurationItem)
        {
            ArgumentNullException.ThrowIfNull(enrollmentConfigurationItem);

            if (enrollmentConfigurationItem is not EnrollmentBlobStorageConfigurationItem blobStorageConfigurationItem)
            {
                throw new ArgumentException(
                    $"The supplied value must be of type {nameof(EnrollmentBlobStorageConfigurationItem)}",
                    nameof(enrollmentConfigurationItem));
            }

            return existingValues.AddBlobStorageConfiguration(this.Key, blobStorageConfigurationItem.Configuration);
        }
    }
}