// <copyright file="UnenrollCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli.Commands;

using System.ComponentModel;
using System.Threading.Tasks;

using Corvus.Tenancy;
using Corvus.Tenancy.Exceptions;

using Spectre.Console;
using Spectre.Console.Cli;

/// <summary>
/// Unenrolls client tenants from services.
/// </summary>
public class UnenrollCommand : AsyncCommand<UnenrollCommand.Settings>
{
    private readonly ITenantStore tenantStore;

    /// <summary>
    /// Creates a new instance of the <see cref="UnenrollCommand"/> class.
    /// </summary>
    /// <param name="tenantStore">The tenant store.</param>
    public UnenrollCommand(ITenantStore tenantStore)
    {
        this.tenantStore = tenantStore;
    }

    /// <inheritdoc />
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            AnsiConsole.MarkupLine($"[bold blue]Unenrolling client tenant '{settings.ClientTenantId}' from service '{settings.ServiceTenantId}'...[/]");

            await this.tenantStore.UnenrollFromServiceAsync(
                settings.ClientTenantId,
                settings.ServiceTenantId).ConfigureAwait(false);

            AnsiConsole.MarkupLine("[bold green]✓[/] Client tenant successfully unenrolled from service!");
            return 0;
        }
        catch (TenantNotFoundException ex)
        {
            AnsiConsole.MarkupLine($"[bold red]✗[/] Unable to complete the unenrollment: {ex.Message}");
            return -1;
        }
        catch (System.Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red]✗[/] Failed to unenroll client tenant: {ex.Message}");
            return -1;
        }
    }

    /// <summary>
    /// Settings for the unenroll command.
    /// </summary>
    public sealed class Settings : CommandSettings
    {
        /// <summary>
        /// Gets or sets the Id of the client tenant.
        /// </summary>
        [CommandArgument(0, "<clientTenantId>")]
        [Description("The Id of the client tenant.")]
        public string ClientTenantId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Id of the service tenant.
        /// </summary>
        [CommandArgument(1, "<serviceTenantId>")]
        [Description("The Id of the service tenant.")]
        public string ServiceTenantId { get; set; } = string.Empty;

        /// <inheritdoc />
        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(this.ClientTenantId))
            {
                return ValidationResult.Error("Client tenant ID is required.");
            }

            if (string.IsNullOrWhiteSpace(this.ServiceTenantId))
            {
                return ValidationResult.Error("Service tenant ID is required.");
            }

            return ValidationResult.Success();
        }
    }
}