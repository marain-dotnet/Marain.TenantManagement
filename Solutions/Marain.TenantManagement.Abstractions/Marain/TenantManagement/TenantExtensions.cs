// <copyright file="TenantExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement
{
    using System;
    using System.Collections.Generic;
    using Corvus.Tenancy;
    using Marain.TenantManagement.Internal;
    using Marain.TenantManagement.ServiceManifests;

    /// <summary>
    /// Extension methods for <see cref="ITenant"/>s.
    /// </summary>
    public static class TenantExtensions
    {
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
        public static string GetDelegatedTenantIdForService(this ITenant tenant, string serviceTenantId)
        {
            if (serviceTenantId == null)
            {
                throw new ArgumentNullException(nameof(serviceTenantId));
            }

            if (string.IsNullOrWhiteSpace(serviceTenantId))
            {
                throw new ArgumentException(nameof(serviceTenantId));
            }

            if (tenant.Properties.TryGet(TenantPropertyKeys.DelegatedTenantId(serviceTenantId), out string delegatedTenantId))
            {
                return delegatedTenantId;
            }

            throw new ArgumentException($"Tenant '{tenant.Name}' with Id '{tenant.Id}' does not contain a delegated tenant Id for service tenant Id {serviceTenantId}");
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

            return new string[0];
        }

        /// <summary>
        /// Adds the given <see cref="ServiceManifest"/> to the tenant's property bag.
        /// </summary>
        /// <param name="tenant">The tenant to add to.</param>
        /// <param name="manifest">The manifest to add.</param>
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
        /// Stores the Id of a delegated tenant that has been created for a service to use when accessing a dependent service
        /// on the tenant's behalf.
        /// </summary>
        /// <param name="tenant">
        /// The tenant who is able to make calls to the service represented by the specified Service Tenant.
        /// </param>
        /// <param name="serviceTenantId">The Id of the Service Tenant which will be using the Delegated Tenant.</param>
        /// <param name="delegatedTenantId">The Id of the tenant that has been created for the service to use.</param>
        internal static void SetDelegatedTenantIdForService(
            this ITenant tenant,
            string serviceTenantId,
            string delegatedTenantId)
        {
            if (serviceTenantId == null)
            {
                throw new ArgumentNullException(nameof(serviceTenantId));
            }

            if (string.IsNullOrWhiteSpace(serviceTenantId))
            {
                throw new ArgumentException(nameof(serviceTenantId));
            }

            if (delegatedTenantId == null)
            {
                throw new ArgumentNullException(nameof(delegatedTenantId));
            }

            if (string.IsNullOrWhiteSpace(delegatedTenantId))
            {
                throw new ArgumentException(nameof(delegatedTenantId));
            }

            tenant.Properties.Set(TenantPropertyKeys.DelegatedTenantId(serviceTenantId), delegatedTenantId);
        }
    }
}
