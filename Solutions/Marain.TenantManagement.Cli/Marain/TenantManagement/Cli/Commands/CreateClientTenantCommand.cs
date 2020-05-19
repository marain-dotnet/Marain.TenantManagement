// <copyright file="CreateClientTenantCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli.Commands
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Threading.Tasks;

    /// <summary>
    /// Creates new client tenants.
    /// </summary>
    public class CreateClientTenantCommand : Command
    {
        private readonly ITenantManagementService tenantManagementService;

        /// <summary>
        /// Creates a new instance of the <see cref="CreateClientTenantCommand"/> class.
        /// </summary>
        /// <param name="tenantManagementService">The tenant management services.</param>
        public CreateClientTenantCommand(ITenantManagementService tenantManagementService)
            : base("create-client", "Initialises the tenancy provider for use with Marain.")
        {
            var clientName = new Argument<string>("name")
            {
                Description = "The name of the new client.",
                Arity = ArgumentArity.ExactlyOne,
            };
            var parentId = new Option<string>("--parentId")
            {
                Description = "[Optional] The ID of the parent Client Tenant.",
            };

            this.AddArgument(clientName);
            this.AddOption(parentId);

            this.Handler = CommandHandler.Create((string name, string parentId) => this.HandleCommand(name, parentId));
            this.tenantManagementService = tenantManagementService;
        }

        private Task HandleCommand(string name, string? parentId)
        {
            return this.tenantManagementService.CreateClientTenantAsync(name, parentId);
        }
    }
}
