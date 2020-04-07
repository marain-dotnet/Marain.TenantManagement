// <copyright file="EnrollCommand.cs" company="Endjin Limited">
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
    /// Creates the initialisation commands.
    /// </summary>
    public class EnrollCommand : Command
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
        public EnrollCommand(
            ITenantManagementService tenantManagementService,
            IJsonSerializerSettingsProvider serializerSettingsProvider)
            : base("enroll", "Enrolls the specified client for the service.")
        {
            this.tenantManagementService = tenantManagementService;
            this.serializerSettingsProvider = serializerSettingsProvider;

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

            var configFile = new Option<FileInfo>("--config")
            {
                Description = "JSON configuration file to use when enrolling.",
            };

            this.AddOption(configFile);

            this.Handler = CommandHandler.Create(
                (string clientTenantId, string serviceName, FileInfo? config) => this.HandleCommand(clientTenantId, serviceName, config));
        }

        private async Task<int> HandleCommand(string enrollingTenantId, string serviceName, FileInfo? config)
        {
            var enrollmentConfig = new EnrollmentConfigurationItem[0];

            if (config != null)
            {
                string configJson = File.ReadAllText(config.FullName);
                enrollmentConfig =
                    JsonConvert.DeserializeObject<EnrollmentConfigurationItem[]>(configJson, this.serializerSettingsProvider.Instance);
            }

            try
            {
                await this.tenantManagementService.EnrollInServiceAsync(
                    enrollingTenantId,
                    serviceName,
                    enrollmentConfig).ConfigureAwait(false);

                return 0;
            }
            catch (TenantNotFoundException ex)
            {
                Console.WriteLine($"Unable to complete the enrollment: {ex.Message}");
                return -1;
            }
            catch (InvalidEnrollmentConfigurationException ex)
            {
                Console.WriteLine("One or more errors were detected with the configuration data supplied:");
                foreach (string error in ex.Errors)
                {
                    Console.WriteLine($" - {error}");
                }

                return -1;
            }
        }
    }
}
