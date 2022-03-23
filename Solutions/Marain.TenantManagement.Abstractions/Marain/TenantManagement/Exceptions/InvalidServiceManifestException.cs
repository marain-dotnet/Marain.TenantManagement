// <copyright file="InvalidServiceManifestException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Marain.TenantManagement.ServiceManifests;

    /// <summary>
    /// Exception that will be thrown when an invalid <see cref="ServiceManifest"/> is used.
    /// </summary>
    [Serializable]
    public class InvalidServiceManifestException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidServiceManifestException"/> class.
        /// </summary>
        /// <param name="errors">The list of issues with the manifest.</param>
        public InvalidServiceManifestException(IEnumerable<string> errors)
            : base("The specified manifest is invalid. For full error information, see the Errors list in the exception Data.")
        {
            this.Errors = errors.ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidServiceManifestException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/>.</param>
        /// <param name="context">The <see cref="StreamingContext"/>.</param>
        protected InvalidServiceManifestException(
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