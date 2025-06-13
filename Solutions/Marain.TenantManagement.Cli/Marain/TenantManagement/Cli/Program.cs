// <copyright file="Program.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli;

using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Spectre.Console.Cli;

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
    public static async Task<int> Main(string[] args)
    {
        ServiceProvider serviceProvider = BuildServiceProvider();
        TypeRegistrar registrar = new(serviceProvider);

        CommandApp app = new(registrar);

        // Configure the application
        app.Configure(config =>
        {
            // Register commands
            config.AddCliCommands();

            // Configure application settings
            config.SetApplicationName("marain");
            config.SetApplicationVersion("1.0.0");
            config.ValidateExamples();
            config.PropagateExceptions();
        });

        return await app.RunAsync(args);
    }

    private static ServiceProvider BuildServiceProvider()
    {
        ServiceCollection services = new();
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        services.AddSingleton<IConfiguration>(config);
        services.AddMarainServices(config);

        return services.BuildServiceProvider();
    }
}