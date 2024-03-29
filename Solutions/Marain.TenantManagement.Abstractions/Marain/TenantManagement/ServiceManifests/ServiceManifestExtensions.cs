﻿// <copyright file="ServiceManifestExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using System;
    using System.Collections.Generic;
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
    }
}