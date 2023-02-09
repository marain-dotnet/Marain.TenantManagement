// <copyright file="TenantManagementTableStorageServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using Corvus.ContentHandling;
    using Corvus.Tenancy;

    using Marain.TenantManagement.Configuration;
    using Marain.TenantManagement.ServiceManifests;

    /// <summary>
    /// Helper methods to add Marain tenant management features to a service collection.
    /// </summary>
    public static class TenantManagementTableStorageServiceCollectionExtensions
    {
        /// <summary>
        /// Adds required dependencies to use management extensions for <see cref="ITenantStore"/>.
        /// </summary>
        /// <param name="serviceCollection">The service collection to add to.</param>
        /// <returns>The modified service collection, for chaining.</returns>
        /// <remarks>
        /// You should ensure that an implementation of
        /// <c>ITenantProvider</c> has also been added to the service collection.
        /// </remarks>
        public static IServiceCollection AddMarainTenantManagementForTableStorage(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddMarainTenantManagement();
            serviceCollection.AddContent(AddTenantManagementContentTypes);
            return serviceCollection;
        }

        private static void AddTenantManagementContentTypes(ContentFactory factory)
        {
            factory.RegisterContent<ServiceManifestTableStorageConfigurationEntry>();
            factory.RegisterContent<ServiceManifestLegacyV2TableStorageConfigurationEntry>();

            factory.RegisterContent<TableConfigurationItem>();
            factory.RegisterContent<LegacyV2TableStorageConfigurationItem>();
        }
    }
}