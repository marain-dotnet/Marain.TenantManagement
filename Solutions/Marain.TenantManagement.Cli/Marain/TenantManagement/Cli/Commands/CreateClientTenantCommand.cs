// <copyright file="CreateClientTenantCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli.Commands
{
    using System;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Threading.Tasks;

    /// <summary>
    /// Creates the initialisation commands.
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

            this.AddArgument(clientName);

            this.Handler = CommandHandler.Create((string name) => this.HandleCommand(name));
            this.tenantManagementService = tenantManagementService;
        }

        private Task HandleCommand(string name)
        {
            return this.tenantManagementService.CreateClientTenantAsync(name);
        }
    }
}
