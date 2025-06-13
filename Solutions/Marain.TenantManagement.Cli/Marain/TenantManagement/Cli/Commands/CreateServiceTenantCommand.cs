// <copyright file="CreateServiceTenantCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli.Commands;

using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using Corvus.Json.Serialization;
using Corvus.Tenancy;

using Marain.TenantManagement.Exceptions;
using Marain.TenantManagement.ServiceManifests;

using Spectre.Console;
using Spectre.Console.Cli;

/// <summary>
/// Creates new service tenants.
/// </summary>
public class CreateServiceTenantCommand : AsyncCommand<CreateServiceTenantCommand.Settings>
{
    private readonly ITenantStore tenantStore;
    private readonly IJsonSerializerOptionsProvider serializerOptionsProvider;

    /// <summary>
    /// Creates a new instance of the <see cref="CreateServiceTenantCommand"/> class.
    /// </summary>
    /// <param name="tenantStore">The tenant store.</param>
    /// <param name="serializerOptionsProvider">
    /// The <see cref="IJsonSerializerOptionsProvider"/> to use when reading manifest files.
    /// </param>
    public CreateServiceTenantCommand(ITenantStore tenantStore, IJsonSerializerOptionsProvider serializerOptionsProvider)
    {
        this.tenantStore = tenantStore;
        this.serializerOptionsProvider = serializerOptionsProvider;
    }

    /// <inheritdoc />
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            AnsiConsole.MarkupLine($"[bold blue]Creating service tenant from manifest '{settings.ManifestFile.Name}'...[/]");

            string manifestJson = await File.ReadAllTextAsync(settings.ManifestFile.FullName);
            ServiceManifest manifest = JsonSerializer.Deserialize<ServiceManifest>(manifestJson, this.serializerOptionsProvider.Instance)!;

            await this.tenantStore.CreateServiceTenantAsync(manifest).ConfigureAwait(false);

            AnsiConsole.MarkupLine($"[bold green]✓[/] Service tenant created successfully from manifest!");
            return 0;
        }
        catch (InvalidServiceManifestException ex)
        {
            AnsiConsole.MarkupLine($"[bold red]✗[/] Invalid service manifest: {ex.Message}");

            var panel = new Panel(string.Join("\n", ex.Errors))
            {
                Header = new PanelHeader("[red]Manifest Errors[/]"),
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 0, 1, 0),
            };
            AnsiConsole.Write(panel);

            return -1;
        }
        catch (System.Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red]✗[/] Failed to create service tenant: {ex.Message}");
            return -1;
        }
    }

    /// <summary>
    /// Settings for the create service tenant command.
    /// </summary>
    public sealed class Settings : CommandSettings
    {
        /// <summary>
        /// Gets or sets the path to the manifest for the new service.
        /// </summary>
        [CommandArgument(0, "<manifest>")]
        [Description("The path to the manifest for the new service.")]
        public FileInfo ManifestFile { get; set; } = null!;

        /// <inheritdoc />
        public override ValidationResult Validate()
        {
            if (this.ManifestFile is null)
            {
                return ValidationResult.Error("Manifest file path is required.");
            }

            if (!this.ManifestFile.Exists)
            {
                return ValidationResult.Error($"Manifest file '{this.ManifestFile.FullName}' does not exist.");
            }

            return ValidationResult.Success();
        }
    }
}