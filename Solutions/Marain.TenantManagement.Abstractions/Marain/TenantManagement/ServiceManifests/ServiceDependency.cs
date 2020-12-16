// <copyright file="ServiceDependency.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Corvus.Tenancy.Exceptions;
    using Marain.TenantManagement.Exceptions;

    /// <summary>
    /// Represents a dependency of a service on another service.
    /// </summary>
    public class ServiceDependency
    {
        /// <summary>
        /// Gets or sets the Id of the dependency's Service Tenant.
        /// </summary>
#nullable disable annotations
        public string Id { get; set; }
#nullable restore annotations

        /// <summary>
        /// Gets or sets the expected name of the dependency's Service Tenant.
        /// </summary>
        /// <remarks>
        /// This field is not mandatory, but if present it is expected to match the name of the tenant whose Id is specified
        /// in the <see cref="Id"/> property.
        /// </remarks>
        public string? ExpectedName { get; set; }

        /// <summary>
        /// Validates the dependency using the supplied <see cref="ITenantStore"/>.
        /// </summary>
        /// <param name="tenantStore">
        /// The <see cref="ITenantStore"/> that will be used to retrieve and check dependencies.
        /// </param>
        /// <param name="messagePrefix">
        /// A prefix to add to all error messages. This can be used to identify the dependency within a list.
        /// </param>
        /// <returns>
        /// A list of validation errors detected. If there are no errors, the list will be empty.
        /// </returns>
        public async Task<IList<string>> ValidateAsync(
            ITenantStore tenantStore,
            string messagePrefix)
        {
            if (tenantStore == null)
            {
                throw new ArgumentNullException(nameof(tenantStore));
            }

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(this.Id))
            {
                errors.Add($"{messagePrefix}: The Id on this dependency is not set or is invalid. Id must be at least one non-whitespace character.");
            }

            ITenant dependentTenant;

            try
            {
                dependentTenant = await tenantStore.GetServiceTenantAsync(this.Id).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(this.ExpectedName) && dependentTenant.Name != this.ExpectedName)
                {
                    errors.Add($"{messagePrefix}: The dependency with Id '{this.Id}' was expected to be called '{this.ExpectedName}' but is actually called '{dependentTenant.Name}'. When specified, the expected name must match the tenant name.");
                }
            }
            catch (TenantNotFoundException)
            {
                errors.Add($"{messagePrefix}: Could not find a tenant with Id '{this.Id}'");
            }
            catch (InvalidMarainTenantTypeException ex)
            {
                errors.Add($"{messagePrefix}: {ex.Message}");
            }

            return errors;
        }
    }
}
