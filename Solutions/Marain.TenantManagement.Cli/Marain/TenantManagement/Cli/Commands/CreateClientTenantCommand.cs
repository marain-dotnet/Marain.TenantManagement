// <copyright file="CreateClientTenantCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli.Commands;

using System;
using System.ComponentModel;
using System.Threading.Tasks;

using Corvus.Tenancy;

using Spectre.Console;
using Spectre.Console.Cli;

/// <summary>
/// Creates new client tenants.
/// </summary>
public class CreateClientTenantCommand : AsyncCommand<CreateClientTenantCommand.Settings>
{
    private readonly ITenantStore tenantStore;

    /// <summary>
    /// Creates a new instance of the <see cref="CreateClientTenantCommand"/> class.
    /// </summary>
    /// <param name="tenantStore">The tenant store.</param>
    public CreateClientTenantCommand(ITenantStore tenantStore)
    {
        this.tenantStore = tenantStore;
    }

    /// <inheritdoc />
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            AnsiConsole.MarkupLine($"[bold blue]Creating client tenant '{settings.Name}'...[/]");

            await this.tenantStore.CreateClientTenantAsync(settings.Name, settings.ParentId, settings.WellKnownGuid);

            AnsiConsole.MarkupLine($"[bold green]✓[/] Client tenant '{settings.Name}' created successfully!");
            return 0;
        }
        catch (System.Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red]✗[/] Failed to create client tenant: {ex.Message}");
            return -1;
        }
    }

    /// <summary>
    /// Settings for the create client tenant command.
    /// </summary>
    public sealed class Settings : CommandSettings
    {
        /// <summary>
        /// Gets or sets the name of the new client.
        /// </summary>
        [CommandArgument(0, "<name>")]
        [Description("The name of the new client.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ID of the parent Client Tenant.
        /// </summary>
        [CommandOption("--parent-id")]
        [Description("The ID of the parent Client Tenant.")]
        public string? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the well-known GUID for the new tenant.
        /// </summary>
        [CommandOption("--well-known-guid")]
        [Description("If specified, will create the new tenant with the provided well-known GUID as the client ID.")]
        public Guid? WellKnownGuid { get; set; }
    }
}