// <copyright file="InitialiseCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli.Commands
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Threading.Tasks;

    /// <summary>
    /// Creates the initialisation command.
    /// </summary>
    public class InitialiseCommand : Command
    {
        private readonly ITenantManagementService tenantManagementService;

        /// <summary>
        /// Creates a new instance of the <see cref="InitialiseCommand"/> class.
        /// </summary>
        /// <param name="tenantManagementService">The tenant management services.</param>
        public InitialiseCommand(ITenantManagementService tenantManagementService)
            : base("init", "Initialises the tenancy provider for use with Marain.")
        {
            this.tenantManagementService = tenantManagementService;
            this.Handler = CommandHandler.Create(() => this.HandleCommand());
        }

        private Task HandleCommand()
        {
            return this.tenantManagementService.InitialiseTenancyProviderAsync();
        }
    }
}
