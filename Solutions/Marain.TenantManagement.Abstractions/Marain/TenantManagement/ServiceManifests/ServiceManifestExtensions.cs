// <copyright file="ServiceManifestExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Marain.TenantManagement.Exceptions;

    /// <summary>
    /// Extension methods for the <see cref="ServiceManifest"/> class.
    /// </summary>
    public static class ServiceManifestExtensions
    {
        /// <summary>
        /// Validates the <see cref="ServiceManifest"/> and throws an <see cref="InvalidServiceManifestException"/> if
        /// any errors are encountered.
        /// </summary>
        /// <param name="manifest">The manifest to validate.</param>
        /// <param name="tenantManagementService">The <see cref="ITenantManagementService"/> that will be used when
        /// validating the manifest.</param>
        /// <returns>A task representing the asynchronous operation. If it completes successfully, the manifest is valid.</returns>
        public static async Task ValidateAndThrowAsync(
            this ServiceManifest manifest,
            ITenantManagementService tenantManagementService)
        {
            if (tenantManagementService == null)
            {
                throw new System.ArgumentNullException(nameof(tenantManagementService));
            }

            var errors = new ConcurrentBag<string>();

            await Task.WhenAll(
                manifest.ValidateServiceNameAsync(tenantManagementService, errors),
                manifest.ValidateDependenciesExistAsync(tenantManagementService, errors)).ConfigureAwait(false);

            if (errors.Count > 0)
            {
                throw new InvalidServiceManifestException(errors);
            }
        }

        private static async Task ValidateServiceNameAsync(
            this ServiceManifest manifest,
            ITenantManagementService tenantManagementService,
            ConcurrentBag<string> errors)
        {
            ITenant? existingTenantWithSameName =
                await tenantManagementService.GetServiceTenantByNameAsync(manifest.ServiceName).ConfigureAwait(false);

            if (existingTenantWithSameName != null)
            {
                errors.Add($"A Service Tenant called '{manifest.ServiceName}' already exists.");
            }
        }

        private static async Task ValidateDependenciesExistAsync(
            this ServiceManifest manifest,
            ITenantManagementService tenantManagementService,
            ConcurrentBag<string> errors)
        {
            if (manifest.DependsOnServiceNames.Count == 0)
            {
                return;
            }

            IEnumerable<Task<ITenant?>> dependentServiceTenantRequests =
                manifest.DependsOnServiceNames.Select(tenantManagementService.GetServiceTenantByNameAsync);

            ITenant[] dependentServiceTenants = await Task.WhenAll(dependentServiceTenantRequests).ConfigureAwait(false);

            for (int index = 0; index < manifest.DependsOnServiceNames.Count; index++)
            {
                if (dependentServiceTenants[index] == null)
                {
                    errors.Add($"The manifest contains a dependency called '{manifest.DependsOnServiceNames[index]}', but no Service Tenant with that name exists.");
                }
            }
        }
    }
}
