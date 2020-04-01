// <copyright file="Program.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli
{
    using System;
    using System.CommandLine.Builder;
    using System.CommandLine.Hosting;
    using System.CommandLine.Parsing;
    using System.Threading.Tasks;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Marain.Tenancy.Client;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// The main class for the console app.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The entry point for the program.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>When complete, an integer representing success (0) or failure (non-0).</returns>
        public static Task<int> Main(string[] args)
        {
            IHostBuilder builder = Host.CreateDefaultBuilder();

            builder.ConfigureServices((ctx, services) =>
            {
                services.AddJsonSerializerSettings();

                var msiTokenSourceOptions = new AzureManagedIdentityTokenSourceOptions
                {
                    AzureServicesAuthConnectionString = ctx.Configuration["AzureServicesAuthConnectionString"],
                };

                services.AddAzureManagedIdentityBasedTokenSource(msiTokenSourceOptions);

                var tenancyClientOptions = new TenancyClientOptions
                {
                    TenancyServiceBaseUri = new Uri(ctx.Configuration["TenancyClient:TenancyServiceBaseUri"]),
                    ResourceIdForMsiAuthentication = ctx.Configuration["TenancyClient:ResourceIdForMsiAuthentication"],
                };

                services.AddSingleton(tenancyClientOptions);

                services.AddTenantProviderServiceClient();
            });

            Parser parser = new CommandLineBuilder()
                .UseDefaults()
                .UseHost(x => builder)
                .Build();

            return parser.InvokeAsync(args);
        }
    }
}
