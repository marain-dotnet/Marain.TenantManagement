// <copyright file="TenantManagementService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Corvus.Tenancy.Exceptions;
    using Marain.TenantManagement.EnrollmentConfiguration;
    using Marain.TenantManagement.Exceptions;
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
            if (string.IsNullOrWhiteSpace(clientName))
            {
                throw new ArgumentException(nameof(clientName));
            }

            ITenant? parent = await this.GetClientTenantParentAsync().ConfigureAwait(false);

            if (parent == null)
            {
                this.ThrowNotInitialisedException();
            }

            return await this.tenantProvider.CreateChildTenantAsync(parent.Id, clientName).ConfigureAwait(false);
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

            ITenant newTenant = await this.tenantProvider.CreateChildTenantAsync(
                parent.Id,
                manifest.ServiceName).ConfigureAwait(false);

            newTenant.SetServiceManifest(manifest);

            await this.tenantProvider.UpdateTenantAsync(newTenant).ConfigureAwait(false);

            return newTenant;
        }

        /// <inheritdoc/>
        public async Task<ITenant?> GetServiceTenantByNameAsync(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
            {
                throw new ArgumentException(nameof(serviceName));
            }

            ITenant? serviceTenantParent = await this.GetServiceTenantParentAsync().ConfigureAwait(false);

            if (serviceTenantParent == null)
            {
                this.ThrowNotInitialisedException();
            }

            return await this.GetTenantByNameAsync(serviceTenantParent.Id, serviceName).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task InitialiseTenancyProviderAsync(bool force = false)
        {
            TenantCollectionResult existingTopLevelTenantIds = await this.tenantProvider.GetChildrenAsync(
                this.tenantProvider.Root.Id).ConfigureAwait(false);

            ITenant? existingClientTenantParent = await this.GetClientTenantParentAsync().ConfigureAwait(false);
            ITenant? existingServiceTenantParent = await this.GetServiceTenantParentAsync().ConfigureAwait(false);

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
        public async Task EnrollInServiceAsync(
            string enrollingTenantId,
            string serviceTenantName,
            EnrollmentConfigurationItem[] configurationItems)
        {
            if (string.IsNullOrWhiteSpace(enrollingTenantId))
            {
                throw new ArgumentException(nameof(enrollingTenantId));
            }

            if (string.IsNullOrWhiteSpace(serviceTenantName))
            {
                throw new ArgumentException(nameof(serviceTenantName));
            }

            if (configurationItems == null)
            {
                throw new ArgumentNullException(nameof(configurationItems));
            }

            ITenant enrollingTenant = await this.tenantProvider.GetTenantAsync(enrollingTenantId).ConfigureAwait(false);

            ITenant serviceTenant = await this.GetServiceTenantByNameAsync(serviceTenantName).ConfigureAwait(false)
                ?? throw new TenantNotFoundException($"Could not find a service tenant with the name '{serviceTenantName}'");

            await this.EnrollInServiceAsync(enrollingTenant, serviceTenant, configurationItems).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task EnrollInServiceAsync(
            ITenant enrollingTenant,
            string serviceTenantName,
            EnrollmentConfigurationItem[] configurationItems)
        {
            if (enrollingTenant == null)
            {
                throw new ArgumentNullException(nameof(enrollingTenant));
            }

            if (string.IsNullOrWhiteSpace(serviceTenantName))
            {
                throw new ArgumentException(nameof(serviceTenantName));
            }

            if (configurationItems == null)
            {
                throw new ArgumentNullException(nameof(configurationItems));
            }

            ITenant serviceTenant = await this.GetServiceTenantByNameAsync(serviceTenantName).ConfigureAwait(false)
                ?? throw new TenantNotFoundException($"Could not find a service tenant with the name '{serviceTenantName}'");

            await this.EnrollInServiceAsync(enrollingTenant, serviceTenant, configurationItems).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task EnrollInServiceAsync(
            ITenant enrollingTenant,
            ITenant serviceTenant,
            EnrollmentConfigurationItem[] configurationItems)
        {
            if (enrollingTenant == null)
            {
                throw new ArgumentNullException(nameof(enrollingTenant));
            }

            if (serviceTenant == null)
            {
                throw new ArgumentNullException(nameof(serviceTenant));
            }

            if (configurationItems == null)
            {
                throw new ArgumentNullException(nameof(configurationItems));
            }

            // First we need to ensure that all the required config items for both the service being enrolled in,
            // as well as any dependent services is provided.
            ServiceManifestRequiredConfigurationEntry[] requiredConfig = await this.GetServiceEnrollmentConfigurationRequirementsAsync(serviceTenant).ConfigureAwait(false);
            configurationItems.ValidateAndThrow(requiredConfig);

            ServiceManifest manifest = serviceTenant.GetServiceManifest();

            // Now, match up the required config items for this service to the relevent supplied config (we may
            // have been supplied with config for dependent services as well, so we can't just attach all
            // of the supplied config items to the enrolling tenant - some of it could belong on delegated
            // tenants.
            IEnumerable<(ServiceManifestRequiredConfigurationEntry RequiredConfigurationEntry, EnrollmentConfigurationItem ProvidedConfigurationItem)> matchedConfigItems =
                requiredConfig.Select(
                    requiredConfigItem =>
                    (requiredConfigItem, configurationItems.Single(item => item.Key == requiredConfigItem.Key)));

            foreach ((ServiceManifestRequiredConfigurationEntry RequiredConfigurationEntry, EnrollmentConfigurationItem ProvidedConfigurationItem) current in matchedConfigItems)
            {
                current.ProvidedConfigurationItem.AddToTenant(enrollingTenant, current.RequiredConfigurationEntry);
            }

            // Add an enrollment entry to the tenant.
            enrollingTenant.AddServiceEnrollment(serviceTenant.Id);

            // If this service has dependencies, we need to create a new delegated tenant for the service to use when
            // accessing those dependencies.
            if (manifest.DependsOnServiceNames.Count > 0)
            {
                ITenant delegatedTenant = await this.CreateDelegatedTenant(enrollingTenant, serviceTenant).ConfigureAwait(false);

                // Now enroll the new delegated tenant for all of the dependent services.
                await Task.WhenAll(manifest.DependsOnServiceNames.Select(
                    dependsOnServiceName => this.EnrollInServiceAsync(
                        delegatedTenant,
                        dependsOnServiceName,
                        configurationItems))).ConfigureAwait(false);

                // Add the delegated tenant Id to the enrolling tenant
                enrollingTenant.SetDelegatedTenantIdForService(serviceTenant.Id, delegatedTenant.Id);
            }

            await this.tenantProvider.UpdateTenantAsync(enrollingTenant).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<ServiceManifestRequiredConfigurationEntry[]> GetServiceEnrollmentConfigurationRequirementsAsync(string serviceTenantName)
        {
            if (string.IsNullOrWhiteSpace(serviceTenantName))
            {
                throw new ArgumentException(nameof(serviceTenantName));
            }

            ITenant serviceTenant = await this.GetServiceTenantByNameAsync(serviceTenantName).ConfigureAwait(false)
                ?? throw new TenantNotFoundException($"Could not find a service tenant with the name '{serviceTenantName}'");

            return await this.GetServiceEnrollmentConfigurationRequirementsAsync(serviceTenant).ConfigureAwait(false);
        }

        private async Task<ServiceManifestRequiredConfigurationEntry[]> GetServiceEnrollmentConfigurationRequirementsAsync(ITenant serviceTenant)
        {
            var requirements = new List<ServiceManifestRequiredConfigurationEntry>();
            ServiceManifest serviceManifest = serviceTenant.GetServiceManifest();
            requirements.AddRange(serviceManifest.RequiredConfigurationEntries);

            ServiceManifestRequiredConfigurationEntry[][] dependentServicesConfigRequirements =
                await Task.WhenAll(
                    serviceManifest.DependsOnServiceNames.Select(
                        x => this.GetServiceEnrollmentConfigurationRequirementsAsync(x))).ConfigureAwait(false);

            requirements.AddRange(dependentServicesConfigRequirements.SelectMany(x => x));

            return requirements.ToArray();
        }

        private Task<ITenant> CreateDelegatedTenant(ITenant accessingTenant, ITenant serviceTenant)
        {
            string delegatedTenantName = TenantNames.DelegatedTenant(serviceTenant.Name, accessingTenant.Name);
            return this.tenantProvider.CreateChildTenantAsync(serviceTenant.Id, delegatedTenantName);
        }

        private Task<ITenant?> GetClientTenantParentAsync() =>
            this.GetTenantByNameAsync(this.tenantProvider.Root.Id, TenantNames.ClientTenantParent);

        private Task<ITenant?> GetServiceTenantParentAsync() =>
            this.GetTenantByNameAsync(this.tenantProvider.Root.Id, TenantNames.ServiceTenantParent);

        private async Task<ITenant?> GetTenantByNameAsync(string parentTenantId, string name)
        {
            await foreach (string childTenantId in this.tenantProvider.EnumerateAllChildrenAsync(parentTenantId))
            {
                ITenant tenant = await this.tenantProvider.GetTenantAsync(childTenantId).ConfigureAwait(false);
                if (tenant.Name == name)
                {
                    return tenant;
                }
            }

            return null;
        }

        [DoesNotReturn]
        private void ThrowNotInitialisedException()
        {
            throw new InvalidOperationException("The underlying tenant provider has not been initialised for use with Marain. Please call the ITenantManagementService.InitialiseTenancyProviderAsync before attempting to create tenants.");
        }
    }
}
