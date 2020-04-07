// <copyright file="TenantManagementServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using Corvus.ContentHandling;
    using Marain.TenantManagement;
    using Marain.TenantManagement.EnrollmentConfiguration;
    using Marain.TenantManagement.Internal;
    using Marain.TenantManagement.ServiceManifests;

    /// <summary>
    /// Helper methods to add Marain tenant management features to a service collection.
    /// </summary>
    public static class TenantManagementServiceCollectionExtensions
    {
        /// <summary>
        /// Adds an implementation of <see cref="ITenantManagementService"/> to the supplied service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection to add to.</param>
        /// <returns>The modified service collection, for chaining.</returns>
        /// <remarks>
        /// In order to support the <see cref="ITenantManagementService"/>, you should ensure that an implementation of
        /// <c>ITenantProvider</c> has also been added to the service collection.
        /// </remarks>
        public static IServiceCollection AddMarainTenantManagement(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddContentSerialization();
            serviceCollection.AddContent(AddTenantManagementContentTypes);
            serviceCollection.AddJsonSerializerSettings();
            serviceCollection.AddSingleton<ITenantManagementService, TenantManagementService>();
            return serviceCollection;
        }

        private static void AddTenantManagementContentTypes(ContentFactory factory)
        {
            factory.RegisterTransientContent<ServiceManifest>();
            factory.RegisterTransientContent<ServiceManifestBlobStorageConfigurationEntry>();
            factory.RegisterTransientContent<ServiceManifestCosmosDbConfigurationEntry>();

            factory.RegisterTransientContent<EnrollmentBlobStorageConfigurationItem>();
            factory.RegisterTransientContent<EnrollmentCosmosConfigurationItem>();
        }
    }
}
