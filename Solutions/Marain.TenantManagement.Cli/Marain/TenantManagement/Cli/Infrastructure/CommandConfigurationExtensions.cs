// <copyright file="CommandConfigurationExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli;

using Marain.TenantManagement.Cli.Commands;
using Spectre.Console.Cli;

/// <summary>
/// Extensions for configuring commands with Spectre.Console.Cli.
/// </summary>
public static class CommandConfigurationExtensions
{
    /// <summary>
    /// Adds all CLI commands to the command configuration.
    /// </summary>
    /// <param name="config">The command configurator.</param>
    public static void AddCliCommands(this IConfigurator config)
    {
        config.AddCommand<InitialiseCommand>("init")
            .WithDescription("Initialises the tenancy provider for use with Marain.");

        config.AddCommand<CreateClientTenantCommand>("create-client")
            .WithDescription("Creates a new client tenant.")
            .WithExample("create-client", "MyClient")
            .WithExample("create-client", "MyClient", "--parent-id", "parent-tenant-id");

        config.AddCommand<CreateServiceTenantCommand>("create-service")
            .WithDescription("Creates a new service tenant.")
            .WithExample("create-service", "manifest.json");

        config.AddCommand<EnrollCommand>("enroll")
            .WithDescription("Enrolls the specified client for the service.")
            .WithExample("enroll", "client-tenant-id", "service-tenant-id")
            .WithExample("enroll", "client-tenant-id", "service-tenant-id", "--config", "config.json");

        config.AddCommand<UnenrollCommand>("unenroll")
            .WithDescription("Removes the enrollment of the specified client from the service.")
            .WithExample("unenroll", "client-tenant-id", "service-tenant-id");

        config.AddCommand<ShowHierarchyCommand>("show-hierarchy")
            .WithDescription("Shows the tenant hierarchy.")
            .WithExample("show-hierarchy", "tenant-id");

        config.AddCommand<ListRequiredConfigurationForServiceCommand>("list-config")
            .WithDescription("Lists the required configuration for a service.")
            .WithExample("list-config", "service-tenant-id");
    }
}