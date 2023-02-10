// <copyright file="TenantManagementServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    using Corvus.ContentHandling;
    using Corvus.Tenancy;

    using Marain.TenantManagement.ServiceManifests;

    /// <summary>
    /// Helper methods to add Marain tenant management features to a service collection.
    /// </summary>
    public static class TenantManagementServiceCollectionExtensions
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
        public static IServiceCollection AddMarainTenantManagement(this IServiceCollection serviceCollection)
        {
            if (serviceCollection.Any(d => d.ServiceType == typeof(Sentinel)))
            {
                // We've already been called.
                return serviceCollection;
            }

            serviceCollection.AddTransient<Sentinel>();

            serviceCollection
                .AddContentTypeBasedSerializationSupport()
                .AddContent(AddTenantManagementContentTypes)
                .AddJsonSerializerOptionsProvider()
                .AddJsonPropertyBagFactory()
                .AddJsonCultureInfoConverter()
                .AddSingleton<JsonConverter>(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

            return serviceCollection;
        }

        private static void AddTenantManagementContentTypes(ContentFactory factory)
        {
            factory.RegisterContent<ServiceManifest>();
            factory.RegisterPolymorphicContentTarget<ServiceManifestRequiredConfigurationEntry>();
        }

        private class Sentinel
        {
        }
    }
}