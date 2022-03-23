// <copyright file="ListRequiredConfigurationForServiceCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli.Commands
{
    using System;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Threading.Tasks;
    using ConsoleTables;
    using Corvus.Tenancy;
    using Marain.TenantManagement.ServiceManifests;

    /// <summary>
    /// Lists the configuration required to enroll a client tenant to use a service.
    /// </summary>
    public class ListRequiredConfigurationForServiceCommand : Command
    {
        private readonly ITenantStore tenantStore;

        /// <summary>
        /// Creates a new instance of the <see cref="ListRequiredConfigurationForServiceCommand"/> class.
        /// </summary>
        /// <param name="tenantStore">The tenant store.</param>
        public ListRequiredConfigurationForServiceCommand(ITenantStore tenantStore)
            : base("list-required-config", "Lists configuration requirements to enroll a client tenant in a service.")
        {
            this.tenantStore = tenantStore;

            var serviceName = new Argument<string>("serviceTenantId")
            {
                Description = "The Id of the service tenant.",
                Arity = ArgumentArity.ExactlyOne,
            };

            this.AddArgument(serviceName);

            this.Handler = CommandHandler.Create((string serviceTenantId) => this.HandleCommand(serviceTenantId));
        }

        private async Task HandleCommand(string serviceTenantId)
        {
            ServiceManifestRequiredConfigurationEntry[] configRequirements =
                await this.tenantStore.GetServiceEnrollmentConfigurationRequirementsAsync(serviceTenantId).ConfigureAwait(false);

            if (configRequirements.Length == 0)
            {
                Console.WriteLine($"The service '{serviceTenantId}' does not have any configuration requirements for enrollment.");
            }
            else
            {
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
}