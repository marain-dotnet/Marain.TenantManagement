// <copyright file="CliServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System.CommandLine;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Marain.Tenancy.Client;
    using Marain.TenantManagement.Cli.Commands;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Extension methods to configure the DI container used by the CLI.
    /// </summary>
    public static class CliServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services required by the command line application to the service collection.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <returns>The service collection, for chaining.</returns>
        public static IServiceCollection AddCliCommands(this IServiceCollection services)
        {
            services.AddSingleton<Command, InitialiseCommand>();
            services.AddSingleton<Command, ShowHierarchyCommand>();
            services.AddSingleton<Command, CreateClientTenantCommand>();
            services.AddSingleton<Command, CreateServiceTenantCommand>();

            return services;
        }

        /// <summary>
        /// Adds services required by the command line application to the service collection.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="config">The <see cref="IConfiguration"/>.</param>
        /// <returns>The service collection, for chaining.</returns>
        public static IServiceCollection AddMarainServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddJsonSerializerSettings();

            var msiTokenSourceOptions = new AzureManagedIdentityTokenSourceOptions
            {
                AzureServicesAuthConnectionString = config["AzureServicesAuthConnectionString"],
            };

            services.AddAzureManagedIdentityBasedTokenSource(msiTokenSourceOptions);

            TenancyClientOptions tenancyClientOptions = config.GetSection("TenancyClient").Get<TenancyClientOptions>();
            services.AddSingleton(tenancyClientOptions);
            services.AddTenantProviderServiceClient();

            services.AddMarainTenantManagement();

            return services;
        }
    }
}
