// <copyright file="ServiceManifest.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Tenancy;

    /// <summary>
    /// Manifest for a Marain service, needed when onboarding a new tenant to use that service.
    /// </summary>
    public class ServiceManifest
    {
        /// <summary>
        /// The content type of the manifest.
        /// </summary>
        public const string RegisteredContentType = "application/vnd.marain.tenancy.servicemanifests.servicemanifest";

        /// <summary>
        /// Gets the content type of the Service Manifest.
        /// </summary>
        public string ContentType => RegisteredContentType;

        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
#nullable disable annotations
        public string ServiceName { get; set; }
#nullable restore annotations

        /// <summary>
        /// Gets a list of the names of other Service Tenants whose services this depends upon.
        /// </summary>
        public IList<string> DependsOnServiceNames { get; } = new List<string>();

        /// <summary>
        /// Gets the list of configuration items required when enrolling a tenant to this service.
        /// </summary>
        public IList<ServiceManifestRequiredConfigurationEntry> RequiredConfigurationEntries { get; } = new List<ServiceManifestRequiredConfigurationEntry>();

        /// <summary>
        /// Validates the manifest using the supplied <see cref="ITenantManagementService"/>.
        /// </summary>
        /// <param name="tenantManagementService">
        /// The <see cref="ITenantManagementService"/> that will be used to validate names and dependencies.
        /// </param>
        /// <returns>
        /// A list of validation errors detected. If there are no errors, the list will be empty.
        /// </returns>
        public async Task<IList<string>> ValidateAsync(
            ITenantManagementService tenantManagementService)
        {
            if (tenantManagementService == null)
            {
                throw new ArgumentNullException(nameof(tenantManagementService));
            }

            var errors = new ConcurrentBag<string>();

            await Task.WhenAll(
                this.ValidateServiceNameAsync(tenantManagementService, errors),
                this.VerifyDependenciesExistAsync(tenantManagementService, errors)).ConfigureAwait(false);

            // TODO: Ensure there aren't multiple items with the same key.
            IEnumerable<string> configErrors = this.RequiredConfigurationEntries.SelectMany((c, i) => c.Validate($"RequiredConfigurationEntries[{i}]"));
            foreach (string configError in configErrors)
            {
                errors.Add(configError);
            }

            return new List<string>(errors);
        }

        private async Task ValidateServiceNameAsync(
            ITenantManagementService tenantManagementService,
            ConcurrentBag<string> errors)
        {
            if (string.IsNullOrWhiteSpace(this.ServiceName))
            {
                errors.Add("The ServiceName on the this is not set or is invalid. ServiceName must be at least one non-whitespace character.");
            }
            else
            {
                ITenant? existingTenantWithSameName =
                    await tenantManagementService.GetServiceTenantByNameAsync(this.ServiceName).ConfigureAwait(false);

                if (existingTenantWithSameName != null)
                {
                    errors.Add($"A Service Tenant called '{this.ServiceName}' already exists.");
                }
            }
        }

        private async Task VerifyDependenciesExistAsync(
            ITenantManagementService tenantManagementService,
            ConcurrentBag<string> errors)
        {
            if (this.DependsOnServiceNames.Count == 0)
            {
                return;
            }

            IEnumerable<Task<ITenant?>> dependentServiceTenantRequests =
                this.DependsOnServiceNames.Select(tenantManagementService.GetServiceTenantByNameAsync);

            ITenant[] dependentServiceTenants = await Task.WhenAll(dependentServiceTenantRequests).ConfigureAwait(false);

            for (int index = 0; index < this.DependsOnServiceNames.Count; index++)
            {
                if (dependentServiceTenants[index] == null)
                {
                    errors.Add($"The manifest contains a dependency called '{this.DependsOnServiceNames[index]}', but no Service Tenant with that name exists.");
                }
            }
        }
    }
}
