// <copyright file="InvalidEnrollmentConfigurationException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Marain.TenantManagement.EnrollmentConfiguration;

    /// <summary>
    /// Exception that will be thrown when invalid <see cref="EnrollmentConfigurationItem"/> entries are supplied
    /// as part of tenant enrollment.
    /// </summary>
    [Serializable]
    public class InvalidEnrollmentConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEnrollmentConfigurationException"/> class.
        /// </summary>
        /// <param name="errors">The list of issues with the manifest.</param>
        public InvalidEnrollmentConfigurationException(IEnumerable<string> errors)
            : base("One or more of the supplied configuration items are invalid. For full error information, see the Errors list in the exception Data.")
        {
            this.Errors = errors.ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEnrollmentConfigurationException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/>.</param>
        /// <param name="context">The <see cref="StreamingContext"/>.</param>
        protected InvalidEnrollmentConfigurationException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the list of errors.
        /// </summary>
        public string[] Errors
        {
            get => (this.Data["Errors"] as string[]) ?? new string[0];

            private set => this.Data["Errors"] = value;
        }
    }
}