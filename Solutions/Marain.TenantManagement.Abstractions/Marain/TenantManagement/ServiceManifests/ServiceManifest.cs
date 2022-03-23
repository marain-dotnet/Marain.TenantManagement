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
        /// Gets a list of other Service Tenants whose services this depends upon.
        /// </summary>
        public IList<ServiceDependency> DependsOnServiceTenants { get; } = new List<ServiceDependency>();

        /// <summary>
        /// Gets the list of configuration items required when enrolling a tenant to this service.
        /// </summary>
        public IList<ServiceManifestRequiredConfigurationEntry> RequiredConfigurationEntries { get; } = new List<ServiceManifestRequiredConfigurationEntry>();

        /// <summary>
        /// Validates the manifest using the supplied <see cref="ITenantStore"/>.
        /// </summary>
        /// <param name="tenantStore">
        /// The <see cref="ITenantStore"/> that will be used to validate names and dependencies.
        /// </param>
        /// <returns>
        /// A list of validation errors detected. If there are no errors, the list will be empty.
        /// </returns>
        public async Task<IList<string>> ValidateAsync(
            ITenantStore tenantStore)
        {
            if (tenantStore == null)
            {
                throw new ArgumentNullException(nameof(tenantStore));
            }

            var errors = new ConcurrentBag<string>();

            if (string.IsNullOrWhiteSpace(this.ServiceName))
            {
                errors.Add("The ServiceName on the this is not set or is invalid. ServiceName must be at least one non-whitespace character.");
            }

            await Task.WhenAll(
                this.ValidateWellKnownTenantId(tenantStore, errors),
                this.VerifyDependenciesAsync(tenantStore, errors)).ConfigureAwait(false);

            // TODO: Ensure there aren't multiple items with the same key.
            IEnumerable<string> configErrors = this.RequiredConfigurationEntries.SelectMany((c, i) => c.Validate($"RequiredConfigurationEntries[{i}]"));
            foreach (string configError in configErrors)
            {
                errors.Add(configError);
            }

            return new List<string>(errors);
        }

        private async Task ValidateWellKnownTenantId(
            ITenantStore tenantStore,
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
                ITenant existingTenant = await tenantStore.GetServiceTenantAsync(expectedChildTenantId).ConfigureAwait(false);

                errors.Add($"A service tenant with well-known GUID '{this.WellKnownTenantGuid}' (resulting in Id '{expectedChildTenantId}') called '{existingTenant.Name}' already exists. All tenants must have unique well-known tenant GUIDs/IDs.");
            }
            catch (TenantNotFoundException)
            {
                // This is fine. The tenant doesn't already exist, so we can continue.
            }
        }

        private async Task VerifyDependenciesAsync(
            ITenantStore tenantStore,
            ConcurrentBag<string> errors)
        {
            if (this.DependsOnServiceTenants.Count == 0)
            {
                return;
            }

            // Ensure there aren't any duplicates
            string[] duplicateIds = this.DependsOnServiceTenants.GroupBy(x => x.Id).Where(x => x.Count() > 1).Select(x => x.Key).ToArray();

            if (duplicateIds.Length > 0)
            {
                string mergedIds = string.Join(", ", duplicateIds.Select(x => $"'{x}'"));
                errors.Add($"The following tenant Ids appear more than once in the DependsOnServiceTenants list: [{mergedIds}]. Each depended-upon service should appear only once.");
            }

            IList<string>[] dependencyErrors = await Task.WhenAll(
                this.DependsOnServiceTenants.Select(
                    (dependency, index) => dependency.ValidateAsync(tenantStore, $"DependsOnServiceTenants[{index}]"))).ConfigureAwait(false);

            foreach (string dependencyError in dependencyErrors.SelectMany(x => x))
            {
                errors.Add(dependencyError);
            }
        }
    }
}