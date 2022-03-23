// <copyright file="InitialiseCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli.Commands
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Threading.Tasks;
    using Corvus.Tenancy;

    /// <summary>
    /// Initialises the tenancy provider.
    /// </summary>
    public class InitialiseCommand : Command
    {
        private readonly ITenantStore tenantStore;

        /// <summary>
        /// Creates a new instance of the <see cref="InitialiseCommand"/> class.
        /// </summary>
        /// <param name="tenantStore">The tenant store.</param>
        public InitialiseCommand(ITenantStore tenantStore)
            : base("init", "Initialises the tenancy provider for use with Marain.")
        {
            this.tenantStore = tenantStore;
            this.Handler = CommandHandler.Create(() => this.HandleCommand());
        }

        private Task HandleCommand()
        {
            return this.tenantStore.InitialiseTenancyProviderAsync();
        }
    }
}