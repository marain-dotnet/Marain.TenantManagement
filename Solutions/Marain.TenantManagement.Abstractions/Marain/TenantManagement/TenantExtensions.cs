// <copyright file="TenantExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Corvus.Tenancy;
    using Marain.TenantManagement.Exceptions;
    using Marain.TenantManagement.Internal;
    using Marain.TenantManagement.ServiceManifests;

    /// <summary>
    /// Extension methods for <see cref="ITenant"/>s.
    /// </summary>
    public static class TenantExtensions
    {
        /// <summary>
        /// Checks that the specified tenant is one of the allowable types.
        /// </summary>
        /// <param name="tenant">The tenant to check.</param>
        /// <param name="allowableTenantTypes">The list of allowable types for the tenant.</param>
        /// <returns>A boolean indicating whether or not the tenant is one of the specified types.</returns>
        public static bool IsTenantOfType(this ITenant tenant, params MarainTenantType[] allowableTenantTypes)
        {
            MarainTenantType tenantType = tenant.GetMarainTenantType();
            return allowableTenantTypes.Contains(tenantType);
        }

        /// <summary>
        /// Checks that the specified tenant is one of the allowable types and throws a
        /// <see cref="InvalidMarainTenantTypeException"/> if it is not.
        /// </summary>
        /// <param name="tenant">The tenant to check.</param>
        /// <param name="allowableTenantTypes">The list of allowable types for the tenant.</param>
        /// <exception cref="InvalidMarainTenantTypeException">The type is not one of the specified types.</exception>
        public static void EnsureTenantIsOfType(this ITenant tenant, params MarainTenantType[] allowableTenantTypes)
        {
            if (!tenant.IsTenantOfType(allowableTenantTypes))
            {
                throw new InvalidMarainTenantTypeException(tenant.Id, tenant.GetMarainTenantType(), allowableTenantTypes);
            }
        }

        /// <summary>
        /// Gets the <see cref="ServiceManifest"/> from the specified tenant.
        /// </summary>
        /// <param name="tenant">The tenant to get the manifest from.</param>
        /// <returns>The Service Manifest.</returns>
        public static ServiceManifest GetServiceManifest(this ITenant tenant)
        {
            if (tenant.Properties.TryGet(TenantPropertyKeys.ServiceManifest, out ServiceManifest manifest))
            {
                return manifest;
            }

            throw new InvalidOperationException($"The tenant '{tenant.Name}' with Id '{tenant.Id}' does not have a Service Manifest");
        }

        /// <summary>
        /// Checks to see if the specified tenant is enrolled for the service with the provided Id.
        /// </summary>
        /// <param name="tenant">The tenant to check enrollment for.</param>
        /// <param name="serviceTenantId">The service to see if they are enrolled in.</param>
        /// <returns>True if the tenant is enrolled for the service, false otherwise.</returns>
        public static bool IsEnrolledForService(this ITenant tenant, string serviceTenantId)
        {
            if (serviceTenantId == null)
            {
                throw new ArgumentNullException(nameof(serviceTenantId));
            }

            if (string.IsNullOrWhiteSpace(serviceTenantId))
            {
                throw new ArgumentException(nameof(serviceTenantId));
            }

            if (tenant.Properties.TryGet(TenantPropertyKeys.Enrollments, out IList<string> enrollments))
            {
                return enrollments.Contains(serviceTenantId);
            }

            // TODO: Should we throw an exception if the tenant is a service tenant? How would we know?
            return false;
        }

        /// <summary>
        /// Gets the Id of the delegated tenant that has been created for a service to use when accessing a dependent service
        /// on the tenant's behalf.
        /// </summary>
        /// <param name="tenant">
        /// The tenant who is able to make calls to the service represented by the specified Service Tenant.
        /// </param>
        /// <param name="serviceTenantId">The Id of the Service Tenant which will uses the Delegated Tenant.</param>
        /// <returns>The Id of the delegated tenant.</returns>
        public static string GetDelegatedTenantIdForServiceId(this ITenant tenant, string serviceTenantId)
        {
            tenant.EnsureTenantIsOfType(MarainTenantType.Client, MarainTenantType.Delegated);

            if (string.IsNullOrWhiteSpace(serviceTenantId))
            {
                throw new ArgumentException(nameof(serviceTenantId));
            }

            if (tenant.Properties.TryGet(TenantPropertyKeys.DelegatedTenantId(serviceTenantId), out string delegatedTenantId))
            {
                if (!string.IsNullOrEmpty(delegatedTenantId))
                {
                    return delegatedTenantId;
                }
            }

            throw new ArgumentException($"Tenant '{tenant.Name}' with Id '{tenant.Id}' does not contain a delegated tenant Id for service tenant with Id '{serviceTenantId}'");
        }

        /// <summary>
        /// Gets the list of enrollments for the tenant.
        /// </summary>
        /// <param name="tenant">The tenant to get enrollments for.</param>
        /// <returns>The list of enrollments for the tenant.</returns>
        public static IEnumerable<string> GetEnrollments(this ITenant tenant)
        {
            if (tenant.Properties.TryGet(TenantPropertyKeys.Enrollments, out IList<string> enrollments))
            {
                return enrollments;
            }

            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Gets the Marain tenant type of the tenant.
        /// </summary>
        /// <param name="tenant">The tenant to get the type of.</param>
        /// <returns>
        /// The <see cref="MarainTenantType"/> of the tenant, or <see cref="MarainTenantType.Undefined"/> if not set.
        /// </returns>
        public static MarainTenantType GetMarainTenantType(this ITenant tenant)
        {
            if (tenant.Properties.TryGet(TenantPropertyKeys.MarainTenantType, out MarainTenantType tenantType))
            {
                return tenantType;
            }

            return MarainTenantType.Undefined;
        }

        /// <summary>
        /// Adds the given <see cref="ServiceManifest"/> to the tenant's property bag.
        /// </summary>
        /// <param name="tenant">The tenant to add to.</param>
        /// <param name="manifest">The manifest to add.</param>
        /// <remarks>This method does not persist the tenant. Calling code should do this once all changes have been made.</remarks>
        internal static void SetServiceManifest(this ITenant tenant, ServiceManifest manifest)
        {
            if (manifest == null)
            {
                throw new ArgumentNullException(nameof(manifest));
            }

            tenant.Properties.Set(TenantPropertyKeys.ServiceManifest, manifest);
        }

        /// <summary>
        /// Adds a service tenant Id to the list of enrollments for the given tenant.
        /// </summary>
        /// <param name="tenant">The tenant being enrolled.</param>
        /// <param name="serviceTenantId">The Service Tenant Id of the service being enrolled in.</param>
        /// <remarks>This method does not persist the tenant. Calling code should do this once all changes have been made.</remarks>
        internal static void AddServiceEnrollment(this ITenant tenant, string serviceTenantId)
        {
            if (serviceTenantId == null)
            {
                throw new ArgumentNullException(nameof(serviceTenantId));
            }

            if (string.IsNullOrWhiteSpace(serviceTenantId))
            {
                throw new ArgumentException(nameof(serviceTenantId));
            }

            // TODO: We should check that the tenant is allowed to enroll (i.e they are not either the root tenant or a
            // service tenant.
            if (!tenant.Properties.TryGet(TenantPropertyKeys.Enrollments, out IList<string> enrollments))
            {
                enrollments = new List<string>();
            }

            if (enrollments.Contains(serviceTenantId))
            {
                throw new InvalidOperationException($"The tenant named '{tenant.Name}' with Id '{tenant.Id}' is already enrolled for service Id '{serviceTenantId}'");
            }

            enrollments.Add(serviceTenantId);

            tenant.Properties.Set(TenantPropertyKeys.Enrollments, enrollments);
        }

        /// <summary>
        /// Removes a service tenant Id from the list of enrollments for the given tenant.
        /// </summary>
        /// <param name="tenant">The tenant being unenrolled.</param>
        /// <param name="serviceTenantId">The Service Tenant Id of the service being unenrolled from.</param>
        /// <remarks>This method does not persist the tenant. Calling code should do this once all changes have been made.</remarks>
        internal static void RemoveServiceEnrollment(this ITenant tenant, string serviceTenantId)
        {
            if (serviceTenantId == null)
            {
                throw new ArgumentNullException(nameof(serviceTenantId));
            }

            if (string.IsNullOrWhiteSpace(serviceTenantId))
            {
                throw new ArgumentException(nameof(serviceTenantId));
            }

            if (tenant.Properties.TryGet(TenantPropertyKeys.Enrollments, out IList<string> enrollments))
            {
                if (enrollments.Contains(serviceTenantId))
                {
                    enrollments.Remove(serviceTenantId);

                    tenant.Properties.Set(TenantPropertyKeys.Enrollments, enrollments);

                    return;
                }
            }

            throw new InvalidOperationException($"The tenant named '{tenant.Name}' with Id '{tenant.Id}' is not enrolled for service Id '{serviceTenantId}'");
        }

        /// <summary>
        /// Stores the Id of a delegated tenant that has been created for a service to use when accessing a dependent service
        /// on the tenant's behalf.
        /// </summary>
        /// <param name="tenant">
        /// The tenant who is able to make calls to the service represented by the specified Service Tenant.
        /// </param>
        /// <param name="serviceTenant">The Service Tenant which will be using the Delegated Tenant.</param>
        /// <param name="delegatedTenant">The tenant that has been created for the service to use.</param>
        /// <remarks>This method does not persist the tenant. Calling code should do this once all changes have been made.</remarks>
        internal static void SetDelegatedTenantForService(
            this ITenant tenant,
            ITenant serviceTenant,
            ITenant delegatedTenant)
        {
            tenant.EnsureTenantIsOfType(MarainTenantType.Client, MarainTenantType.Delegated);

            if (serviceTenant == null)
            {
                throw new ArgumentNullException(nameof(serviceTenant));
            }

            serviceTenant.EnsureTenantIsOfType(MarainTenantType.Service);

            if (delegatedTenant == null)
            {
                throw new ArgumentNullException(nameof(delegatedTenant));
            }

            delegatedTenant.EnsureTenantIsOfType(MarainTenantType.Delegated);

            tenant.Properties.Set(TenantPropertyKeys.DelegatedTenantId(serviceTenant.Id), delegatedTenant.Id);
        }

        /// <summary>
        /// Clears the delegated Id for a specific tenant and service.
        /// </summary>
        /// <param name="tenant">
        /// The tenant who was able to make calls to the service represented by the specified Service Tenant.
        /// </param>
        /// <param name="serviceTenant">The Service Tenant which was using the Delegated Tenant.</param>
        /// <remarks>This method does not persist the tenant. Calling code should do this once all changes have been made.</remarks>
        internal static void ClearDelegatedTenantForService(
            this ITenant tenant,
            ITenant serviceTenant)
        {
            tenant.EnsureTenantIsOfType(MarainTenantType.Client, MarainTenantType.Delegated);

            if (serviceTenant == null)
            {
                throw new ArgumentNullException(nameof(serviceTenant));
            }

            serviceTenant.EnsureTenantIsOfType(MarainTenantType.Service);

            tenant.Properties.Set(TenantPropertyKeys.DelegatedTenantId(serviceTenant.Id), string.Empty);
        }

        /// <summary>
        /// Sets the Marain tenant type of the tenant.
        /// </summary>
        /// <param name="tenant">The tenant to set the type of.</param>
        /// <param name="marainTenantType">The tenant type to set.</param>
        /// <remarks>This method does not persist the tenant. Calling code should do this once all changes have been made.</remarks>
        internal static void SetMarainTenantType(this ITenant tenant, MarainTenantType marainTenantType)
        {
            tenant.Properties.Set(TenantPropertyKeys.MarainTenantType, marainTenantType);
        }
    }
}
