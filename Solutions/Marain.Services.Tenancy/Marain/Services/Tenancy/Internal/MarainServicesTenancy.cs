// <copyright file="MarainServicesTenancy.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Services.Tenancy.Internal
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Marain.TenantManagement;

    /// <summary>
    /// Provides methods required by Marain services to validate and work with tenants.
    /// </summary>
    public class MarainServicesTenancy : IMarainServicesTenancy
    {
        private readonly ITenantProvider tenantProvider;
        private readonly MarainServiceConfiguration serviceConfiguration;

        /// <summary>
        /// Creates a new instance of the <see cref="MarainServicesTenancy"/> class.
        /// </summary>
        /// <param name="tenantProvider">The tenant management service.</param>
        /// <param name="serviceConfiguration">Service configuration for the current service.</param>
        public MarainServicesTenancy(
            ITenantProvider tenantProvider,
            MarainServiceConfiguration serviceConfiguration)
        {
            this.tenantProvider = tenantProvider;
            this.serviceConfiguration = serviceConfiguration;
        }

        /// <inheritdoc/>
        public async Task<ITenant> GetRequestingTenantAsync(string tenantId)
        {
            ITenant tenant = await this.GetTenantAsync(tenantId).ConfigureAwait(false);

            tenant.EnsureTenantIsOfType(MarainTenantType.Client, MarainTenantType.Delegated);

            // Ensure the tenant is enrolled for the service.
            if (!tenant.IsEnrolledForService(this.serviceConfiguration.ServiceTenantId))
            {
                throw new ArgumentException(
                    $"The tenant with Id '{tenantId}' is not enrolled in the service '{this.serviceConfiguration.ServiceDisplayName}' with Service Tenant Id '{this.serviceConfiguration.ServiceTenantId}'");
            }

            return tenant;
        }

        /// <inheritdoc/>
        public async Task<string> GetDelegatedTenantIdForRequestingTenantAsync(string tenantId)
        {
            ITenant tenant = await this.GetTenantAsync(tenantId).ConfigureAwait(false);
            return tenant.GetDelegatedTenantIdForServiceId(this.serviceConfiguration.ServiceTenantId);
        }

        private async Task<ITenant> GetTenantAsync(string tenantId)
        {
            return await this.tenantProvider.GetTenantAsync(tenantId).ConfigureAwait(false);
        }
    }
}