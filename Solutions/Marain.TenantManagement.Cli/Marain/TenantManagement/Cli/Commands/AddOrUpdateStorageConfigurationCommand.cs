// <copyright file="AddOrUpdateStorageConfigurationCommand.cs" company="Endjin Limited">
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
    using Corvus.Tenancy;
    using Corvus.Tenancy.Exceptions;
    using Marain.TenantManagement.Configuration;
    using Marain.TenantManagement.Exceptions;
    using Newtonsoft.Json;

    /// <summary>
    /// Add or update arbitrary storage configuration for a tenant.
    /// </summary>
    public class AddOrUpdateStorageConfigurationCommand : Command
    {
        private readonly ITenantStore tenantStore;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;

        /// <summary>
        /// Creates a new instance of the <see cref="AddOrUpdateStorageConfigurationCommand"/> class.
        /// </summary>
        /// <param name="tenantStore">The tenant store.</param>
        /// <param name="serializerSettingsProvider">
        /// The <see cref="IJsonSerializerSettingsProvider"/> to use when reading manifest files.
        /// </param>
        public AddOrUpdateStorageConfigurationCommand(
            ITenantStore tenantStore,
            IJsonSerializerSettingsProvider serializerSettingsProvider)
            : base("add-storage-config", "Adds arbitrary storage configuration for the client.")
        {
            this.tenantStore = tenantStore;
            this.serializerSettingsProvider = serializerSettingsProvider;

            var tenantId = new Argument<string>("tenantId")
            {
                Description = "The Id of the tenant.",
                Arity = ArgumentArity.ExactlyOne,
            };

            this.AddArgument(tenantId);

            var configFile = new Argument<FileInfo>("configFile")
            {
                Description = "JSON configuration file path.",
                Arity = ArgumentArity.ExactlyOne,
            };

            this.AddArgument(configFile);

            this.Handler = CommandHandler.Create(
                (string tenantId, FileInfo configFile) => this.HandleCommand(tenantId, configFile));
        }

        private async Task<int> HandleCommand(string tenantId, FileInfo configFile)
        {
            string configJson = File.ReadAllText(configFile.FullName);
            ConfigurationItem[] config = JsonConvert.DeserializeObject<ConfigurationItem[]>(configJson, this.serializerSettingsProvider.Instance);

            try
            {
                await this.tenantStore.AddOrUpdateStorageConfigurationAsync(
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