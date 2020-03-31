// <copyright file="TenantManagementServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using Marain.TenantManagement;
    using Marain.TenantManagement.Internal;
    using Microsoft.Extensions.DependencyInjection;

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
            serviceCollection.AddSingleton<ITenantManagementService, TenantManagementService>();
            return serviceCollection;
        }
    }
}
