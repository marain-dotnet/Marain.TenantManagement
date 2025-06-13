// <copyright file="CliServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;

using Corvus.Identity.ClientAuthentication.Azure;

using Marain.Tenancy.Client;
using Marain.TenantManagement.Cli.Commands;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

/// <summary>
/// Extension methods to configure the DI container used by the CLI.
/// </summary>
public static class CliServiceCollectionExtensions
{
    /// <summary>
    /// Adds the CLI commands to the DI container. These are resolved when the commands are registered with the
    /// Spectre.Console.Cli CommandApp.
    /// </summary>
    /// <param name="services">The service collection to add to.</param>
    /// <returns>The service collection, for chaining.</returns>
    /// <remarks>
    /// We are using convention to register the commands; essentially everything in the same namespace as the
    /// <see cref="InitialiseCommand"/> and that inherits from Spectre.Console.Cli Command types will be registered.
    /// If any commands are added in other namespaces, this method will need to be modified/extended to deal with that.
    /// </remarks>
    public static IServiceCollection AddCliCommands(this IServiceCollection services)
    {
        // Register all command classes for DI resolution
        Type initialiseType = typeof(InitialiseCommand);

        IEnumerable<Type> commands = initialiseType
            .Assembly
            .GetExportedTypes()
            .Where(x => x.Namespace == initialiseType.Namespace &&
                        x.IsClass &&
                        !x.IsAbstract &&
                        x.Name.EndsWith("Command"));

        foreach (Type command in commands)
        {
            services.AddTransient(command);
        }

        return services;
    }

    /// <summary>
    /// Adds services required by the command line application to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add to.</param>
    /// <param name="config">The configuration instance.</param>
    /// <returns>The service collection, for chaining.</returns>
    public static IServiceCollection AddMarainServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddLogging(config => config.AddConsole());

        services.AddJsonSerializerOptionsProvider();
        services.AddJsonPropertyBagFactory();
        services.AddJsonCultureInfoConverter();
        services.AddJsonDateTimeOffsetToIso8601AndUnixTimeConverter();
        services.AddCamelCaseConverterForEnums();

        LegacyAzureServiceTokenProviderOptions serviceTokenProviderOptions = config.Get<LegacyAzureServiceTokenProviderOptions>() ?? new LegacyAzureServiceTokenProviderOptions();

        services.AddServiceIdentityAzureTokenCredentialSourceFromLegacyConnectionString(serviceTokenProviderOptions);
        services.AddMicrosoftRestAdapterForServiceIdentityAccessTokenSource();

        TenancyClientOptions tenancyClientOptions = config.GetSection("TenancyClient").Get<TenancyClientOptions>() ?? new TenancyClientOptions();
        services.AddSingleton(tenancyClientOptions);
        services.AddTenantProviderServiceClient();

        services.AddMarainTenantManagementForBlobStorage();
        services.AddMarainTenantManagementForTableStorage();
        services.AddMarainTenantManagementForCosmosDb();

        return services;
    }
}