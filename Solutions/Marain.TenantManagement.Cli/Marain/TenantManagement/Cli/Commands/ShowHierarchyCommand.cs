// <copyright file="ShowHierarchyCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli.Commands;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Corvus.Tenancy;

using Marain.TenantManagement;

using Spectre.Console;
using Spectre.Console.Cli;

/// <summary>
/// Writes out the current tenancy hierarchy.
/// </summary>
public class ShowHierarchyCommand : AsyncCommand<ShowHierarchyCommand.Settings>
{
    private readonly ITenantStore tenantStore;

    /// <summary>
    /// Creates a new instance of the <see cref="ShowHierarchyCommand"/>.
    /// </summary>
    /// <param name="tenantStore">The <see cref="ITenantStore"/>.</param>
    public ShowHierarchyCommand(ITenantStore tenantStore)
    {
        this.tenantStore = tenantStore;
    }

    /// <inheritdoc />
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        try
        {
            AnsiConsole.MarkupLine("[bold blue]Loading tenant hierarchy...[/]");

            ITenant startTenant = string.IsNullOrEmpty(settings.TenantId)
                ? this.tenantStore.Root
                : await this.tenantStore.GetTenantAsync(settings.TenantId);

            var root = new TenantWithChildren(startTenant, 0);
            await this.AddChildrenTo(root).ConfigureAwait(false);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[bold green]Tenant Hierarchy:[/]");
            AnsiConsole.WriteLine();

            this.WriteTenantHierarchy(root);
            return 0;
        }
        catch (System.Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red]✗[/] Failed to show hierarchy: {ex.Message}");
            return -1;
        }
    }

    private async Task AddChildrenTo(TenantWithChildren parent)
    {
        await foreach (string childId in this.tenantStore.EnumerateAllChildrenAsync(parent.Tenant.Id))
        {
            ITenant childTenant = await this.tenantStore.GetTenantAsync(childId).ConfigureAwait(false);

            var childEntry = new TenantWithChildren(childTenant, parent.Depth + 1);
            await this.AddChildrenTo(childEntry).ConfigureAwait(false);

            parent.Children.Add(childEntry);
        }
    }

    private IList<ITenant> FlattenHierarchy(TenantWithChildren root)
    {
        List<ITenant> result =
        [
            root.Tenant
        ];

        foreach (TenantWithChildren child in root.Children)
        {
            result.AddRange(this.FlattenHierarchy(child));
        }

        return result;
    }

    private void WriteTenantHierarchy(TenantWithChildren root, IList<ITenant>? allTenants = null)
    {
        allTenants ??= this.FlattenHierarchy(root);

        // Create tree structure using Spectre.Console Tree
        if (root.Depth == 0)
        {
            var tree = new Tree(this.FormatTenantNode(root.Tenant));
            this.AddChildrenToTree(tree, root, allTenants);
            AnsiConsole.Write(tree);
        }
    }

    private void AddChildrenToTree(IHasTreeNodes parent, TenantWithChildren tenantNode, IList<ITenant> allTenants)
    {
        foreach (TenantWithChildren child in tenantNode.Children)
        {
            TreeNode childTreeNode = parent.AddNode(this.FormatTenantNode(child.Tenant));

            // Add enrollments as sub-nodes
            List<string> enrollments = child.Tenant.GetEnrollments().ToList();

            if (enrollments.Any())
            {
                TreeNode enrollmentNode = childTreeNode.AddNode("[dim]Enrollments:[/]");
                foreach (string enrollment in enrollments)
                {
                    ITenant? serviceTenant = allTenants.FirstOrDefault(x => x.Id == enrollment);
                    if (serviceTenant?.GetServiceManifest().DependsOnServiceTenants.Count > 0)
                    {
                        string delegatedTenantId = child.Tenant.GetDelegatedTenantIdForServiceId(serviceTenant.Id);
                        ITenant? delegatedTenant = allTenants.FirstOrDefault(x => x.Id == delegatedTenantId);
                        enrollmentNode.AddNode($"[yellow]↳[/] {serviceTenant.Name} [dim](delegated: {delegatedTenant?.Name ?? delegatedTenantId})[/]");
                    }
                    else
                    {
                        enrollmentNode.AddNode($"[yellow]↳[/] {serviceTenant?.Name ?? enrollment}");
                    }
                }
            }

            this.AddChildrenToTree(childTreeNode, child, allTenants);
        }
    }

    private string FormatTenantNode(ITenant tenant)
    {
        MarainTenantType tenantType = tenant.GetMarainTenantType();
        string typeColor = tenantType switch
        {
            MarainTenantType.Service => "green",
            MarainTenantType.Client => "blue",
            _ => "white",
        };

        return $"[bold {typeColor}]{tenant.Name}[/] [dim]({tenant.Id})[/] [dim][[{tenantType}]][/]";
    }

    /// <summary>
    /// Settings for the show hierarchy command.
    /// </summary>
    public sealed class Settings : CommandSettings
    {
        /// <summary>
        /// Gets or sets the tenant ID to start the hierarchy from.
        /// </summary>
        [CommandArgument(0, "[tenantId]")]
        [Description("The tenant ID to start the hierarchy from. If not specified, shows the full hierarchy from root.")]
        public string? TenantId { get; set; }
    }

    private class TenantWithChildren
    {
        public TenantWithChildren(ITenant tenant, int depth)
        {
            this.Tenant = tenant;
            this.Depth = depth;
        }

        public int Depth { get; set; }

        public List<TenantWithChildren> Children { get; set; } = new();

        public ITenant Tenant { get; set; }
    }
}