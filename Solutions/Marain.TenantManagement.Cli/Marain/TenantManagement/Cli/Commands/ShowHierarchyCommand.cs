// <copyright file="ShowHierarchyCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli.Commands
{
    using System;
    using System.Collections.Generic;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Threading.Tasks;
    using Corvus.Tenancy;

    /// <summary>
    /// Creates the Show Hierarchy command.
    /// </summary>
    public class ShowHierarchyCommand : Command
    {
        private readonly ITenantProvider tenantProvider;

        /// <summary>
        /// Creates a new instance of the <see cref="ShowHierarchyCommand"/>.
        /// </summary>
        /// <param name="tenantProvider">The <see cref="ITenantProvider"/>.</param>
        public ShowHierarchyCommand(ITenantProvider tenantProvider)
            : base("show-hierarchy", "Outputs a visualisation of the current tenancy hierarchy")
        {
            this.tenantProvider = tenantProvider;
            this.Handler = CommandHandler.Create(() => this.HandleCommand());
        }

        private async Task HandleCommand()
        {
            var root = new TenantWithChildren(this.tenantProvider.Root, 0);
            await this.AddChildrenTo(root).ConfigureAwait(false);

            this.WriteTenantHierarchy(root);
        }

        private async Task AddChildrenTo(TenantWithChildren parent)
        {
            IList<string> children = await this.tenantProvider.GetAllChildrenAsync(parent.Tenant.Id).ConfigureAwait(false);
            ITenant[] childTenants = await this.tenantProvider.GetTenantsAsync(children).ConfigureAwait(false);

            foreach (ITenant child in childTenants)
            {
                var childEntry = new TenantWithChildren(child, parent.Depth + 1);
                await this.AddChildrenTo(childEntry).ConfigureAwait(false);

                parent.Children.Add(childEntry);
            }
        }

        private void WriteTenantHierarchy(TenantWithChildren root)
        {
            if (root.Depth > 0)
            {
                string spacing = " |";
                for (int i = 1; i < root.Depth; i++)
                {
                    spacing += "     |";
                }

                Console.WriteLine(spacing);
                Console.Write(spacing);
                Console.Write("-> ");
            }

            Console.WriteLine($"{root.Tenant.Name} - ({root.Tenant.Id})");

            foreach (TenantWithChildren child in root.Children)
            {
                this.WriteTenantHierarchy(child);
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
