// <copyright file="CliServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection;

using Corvus.Identity.ClientAuthentication.Azure;

using Marain.Tenancy.Client;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

/// <summary>
/// Extension methods to configure the DI container used by the CLI.
/// </summary>
public static class CliServiceCollectionExtensions
{
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