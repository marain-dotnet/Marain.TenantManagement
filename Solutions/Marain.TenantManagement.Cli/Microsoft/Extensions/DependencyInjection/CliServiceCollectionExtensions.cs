// <copyright file="CliServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection;

using System;
using Corvus.Identity.ClientAuthentication.Azure;
using Marain.Tenancy;

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
    /// <param name="enableTenantCaching">A flag indicating whether or not tenants retrieved from the tenancy API should be cached.</param>
    /// <returns>The service collection, for chaining.</returns>
    public static IServiceCollection AddMarainServices(this IServiceCollection services, IConfiguration config, bool enableTenantCaching = true)
    {
        services.AddLogging(config => config.AddConsole());

        services.AddJsonSerializerOptionsProvider();
        services.AddJsonPropertyBagFactory();
        services.AddJsonCultureInfoConverter();
        services.AddJsonDateTimeOffsetToIso8601AndUnixTimeConverter();
        services.AddCamelCaseConverterForEnums();

        LegacyAzureServiceTokenProviderOptions serviceTokenProviderOptions = config.Get<LegacyAzureServiceTokenProviderOptions>() ?? new LegacyAzureServiceTokenProviderOptions();

        services.AddServiceIdentityAzureTokenCredentialSourceFromLegacyConnectionString(serviceTokenProviderOptions);

        TenancyApiClientConfiguration? tenancyClientOptions = config.GetSection("TenancyClient").Get<TenancyApiClientConfiguration>();
        if (tenancyClientOptions is null)
        {
            throw new InvalidOperationException("Missing TenancyClient configuration");
        }

        services.AddTenancyClient(_ => tenancyClientOptions, true);

        services.AddSingleton(tenancyClientOptions);
        services.AddTenantProviderServiceClient();

        services.AddMarainTenantManagementForBlobStorage();
        services.AddMarainTenantManagementForTableStorage();
        services.AddMarainTenantManagementForCosmosDb();

        return services;
    }
}