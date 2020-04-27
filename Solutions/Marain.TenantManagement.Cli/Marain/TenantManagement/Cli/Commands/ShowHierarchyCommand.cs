// <copyright file="ShowHierarchyCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli.Commands
{
    using System;
    using System.Collections.Generic;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Tenancy;

    /// <summary>
    /// Writes out the current tenancy hierarchy.
    /// </summary>
    public class ShowHierarchyCommand : Command
    {
        private readonly ITenantStore tenantStore;

        /// <summary>
        /// Creates a new instance of the <see cref="ShowHierarchyCommand"/>.
        /// </summary>
        /// <param name="tenantStore">The <see cref="ITenantStore"/>.</param>
        public ShowHierarchyCommand(ITenantStore tenantStore)
            : base("show-hierarchy", "Outputs a visualisation of the current tenancy hierarchy")
        {
            this.tenantStore = tenantStore;
            this.Handler = CommandHandler.Create(() => this.HandleCommand());
        }

        private async Task HandleCommand()
        {
            var root = new TenantWithChildren(this.tenantStore.Root, 0);
            await this.AddChildrenTo(root).ConfigureAwait(false);

            this.WriteTenantHierarchy(root);
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
            var result = new List<ITenant>();
            result.Add(root.Tenant);

            foreach (TenantWithChildren child in root.Children)
            {
                result.AddRange(this.FlattenHierarchy(child));
            }

            return result;
        }

        private void WriteTenantHierarchy(TenantWithChildren root, IList<ITenant>? allTenants = null)
        {
            allTenants ??= this.FlattenHierarchy(root);

            string spacing = " |";
            for (int i = 1; i < root.Depth; i++)
            {
                spacing += "     |";
            }

            if (root.Depth > 0)
            {
                Console.WriteLine(spacing);
                Console.Write(spacing);
                Console.Write("-> ");
            }

            Console.WriteLine($"{root.Tenant.Name} - ({root.Tenant.Id})");
            Console.Write(spacing);
            Console.WriteLine($"     [Type: {root.Tenant.GetMarainTenantType()}]");

            var enrollments = root.Tenant.GetEnrollments().ToList();
            foreach (string enrollment in enrollments)
            {
                ITenant? serviceTenant = allTenants.FirstOrDefault(x => x.Id == enrollment);
                Console.Write(spacing);

                if (serviceTenant != null && serviceTenant.GetServiceManifest().DependsOnServiceTenants.Count > 0)
                {
                    string delegatedTenantId = root.Tenant.GetDelegatedTenantIdForServiceId(serviceTenant.Id);
                    ITenant? delegatedTenant = allTenants.FirstOrDefault(x => x.Id == delegatedTenantId);
                    Console.WriteLine($"     [Enrollment: '{serviceTenant.Name}', with delegated tenant '{delegatedTenant?.Name ?? delegatedTenantId}']");
                }
                else
                {
                    Console.WriteLine($"     [Enrollment: {serviceTenant?.Name ?? enrollment}]");
                }
            }

            foreach (TenantWithChildren child in root.Children)
            {
                this.WriteTenantHierarchy(child, allTenants);
            }
        }

        private class TenantWithChildren
        {
            public TenantWithChildren(ITenant tenant, int depth)
            {
                this.Tenant = tenant;
                this.Depth = depth;
            }

            public int Depth { get; set; }

            public List<TenantWithChildren> Children { get; set; } = new List<TenantWithChildren>();

            public ITenant Tenant { get; set; }
        }
    }
}
