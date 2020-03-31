// <copyright file="TenantManagementService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Tenancy;

    /// <summary>
    /// Implementation of <see cref="ITenantManagementService"/> over the Corvus <see cref="ITenantProvider"/>.
    /// </summary>
    public class TenantManagementService : ITenantManagementService
    {
        private readonly ITenantProvider tenantProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantManagementService"/> class.
        /// </summary>
        /// <param name="tenantProvider">The underlying tenant provider used to access tenants.</param>
        public TenantManagementService(ITenantProvider tenantProvider)
        {
            this.tenantProvider = tenantProvider;
        }

        /// <inheritdoc/>
        public Task<ITenant> CreateClientTenantAsync(string clientName)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<ITenant> CreateServiceTenantAsync(string serviceName)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<ITenant> GetServiceTenantByNameAsync(string serviceName)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task InitialiseTenancyProviderAsync(bool force = false)
        {
            IList<string> existingTopLevelTenantIds = await this.tenantProvider.GetAllChildrenAsync(this.tenantProvider.Root.Id).ConfigureAwait(false);
            IEnumerable<Task<ITenant>> tenantRequests = existingTopLevelTenantIds.Select(x => this.tenantProvider.GetTenantAsync(x));

            ITenant[] existingTopLevelTenants = await Task.WhenAll(tenantRequests).ConfigureAwait(false);

            ITenant existingClientTenantParent = Array.Find(existingTopLevelTenants, x => x.Name == TenantNames.ClientTenantParent);
            ITenant existingServiceTenantParent = Array.Find(existingTopLevelTenants, x => x.Name == TenantNames.ServiceTenantParent);

            if (existingClientTenantParent != null && existingServiceTenantParent != null)
            {
                // All done - both parent tenants exist.
                return;
            }

            // The parent tenants don't exist. If there are other tenants, we're being asked to initialise into a non-empty
            // tenant provider, which we shouldn't do by default.
            if (existingTopLevelTenants.Length != 0 && !force)
            {
                throw new InvalidOperationException($"Cannot initialise the tenancy provider for use with Marain because it already contains {existingTopLevelTenants.Length} non-Marain tenants at the root level. If you wish to initialise anyway, re-invoke the method with the 'force' parameter set to true.");
            }

            // Create the tenants
            if (existingClientTenantParent == null)
            {
                await this.tenantProvider.CreateChildTenantAsync(
                    this.tenantProvider.Root.Id,
                    TenantNames.ClientTenantParent).ConfigureAwait(false);
            }

            if (existingServiceTenantParent == null)
            {
                await this.tenantProvider.CreateChildTenantAsync(
                    this.tenantProvider.Root.Id,
                    TenantNames.ServiceTenantParent).ConfigureAwait(false);
            }
        }
    }
}
