// <copyright file="ListRequiredConfigForServiceCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli.Commands
{
    using System;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Threading.Tasks;
    using ConsoleTables;
    using Marain.TenantManagement.ServiceManifests;

    /// <summary>
    /// Creates the Get Required Config command.
    /// </summary>
    public class ListRequiredConfigForServiceCommand : Command
    {
        private readonly ITenantManagementService tenantManagementService;

        /// <summary>
        /// Creates a new instance of the <see cref="ListRequiredConfigForServiceCommand"/> class.
        /// </summary>
        /// <param name="tenantManagementService">The tenant management services.</param>
        public ListRequiredConfigForServiceCommand(ITenantManagementService tenantManagementService)
            : base("list-required-config", "Lists configuration requirements to enroll a client tenant in a service.")
        {
            this.tenantManagementService = tenantManagementService;

            var serviceName = new Argument<string>("serviceName")
            {
                Description = "The name of the service tenant.",
                Arity = ArgumentArity.ExactlyOne,
            };

            this.AddArgument(serviceName);

            this.Handler = CommandHandler.Create((string serviceName) => this.HandleCommand(serviceName));
        }

        private async Task HandleCommand(string serviceName)
        {
            ServiceManifestRequiredConfigurationEntry[] configRequirements =
                await this.tenantManagementService.GetServiceEnrollmentConfigurationRequirementsAsync(serviceName).ConfigureAwait(false);

            var table = new ConsoleTable(new[] { "Key", "Description", "Content Type" });
            foreach (ServiceManifestRequiredConfigurationEntry current in configRequirements)
            {
                table.AddRow(new[] { current.Key, current.Description, current.ContentType });
            }

            table.Options.OutputTo = Console.Out;
            table.Options.EnableCount = false;
            table.Write(Format.Minimal);
        }
    }
}
