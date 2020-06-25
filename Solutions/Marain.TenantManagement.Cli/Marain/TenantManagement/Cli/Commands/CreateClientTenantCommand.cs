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
                Description = "The ID of the parent Client Tenant.",
            };
            var wellKnownGuid = new Option<Guid>("--wellKnownGuid")
            {
                Description = "If specified, will create the new tenant with the provided well-known GUID as the client ID.",
            };

            this.AddArgument(clientName);
            this.AddOption(parentId);
            this.AddOption(wellKnownGuid);

            this.Handler = CommandHandler.Create((string name, string? parentId, Guid? wellKnownGuid) => this.HandleCommand(name, parentId, wellKnownGuid));
            this.tenantManagementService = tenantManagementService;
        }

        private Task HandleCommand(string name, string? parentId, Guid? wellKnownGuid)
        {
            return this.tenantManagementService.CreateClientTenantAsync(name, parentId, wellKnownGuid);
        }
    }
}
