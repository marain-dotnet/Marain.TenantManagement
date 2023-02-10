// <copyright file="ServiceDependencyExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Corvus.Tenancy;
using Corvus.Tenancy.Exceptions;

using Marain.TenantManagement.Exceptions;

/// <summary>
/// Extension methods for <see cref="ServiceDependency"/>.
/// </summary>
public static class ServiceDependencyExtensions
{
    /// <summary>
    /// Validates the dependency using the supplied <see cref="ITenantStore"/>.
    /// </summary>
    /// <param name="serviceDependency">The service dependency to validate.</param>
    /// <param name="tenantStore">
    /// The <see cref="ITenantStore"/> that will be used to retrieve and check dependencies.
    /// </param>
    /// <param name="messagePrefix">
    /// A prefix to add to all error messages. This can be used to identify the dependency within a list.
    /// </param>
    /// <returns>
    /// A list of validation errors detected. If there are no errors, the list will be empty.
    /// </returns>
    public static async Task<IList<string>> ValidateAsync(
        this ServiceDependency serviceDependency,
        ITenantStore tenantStore,
        string messagePrefix)
    {
        ArgumentNullException.ThrowIfNull(tenantStore);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(serviceDependency.Id))
        {
            errors.Add($"{messagePrefix}: The Id on this dependency is not set or is invalid. Id must be at least one non-whitespace character.");
        }

        ITenant dependentTenant;

        try
        {
            dependentTenant = await tenantStore.GetServiceTenantAsync(serviceDependency.Id).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(serviceDependency.ExpectedName) && dependentTenant.Name != serviceDependency.ExpectedName)
            {
                errors.Add($"{messagePrefix}: The dependency with Id '{serviceDependency.Id}' was expected to be called '{serviceDependency.ExpectedName}' but is actually called '{dependentTenant.Name}'. When specified, the expected name must match the tenant name.");
            }
        }
        catch (TenantNotFoundException)
        {
            errors.Add($"{messagePrefix}: Could not find a tenant with Id '{serviceDependency.Id}'");
        }
        catch (InvalidMarainTenantTypeException ex)
        {
            errors.Add($"{messagePrefix}: {ex.Message}");
        }

        return errors;
    }
}