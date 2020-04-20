// <copyright file="InvalidMarainTenantTypeException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Exceptions
{
    using System;

    /// <summary>
    /// Exception thrown when a tenant or tenant Id is supplied to a method but has an invalid <see cref="MarainTenantType"/>
    /// for the requested operation.
    /// </summary>
    [Serializable]
#pragma warning disable RCS1194 // Implement exception constructors.
    public class InvalidMarainTenantTypeException : Exception
#pragma warning restore RCS1194 // Implement exception constructors.
    {
        /// <summary>
        /// Creates a new instance of the <see cref="InvalidMarainTenantTypeException"/>.
        /// </summary>
        /// <param name="tenantId">The Id of the tenant.</param>
        /// <param name="actualTenantType">The actual <see cref="MarainTenantType"/> of the tenant.</param>
        /// <param name="allowableTenantTypes">The valid values for <see cref="MarainTenantType"/>.</param>
        public InvalidMarainTenantTypeException(
            string tenantId,
            MarainTenantType actualTenantType,
            params MarainTenantType[] allowableTenantTypes)
            : base($"The tenant with Id '{tenantId}' has a tenant type of '{actualTenantType}'. Valid tenant type(s) here are: {string.Join(", ", allowableTenantTypes)}")
        {
        }
    }
}
