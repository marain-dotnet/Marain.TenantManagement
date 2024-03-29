﻿// <copyright file="EnrollCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli.Commands
{
    using System;
    using System.Collections.Immutable;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.IO;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.Tenancy;
    using Corvus.Tenancy.Exceptions;

    using Marain.TenantManagement.Configuration;
    using Marain.TenantManagement.EnrollmentConfiguration;
    using Marain.TenantManagement.Exceptions;
    using Newtonsoft.Json;

    /// <summary>
    /// Enrolls client tenants to use services.
    /// </summary>
    public class EnrollCommand : Command
    {
        private readonly ITenantStore tenantStore;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;

        /// <summary>
        /// Creates a new instance of the <see cref="EnrollCommand"/> class.
        /// </summary>
        /// <param name="tenantStore">The tenant store.</param>
        /// <param name="serializerSettingsProvider">
        /// The <see cref="IJsonSerializerSettingsProvider"/> to use when reading manifest files.
        /// </param>
        public EnrollCommand(
            ITenantStore tenantStore,
            IJsonSerializerSettingsProvider serializerSettingsProvider)
            : base("enroll", "Enrolls the specified client for the service.")
        {
            this.tenantStore = tenantStore;
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

            var configFile = new Option<FileInfo>("--config")
            {
                Description = "JSON configuration file to use when enrolling.",
            };

            this.AddOption(configFile);

            this.Handler = CommandHandler.Create(
                (string clientTenantId, string serviceTenantId, FileInfo? config) => this.HandleCommand(clientTenantId, serviceTenantId, config));
        }

        private async Task<int> HandleCommand(string enrollingTenantId, string serviceTenantId, FileInfo? config)
        {
            EnrollmentConfigurationEntry enrollmentConfig;

            if (config != null)
            {
                string configJson = File.ReadAllText(config.FullName);
                enrollmentConfig =
                    JsonConvert.DeserializeObject<EnrollmentConfigurationEntry>(configJson, this.serializerSettingsProvider.Instance)!;
            }
            else
            {
                enrollmentConfig = new(ImmutableDictionary<string, ConfigurationItem>.Empty, null);
            }

            try
            {
                await this.tenantStore.EnrollInServiceAsync(
                    enrollingTenantId,
                    serviceTenantId,
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