// <copyright file="EnrollCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli.Commands
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.Threading.Tasks;
    using Marain.TenantManagement.EnrollmentConfiguration;

    /// <summary>
    /// Creates the initialisation commands.
    /// </summary>
    public class EnrollCommand : Command
    {
        private readonly ITenantManagementService tenantManagementService;

        /// <summary>
        /// Creates a new instance of the <see cref="EnrollCommand"/> class.
        /// </summary>
        /// <param name="tenantManagementService">The tenant management services.</param>
        public EnrollCommand(ITenantManagementService tenantManagementService)
            : base("enroll", "Enrolls the specified client for the service.")
        {
            this.tenantManagementService = tenantManagementService;

            var clientTenantId = new Argument<string>("clientTenantId")
            {
                Description = "The Id of the client tenant.",
                Arity = ArgumentArity.ExactlyOne,
            };

            this.AddArgument(clientTenantId);

            var serviceName = new Argument<string>("serviceName")
            {
                Description = "The name of the service tenant.",
                Arity = ArgumentArity.ExactlyOne,
            };

            this.AddArgument(serviceName);

            this.Handler = CommandHandler.Create(
                (string clientTenantId, string serviceName) => this.HandleCommand(clientTenantId, serviceName));
        }

        private Task HandleCommand(string enrollingTenantId, string serviceName)
        {
            return this.tenantManagementService.EnrollInServiceAsync(
                enrollingTenantId,
                serviceName,
                new EnrollmentConfigurationItem[0]);
        }
    }
}
