// <copyright file="InitialiseCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli.Commands;

using System.Threading.Tasks;

using Corvus.Tenancy;

using Spectre.Console;
using Spectre.Console.Cli;

/// <summary>
/// Initialises the tenancy provider.
/// </summary>
public class InitialiseCommand : AsyncCommand
{
    private readonly ITenantStore tenantStore;

    /// <summary>
    /// Creates a new instance of the <see cref="InitialiseCommand"/> class.
    /// </summary>
    /// <param name="tenantStore">The tenant store.</param>
    public InitialiseCommand(ITenantStore tenantStore)
    {
        this.tenantStore = tenantStore;
    }

    /// <inheritdoc />
    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        try
        {
            AnsiConsole.MarkupLine("[bold blue]Initialising tenancy provider...[/]");

            await this.tenantStore.InitialiseTenancyProviderAsync();

            AnsiConsole.MarkupLine("[bold green]✓[/] Tenancy provider initialised successfully!");
            return 0;
        }
        catch (System.Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red]✗[/] Failed to initialise tenancy provider: {ex.Message}");
            return -1;
        }
    }
}