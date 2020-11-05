// <copyright file="AddConfigurationCommand.cs" company="Endjin Limited">
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
    using Marain.TenantManagement.Configuration;
    using Marain.TenantManagement.Exceptions;
    using Newtonsoft.Json;

    /// <summary>
    /// Enrolls client tenants to use services.
    /// </summary>
    public class AddConfigurationCommand : Command
    {
        private readonly ITenantManagementService tenantManagementService;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;

        /// <summary>
        /// Creates a new instance of the <see cref="AddConfigurationCommand"/> class.
        /// </summary>
        /// <param name="tenantManagementService">The tenant management services.</param>
        /// <param name="serializerSettingsProvider">
        /// The <see cref="IJsonSerializerSettingsProvider"/> to use when reading manifest files.
        /// </param>
        public AddConfigurationCommand(
            ITenantManagementService tenantManagementService,
            IJsonSerializerSettingsProvider serializerSettingsProvider)
            : base("add-config", "Adds arbitrary configuration for the client.")
        {
            this.tenantManagementService = tenantManagementService;
            this.serializerSettingsProvider = serializerSettingsProvider;

            var clientTenantId = new Argument<string>("clientTenantId")
            {
                Description = "The Id of the client tenant.",
                Arity = ArgumentArity.ExactlyOne,
            };

            this.AddArgument(clientTenantId);

            var configFile = new Option<FileInfo>("--config")
            {
                Description = "JSON configuration file.",
                Required = true,
            };

            this.AddOption(configFile);

            this.Handler = CommandHandler.Create(
                (string clientTenantId, string serviceTenantId, FileInfo configFile) => this.HandleCommand(clientTenantId, configFile));
        }

        private async Task<int> HandleCommand(string tenantId, FileInfo configFile)
        {
            string configJson = File.ReadAllText(configFile.FullName);
            ConfigurationItem[] config = JsonConvert.DeserializeObject<ConfigurationItem[]>(configJson, this.serializerSettingsProvider.Instance);

            try
            {
                await this.tenantManagementService.AddConfigurationAsync(
                    tenantId,
                    config).ConfigureAwait(false);

                return 0;
            }
            catch (TenantNotFoundException ex)
            {
                Console.WriteLine($"Unable to add the configuration: {ex.Message}");
                return -1;
            }
            catch (InvalidConfigurationException ex)
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
