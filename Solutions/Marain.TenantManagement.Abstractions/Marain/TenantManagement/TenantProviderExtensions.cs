﻿// <copyright file="TenantProviderExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Tenancy;

    /// <summary>
    /// Extension methods for the ITenantProvider interface.
    /// </summary>
    public static class TenantProviderExtensions
    {
        /// <summary>
        /// Retrieves all children of the specified tenant.
        /// </summary>
        /// <param name="tenantProvider">The underlying tenant provider to use.</param>
        /// <param name="tenantId">The Id of the parent tenant.</param>
        /// <returns>The list of child tenants.</returns>
        /// <remarks>
        /// This method will make as many calls to <see cref="ITenantProvider.GetChildrenAsync(string, int, string)"/> as
        /// needed to retrieve all of the child tenants. If there is a possibility that there's a large number of child
        /// tenants and the underlying provider is likely to be making expensive calls to retrieve tenants, this method
        /// should be used with extreme caution.
        /// </remarks>
        public static IAsyncEnumerable<string> EnumerateAllChildrenAsync(this ITenantProvider tenantProvider, string tenantId)
        {
            if (tenantId == null)
            {
                throw new ArgumentNullException(nameof(tenantId));
            }

            if (string.IsNullOrWhiteSpace(tenantId))
            {
                throw new ArgumentException(nameof(tenantId));
            }

            return EnumerateAllChildrenInternalAsync(tenantProvider, tenantId);
        }

        /// <summary>
        /// Gets the tenants for a given set of tenant IDs.
        /// </summary>
        /// <param name="tenantProvider">The underlying tenant provider to use.</param>
        /// <param name="tenantIds">The ids of the tenants to retrieve.</param>
        /// <returns>A task that produces the specified tenants.</returns>
        public static async Task<ITenant[]> GetTenantsAsync(this ITenantProvider tenantProvider, IEnumerable<string> tenantIds)
        {
            if (tenantIds == null)
            {
                throw new ArgumentNullException(nameof(tenantIds));
            }

            IEnumerable<Task<ITenant>> getTenantTasks = tenantIds.Select(tenantId => tenantProvider.GetTenantAsync(tenantId));

            return await Task.WhenAll(getTenantTasks).ConfigureAwait(false);
        }

        /// <summary>
        /// Internal method corresponding to <see cref="EnumerateAllChildrenAsync(ITenantProvider, string)"/>. The public method
        /// verifies the parameters are valid and this method implements the enumeration.
        /// </summary>
        private static async IAsyncEnumerable<string> EnumerateAllChildrenInternalAsync(
            ITenantProvider tenantProvider,
            string tenantId)
        {
            string? continuationToken = null;
            const int limit = 100;

            do
            {
                TenantCollectionResult results = await tenantProvider.GetChildrenAsync(
                    tenantId,
                    limit,
                    continuationToken).ConfigureAwait(false);

                foreach (string tenant in results.Tenants)
                {
                    yield return tenant;
                }

                continuationToken = results.ContinuationToken;
            }
            while (!string.IsNullOrEmpty(continuationToken));
        }
    }
}
