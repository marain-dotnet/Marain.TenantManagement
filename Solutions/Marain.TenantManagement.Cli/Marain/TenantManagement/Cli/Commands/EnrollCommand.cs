// <copyright file="EnrollCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli.Commands;

using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Corvus.Json.Serialization;
using Corvus.Tenancy;
using Corvus.Tenancy.Exceptions;

using Marain.TenantManagement.Configuration;
using Marain.TenantManagement.EnrollmentConfiguration;
using Marain.TenantManagement.Exceptions;

using Spectre.Console;
using Spectre.Console.Cli;

/// <summary>
/// Enrolls client tenants to use services.
/// </summary>
public class EnrollCommand : AsyncCommand<EnrollCommand.Settings>
{
    private readonly ITenantStore tenantStore;
    private readonly IJsonSerializerOptionsProvider serializerOptionsProvider;

    /// <summary>
    /// Creates a new instance of the <see cref="EnrollCommand"/> class.
    /// </summary>
    /// <param name="tenantStore">The tenant store.</param>
    /// <param name="serializerOptionsProvider">
    /// The <see cref="IJsonSerializerOptionsProvider"/> to use when reading manifest files.
    /// </param>
    public EnrollCommand(ITenantStore tenantStore, IJsonSerializerOptionsProvider serializerOptionsProvider)
    {
        this.tenantStore = tenantStore;
        this.serializerOptionsProvider = serializerOptionsProvider;
    }

    /// <inheritdoc />
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            AnsiConsole.MarkupLine($"[bold blue]Enrolling client tenant '{settings.ClientTenantId}' in service '{settings.ServiceTenantId}'...[/]");

            EnrollmentConfigurationEntry enrollmentConfig;

            if (settings.ConfigFile != null)
            {
                AnsiConsole.MarkupLine($"[dim]Using configuration from '{settings.ConfigFile.Name}'[/]");
                string configJson = await File.ReadAllTextAsync(settings.ConfigFile.FullName);
                enrollmentConfig = JsonSerializer.Deserialize<EnrollmentConfigurationEntry>(configJson, this.serializerOptionsProvider.Instance)!;
            }
            else
            {
                enrollmentConfig = new EnrollmentConfigurationEntry(ImmutableDictionary<string, ConfigurationItem>.Empty, null);
            }

            await this.tenantStore.EnrollInServiceAsync(settings.ClientTenantId, settings.ServiceTenantId, enrollmentConfig).ConfigureAwait(false);

            AnsiConsole.MarkupLine("[bold green]✓[/] Client tenant successfully enrolled in service!");
            return 0;
        }
        catch (TenantNotFoundException ex)
        {
            AnsiConsole.MarkupLine($"[bold red]✗[/] Unable to complete the enrollment: {ex.Message}");
            return -1;
        }
        catch (InvalidEnrollmentConfigurationException ex)
        {
            AnsiConsole.MarkupLine("[bold red]✗[/] One or more errors were detected with the configuration data supplied:");

            Panel panel = new(string.Join("\n", ex.Errors.Select(error => $"• {error}")))
            {
                Header = new PanelHeader("[red]Configuration Errors[/]"),
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 0, 1, 0),
            };
            AnsiConsole.Write(panel);

            return -1;
        }
        catch (System.Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red]✗[/] Failed to enroll client tenant: {ex.Message}");
            return -1;
        }
    }

    /// <summary>
    /// Settings for the enroll command.
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

        /// <summary>
        /// Gets or sets the JSON configuration file to use when enrolling.
        /// </summary>
        [CommandOption("--config")]
        [Description("JSON configuration file to use when enrolling.")]
        public FileInfo? ConfigFile { get; set; }

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

            if (this.ConfigFile is { Exists: false })
            {
                return ValidationResult.Error($"Configuration file '{this.ConfigFile.FullName}' does not exist.");
            }

            return ValidationResult.Success();
        }
    }
}