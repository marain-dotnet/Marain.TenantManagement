// <copyright file="Program.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli
{
    using System.CommandLine;
    using System.CommandLine.Builder;
    using System.CommandLine.Parsing;
    using System.Threading.Tasks;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Marain.Tenancy.Client;
    using Marain.TenantManagement.Cli.Commands;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

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
            var services = new ServiceCollection();
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            services.AddSingleton<IConfiguration>(config);

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

            services.AddSingleton<Command, InitialiseCommand>();
            services.AddSingleton<Command, ShowHierarchyCommand>();
            services.AddSingleton<Command, CreateClientTenantCommand>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            var commandLineBuilder = new CommandLineBuilder();

            foreach (Command command in serviceProvider.GetServices<Command>())
            {
                commandLineBuilder.AddCommand(command);
            }

            Parser parser = commandLineBuilder.UseDefaults().Build();

            return parser.InvokeAsync(args);
        }
    }
}
