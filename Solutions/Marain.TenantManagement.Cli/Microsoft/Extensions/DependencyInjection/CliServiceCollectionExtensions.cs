// <copyright file="CliServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using System.CommandLine;
    using System.Linq;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Marain.Tenancy.Client;
    using Marain.TenantManagement.Cli.Commands;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Extension methods to configure the DI container used by the CLI.
    /// </summary>
    public static class CliServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the CLI commands to the DI container. These are resolved when the commands are registered with the
        /// <c>CommandLineBuilder</c>.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <returns>The service collection, for chaining.</returns>
        /// <remarks>
        /// We are using convention to register the commands; essentially everything in the same namespace as the
        /// <see cref="InitialiseCommand"/> and that implements <c>Command</c> will be registered. If any commands are
        /// added in other namespaces, this method will need to be modified/extended to deal with that.
        /// </remarks>
        public static IServiceCollection AddCliCommands(this IServiceCollection services)
        {
            Type initialiseType = typeof(InitialiseCommand);
            Type commandType = typeof(Command);

            IEnumerable<Type> commands = initialiseType
                .Assembly
                .GetExportedTypes()
                .Where(x => x.Namespace == initialiseType.Namespace && commandType.IsAssignableFrom(x));

            foreach (Type command in commands)
            {
                services.AddSingleton(commandType, command);
            }

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
            services.AddLogging(config => config.AddConsole());

            services.AddJsonNetSerializerSettingsProvider();
            services.AddJsonNetPropertyBag();
            services.AddJsonNetCultureInfoConverter();
            services.AddJsonNetDateTimeOffsetToIso8601AndUnixTimeConverter();
            services.AddSingleton<JsonConverter>(new StringEnumConverter(true));

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