// <copyright file="ServiceManifestExtensions.cs" company="Endjin Limited">
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

    using Marain.TenantManagement.Exceptions;

    /// <summary>
    /// Extension methods for the <see cref="ServiceManifest"/> class.
    /// </summary>
    public static class ServiceManifestExtensions
    {
        /// <summary>
        /// Validates the manifest using the supplied <see cref="ITenantStore"/>.
        /// </summary>
        /// <param name="manifest">The manifest to validate.</param>
        /// <param name="tenantStore">
        /// The <see cref="ITenantStore"/> that will be used to validate names and dependencies.
        /// </param>
        /// <returns>
        /// A list of validation errors detected. If there are no errors, the list will be empty.
        /// </returns>
        public static async Task<IList<string>> ValidateAsync(
            this ServiceManifest manifest,
            ITenantStore tenantStore)
        {
            ArgumentNullException.ThrowIfNull(tenantStore);

            var errors = new ConcurrentBag<string>();

            if (string.IsNullOrWhiteSpace(manifest.ServiceName))
            {
                errors.Add("The ServiceName on the this is not set or is invalid. ServiceName must be at least one non-whitespace character.");
            }

            await Task.WhenAll(
                manifest.ValidateWellKnownTenantId(tenantStore, errors),
                manifest.VerifyDependenciesAsync(tenantStore, errors)).ConfigureAwait(false);

            // TODO: Ensure there aren't multiple items with the same key.
            IEnumerable<string> configErrors = manifest.RequiredConfigurationEntries.SelectMany((c, i) => c.Validate($"RequiredConfigurationEntries[{i}]"));
            foreach (string configError in configErrors)
            {
                errors.Add(configError);
            }

            return new List<string>(errors);
        }

        /// <summary>
        /// Validates the <see cref="ServiceManifest"/> and throws an <see cref="InvalidServiceManifestException"/> if
        /// any errors are encountered.
        /// </summary>
        /// <param name="manifest">The manifest to validate.</param>
        /// <param name="tenantStore">The <see cref="ITenantStore"/> that will be used when
        /// validating the manifest.</param>
        /// <returns>A task representing the asynchronous operation. If it completes successfully, the manifest is valid.</returns>
        /// <exception cref="InvalidServiceManifestException">The service manifest contains one or more errors.</exception>
        public static async Task ValidateAndThrowAsync(
            this ServiceManifest manifest,
            ITenantStore tenantStore)
        {
            ArgumentNullException.ThrowIfNull(tenantStore);

            IList<string> errors = await manifest.ValidateAsync(tenantStore).ConfigureAwait(false);

            if (errors.Count > 0)
            {
                throw new InvalidServiceManifestException(errors);
            }
        }

        private static async Task ValidateWellKnownTenantId(
            this ServiceManifest manifest,
            ITenantStore tenantStore,
            ConcurrentBag<string> errors)
        {
            if (manifest.WellKnownTenantGuid == Guid.Empty)
            {
                errors.Add("A value must be supplied for the Well Known Tenant Guid. This is necessary to ensure that the Id of the service tenant is well known.");
            }

            string expectedChildTenantId = WellKnownTenantIds.ServiceTenantParentId.CreateChildId(manifest.WellKnownTenantGuid);

            try
            {
                // See if the tenant already exists...
                ITenant existingTenant = await tenantStore.GetServiceTenantAsync(expectedChildTenantId).ConfigureAwait(false);

                errors.Add($"A service tenant with well-known GUID '{manifest.WellKnownTenantGuid}' (resulting in Id '{expectedChildTenantId}') called '{existingTenant.Name}' already exists. All tenants must have unique well-known tenant GUIDs/IDs.");
            }
            catch (TenantNotFoundException)
            {
                // This is fine. The tenant doesn't already exist, so we can continue.
            }
        }

        private static async Task VerifyDependenciesAsync(
            this ServiceManifest manifest,
            ITenantStore tenantStore,
            ConcurrentBag<string> errors)
        {
            if (manifest.DependsOnServiceTenants.Count == 0)
            {
                return;
            }

            // Ensure there aren't any duplicates
            string[] duplicateIds = manifest.DependsOnServiceTenants.GroupBy(x => x.Id).Where(x => x.Count() > 1).Select(x => x.Key).ToArray();

            if (duplicateIds.Length > 0)
            {
                string mergedIds = string.Join(", ", duplicateIds.Select(x => $"'{x}'"));
                errors.Add($"The following tenant Ids appear more than once in the DependsOnServiceTenants list: [{mergedIds}]. Each depended-upon service should appear only once.");
            }

            IList<string>[] dependencyErrors = await Task.WhenAll(
                manifest.DependsOnServiceTenants.Select(
                    (dependency, index) => dependency.ValidateAsync(tenantStore, $"DependsOnServiceTenants[{index}]"))).ConfigureAwait(false);

            foreach (string dependencyError in dependencyErrors.SelectMany(x => x))
            {
                errors.Add(dependencyError);
            }
        }
    }
}