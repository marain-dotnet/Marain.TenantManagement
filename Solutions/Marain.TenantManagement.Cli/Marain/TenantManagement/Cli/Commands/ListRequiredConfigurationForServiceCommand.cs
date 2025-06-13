// <copyright file="ListRequiredConfigurationForServiceCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli.Commands;

using System.ComponentModel;
using System.Threading.Tasks;

using Corvus.Tenancy;

using Marain.TenantManagement.ServiceManifests;

using Spectre.Console;
using Spectre.Console.Cli;

/// <summary>
/// Lists the configuration required to enroll a client tenant to use a service.
/// </summary>
public class ListRequiredConfigurationForServiceCommand : AsyncCommand<ListRequiredConfigurationForServiceCommand.Settings>
{
    private readonly ITenantStore tenantStore;

    /// <summary>
    /// Creates a new instance of the <see cref="ListRequiredConfigurationForServiceCommand"/> class.
    /// </summary>
    /// <param name="tenantStore">The tenant store.</param>
    public ListRequiredConfigurationForServiceCommand(ITenantStore tenantStore)
    {
        this.tenantStore = tenantStore;
    }

    /// <inheritdoc />
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            AnsiConsole.MarkupLine($"[bold blue]Retrieving configuration requirements for service '{settings.ServiceTenantId}'...[/]");

            ServiceManifestRequiredConfigurationEntryIncludingDescendants configRequirements =
                await this.tenantStore.GetServiceEnrollmentConfigurationRequirementsAsync(settings.ServiceTenantId).ConfigureAwait(false);

            if (configRequirements.RequiredConfigurationEntries.Count == 0)
            {
                AnsiConsole.MarkupLine($"[yellow]ℹ[/] The service '{settings.ServiceTenantId}' does not have any configuration requirements for enrollment.");
                return 0;
            }

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[bold green]Required Configuration Entries:[/]");
            AnsiConsole.WriteLine();

            var table = new Table();
            table.AddColumn("[bold]Key[/]");
            table.AddColumn("[bold]Description[/]");
            table.AddColumn("[bold]Content Type[/]");

            table.Border = TableBorder.Rounded;
            table.BorderColor(Color.Blue);

            foreach (ServiceManifestRequiredConfigurationEntry current in configRequirements.RequiredConfigurationEntries)
            {
                table.AddRow(
                    $"[cyan]{current.Key}[/]",
                    current.Description ?? "[dim]No description[/]",
                    $"[yellow]{current.ContentType}[/]");
            }

            AnsiConsole.Write(table);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[bold green]✓[/] Found {configRequirements.RequiredConfigurationEntries.Count} configuration requirement(s).");

            return 0;
        }
        catch (System.Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red]✗[/] Failed to retrieve configuration requirements: {ex.Message}");
            return -1;
        }
    }

    /// <summary>
    /// Settings for the list required configuration command.
    /// </summary>
    public sealed class Settings : CommandSettings
    {
        /// <summary>
        /// Gets or sets the Id of the service tenant.
        /// </summary>
        [CommandArgument(0, "<serviceTenantId>")]
        [Description("The Id of the service tenant.")]
        public string ServiceTenantId { get; set; } = string.Empty;

        /// <inheritdoc />
        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(this.ServiceTenantId))
            {
                return ValidationResult.Error("Service tenant ID is required.");
            }

            return ValidationResult.Success();
        }
    }
}