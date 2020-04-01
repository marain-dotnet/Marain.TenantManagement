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
    using Corvus.Tenancy.Exceptions;
    using Marain.TenantManagement.ServiceManifests;

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
        public async Task<ITenant> CreateClientTenantAsync(string clientName)
        {
            ITenant? parent = await this.GetClientTenantParentAsync().ConfigureAwait(false);

            if (parent == null)
            {
                this.ThrowNotInitialisedException();
            }

            return await this.tenantProvider.CreateChildTenantAsync(parent!.Id, clientName).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<ITenant> CreateServiceTenantAsync(ServiceManifest manifest)
        {
            if (manifest == null)
            {
                throw new ArgumentNullException(nameof(manifest));
            }

            await manifest.ValidateAndThrowAsync(this).ConfigureAwait(false);

            ITenant? parent = await this.GetServiceTenantParentAsync().ConfigureAwait(false);

            if (parent == null)
            {
                this.ThrowNotInitialisedException();
            }

            // TODO: Make sure there isn't already a tenant with the same name.
            ITenant newTenant = await this.tenantProvider.CreateChildTenantAsync(
                parent!.Id,
                manifest.ServiceName).ConfigureAwait(false);

            newTenant.SetServiceManifest(manifest);

            await this.tenantProvider.UpdateTenantAsync(newTenant).ConfigureAwait(false);

            return newTenant;
        }

        /// <inheritdoc/>
        public async Task<ITenant?> GetServiceTenantByNameAsync(string serviceName)
        {
            ITenant? serviceTenantParent = await this.GetServiceTenantParentAsync().ConfigureAwait(false);

            if (serviceTenantParent == null)
            {
                this.ThrowNotInitialisedException();
            }

            return await this.GetTenantByNameAsync(serviceTenantParent!.Id, serviceName).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task InitialiseTenancyProviderAsync(bool force = false)
        {
            TenantCollectionResult existingTopLevelTenantIds = await this.tenantProvider.GetChildrenAsync(
                this.tenantProvider.Root.Id).ConfigureAwait(false);

            ITenant? existingClientTenantParent = await this.GetClientTenantParentAsync();
            ITenant? existingServiceTenantParent = await this.GetServiceTenantParentAsync();

            if (existingClientTenantParent != null && existingServiceTenantParent != null)
            {
                // All done - both parent tenants exist.
                return;
            }

            // The parent tenants don't exist. If there are other tenants, we're being asked to initialise into a non-empty
            // tenant provider, which we shouldn't do by default.
            if (existingTopLevelTenantIds.Tenants.Count != 0 && !force)
            {
                throw new InvalidOperationException($"Cannot initialise the tenancy provider for use with Marain because it already contains non-Marain tenants at the root level. If you wish to initialise anyway, re-invoke the method with the 'force' parameter set to true.");
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

        /// <inheritdoc/>
        public Task EnrollInServiceAsync(ITenant enrollingTenant, ITenant serviceTenant)
        {
            enrollingTenant.AddServiceEnrollment(serviceTenant.Id);

            return this.tenantProvider.UpdateTenantAsync(enrollingTenant);
        }

        private Task<ITenant?> GetClientTenantParentAsync() =>
            this.GetTenantByNameAsync(this.tenantProvider.Root.Id, TenantNames.ClientTenantParent);

        private Task<ITenant?> GetServiceTenantParentAsync() =>
            this.GetTenantByNameAsync(this.tenantProvider.Root.Id, TenantNames.ServiceTenantParent);

        private async Task<ITenant?> GetTenantByNameAsync(string parentTenantId, string name)
        {
            string? continuationToken = null;

            while (true)
            {
                TenantCollectionResult page = await this.tenantProvider.GetChildrenAsync(
                    parentTenantId,
                    20,
                    continuationToken).ConfigureAwait(false);

                IEnumerable<Task<ITenant>> tenantRequests = page.Tenants.Select(x => this.tenantProvider.GetTenantAsync(x));
                ITenant[] tenants = await Task.WhenAll(tenantRequests).ConfigureAwait(false);

                ITenant? matchingTenant = Array.Find(tenants, x => x.Name == name);

                if (matchingTenant != null)
                {
                    return matchingTenant;
                }

                if (string.IsNullOrEmpty(page.ContinuationToken))
                {
                    return null;
                }

                continuationToken = page.ContinuationToken;
            }
        }

        private void ThrowNotInitialisedException()
        {
            throw new InvalidOperationException("The underlying tenant provider has not been initialised for use with Marain. Please call the ITenantManagementService.InitialiseTenancyProviderAsync before attempting to create tenants.");
        }
    }
}
