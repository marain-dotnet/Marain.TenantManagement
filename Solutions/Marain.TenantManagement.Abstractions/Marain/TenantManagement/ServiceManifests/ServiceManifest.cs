// <copyright file="ServiceManifest.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.ServiceManifests
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Manifest for a Marain service, needed when onboarding a new tenant to use that service.
    /// </summary>
    public class ServiceManifest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceManifest"/> class.
        /// </summary>
        /// <param name="serviceName">The <see cref="ServiceName"/>.</param>
        public ServiceManifest(string serviceName)
        {
            if (serviceName == null)
            {
                throw new ArgumentNullException(nameof(serviceName), "You must provide a service name when creating a ServiceManifest");
            }

            if (string.IsNullOrWhiteSpace(serviceName))
            {
                throw new ArgumentException(nameof(serviceName), "Service name cannot be empty or whitespace only");
            }

            this.ServiceName = serviceName;
        }

        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        public string ServiceName { get; }

        /// <summary>
        /// Gets a list of the names of other Service Tenants whose services this depends upon.
        /// </summary>
        public IList<string> DependsOnServiceNames { get; } = new List<string>();
    }
}
