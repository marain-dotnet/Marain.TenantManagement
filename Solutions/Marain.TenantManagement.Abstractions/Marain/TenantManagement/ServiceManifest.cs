// <copyright file="ServiceManifest.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement
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
