// <copyright file="CliServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods to configure the DI container used by the CLI.
    /// </summary>
    public static class CliServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services required by the command line application to the service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection to add to.</param>
        /// <returns>The service collection, for chaining.</returns>
        public static IServiceCollection AddCommandLineServices(this IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }
    }
}
