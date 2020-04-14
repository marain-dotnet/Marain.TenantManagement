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
    using Corvus.Tenancy.Exceptions;

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
        /// Gets or sets the well known Guid that will be used when creating the service tenant.
        /// </summary>
        public Guid WellKnownTenantGuid { get; set; }

        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
#nullable disable annotations
        public string ServiceName { get; set; }
#nullable restore annotations

        /// <summary>
        /// Gets a list of the names of other Service Tenants whose services this depends upon.
        /// </summary>
        public IList<string> DependsOnServiceTenantIds { get; } = new List<string>();

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

            if (string.IsNullOrWhiteSpace(this.ServiceName))
            {
                errors.Add("The ServiceName on the this is not set or is invalid. ServiceName must be at least one non-whitespace character.");
            }

            await Task.WhenAll(
                this.ValidateWellKnownTenantId(tenantManagementService, errors),
                this.VerifyDependenciesExistAsync(tenantManagementService, errors)).ConfigureAwait(false);

            // TODO: Ensure there aren't multiple items with the same key.
            IEnumerable<string> configErrors = this.RequiredConfigurationEntries.SelectMany((c, i) => c.Validate($"RequiredConfigurationEntries[{i}]"));
            foreach (string configError in configErrors)
            {
                errors.Add(configError);
            }

            return new List<string>(errors);
        }

        private async Task ValidateWellKnownTenantId(
            ITenantManagementService tenantManagementService,
            ConcurrentBag<string> errors)
        {
            if (this.WellKnownTenantGuid == Guid.Empty)
            {
                errors.Add("A value must be supplied for the Well Known Tenant Guid. This is necessary to ensure that the Id of the service tenant is well known.");
            }

            string expectedChildTenantId = WellKnownTenantIds.ServiceTenantParentId.CreateChildId(this.WellKnownTenantGuid);

            try
            {
                // See if the tenant already exists...
                ITenant existingTenant = await tenantManagementService.GetServiceTenantAsync(expectedChildTenantId).ConfigureAwait(false);

                errors.Add($"A service tenant with well-known GUID '{this.WellKnownTenantGuid}' (resulting in Id '{expectedChildTenantId}') called '{existingTenant.Name}' already exists. All tenants must have unique well-known tenant GUIDs/IDs.");
            }
            catch (TenantNotFoundException)
            {
                // This is fine. The tenant doesn't already exist, so we can continue.
                // TODO: Catch scenario where the tenant exists but isn't a service tenant.
            }
        }

        private async Task VerifyDependenciesExistAsync(
            ITenantManagementService tenantManagementService,
            ConcurrentBag<string> errors)
        {
            if (this.DependsOnServiceTenantIds.Count == 0)
            {
                return;
            }

            Task<ITenant>[] dependentServiceTenantRequests =
                this.DependsOnServiceTenantIds.Select(tenantManagementService.GetServiceTenantAsync).ToArray();

            try
            {
                await Task.WhenAll(dependentServiceTenantRequests).ConfigureAwait(false);
            }
            catch (Exception)
            {
                for (int i = 0; i < dependentServiceTenantRequests.Length; i++)
                {
                    if (dependentServiceTenantRequests[i].Exception != null)
                    {
                        if (dependentServiceTenantRequests[i].Exception!.InnerException is TenantNotFoundException)
                        {
                            errors.Add($"The manifest contains a dependency with Id '{this.DependsOnServiceTenantIds[i]}', but no Service Tenant with that Id exists.");
                        }
                        else
                        {
                            errors.Add($"An unexpected exception occurred when trying to verify the dependency with Id '{this.DependsOnServiceTenantIds[i]}': {dependentServiceTenantRequests[i].Exception}");
                        }
                    }
                }
            }
        }
    }
}
