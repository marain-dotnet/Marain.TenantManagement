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
            IList<string> errors = await manifest.ValidateAsync(tenantManagementService).ConfigureAwait(false);

            if (errors.Count > 0)
            {
                throw new InvalidServiceManifestException(errors);
            }
        }
    }
}
