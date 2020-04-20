﻿// <copyright file="UnenrollCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli.Commands
{
    using System;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.IO;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.Tenancy.Exceptions;
    using Marain.TenantManagement.EnrollmentConfiguration;
    using Marain.TenantManagement.Exceptions;
    using Newtonsoft.Json;

    /// <summary>
    /// Enrolls client tenants to use services.
    /// </summary>
    public class UnenrollCommand : Command
    {
        private readonly ITenantManagementService tenantManagementService;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;

        /// <summary>
        /// Creates a new instance of the <see cref="EnrollCommand"/> class.
        /// </summary>
        /// <param name="tenantManagementService">The tenant management services.</param>
        /// <param name="serializerSettingsProvider">
        /// The <see cref="IJsonSerializerSettingsProvider"/> to use when reading manifest files.
        /// </param>
        public UnenrollCommand(
            ITenantManagementService tenantManagementService,
            IJsonSerializerSettingsProvider serializerSettingsProvider)
            : base("unenroll", "Unenrolls the specified client from the service.")
        {
            this.tenantManagementService = tenantManagementService;
            this.serializerSettingsProvider = serializerSettingsProvider;

            var clientTenantId = new Argument<string>("clientTenantId")
            {
                Description = "The Id of the client tenant.",
                Arity = ArgumentArity.ExactlyOne,
            };

            this.AddArgument(clientTenantId);

            var serviceName = new Argument<string>("serviceTenantId")
            {
                Description = "The Id of the service tenant.",
                Arity = ArgumentArity.ExactlyOne,
            };

            this.AddArgument(serviceName);

            this.Handler = CommandHandler.Create(
                (string clientTenantId, string serviceTenantId) => this.HandleCommand(clientTenantId, serviceTenantId));
        }

        private async Task<int> HandleCommand(string enrollingTenantId, string serviceTenantId)
        {
            try
            {
                await this.tenantManagementService.UnenrollFromServiceAsync(
                    enrollingTenantId,
                    serviceTenantId).ConfigureAwait(false);

                return 0;
            }
            catch (TenantNotFoundException ex)
            {
                Console.WriteLine($"Unable to complete the enrollment: {ex.Message}");
                return -1;
            }
        }
    }
}
