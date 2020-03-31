// <copyright file="TenantProviderExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Internal
{
    using System.Collections.Generic;
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
        public static async Task<IList<string>> GetAllChildrenAsync(this ITenantProvider tenantProvider, string tenantId)
        {
            string? continuationToken = null;
            const int limit = 100;

            var tenants = new List<string>();

            do
            {
                TenantCollectionResult results = await tenantProvider.GetChildrenAsync(
                    tenantId,
                    limit,
                    continuationToken).ConfigureAwait(false);

                tenants.AddRange(results.Tenants);

                continuationToken = results.ContinuationToken;
            }
            while (!string.IsNullOrEmpty(continuationToken));

            return tenants;
        }
    }
}
