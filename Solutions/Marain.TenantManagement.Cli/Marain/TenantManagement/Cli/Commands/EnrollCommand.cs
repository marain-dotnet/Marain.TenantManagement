// <copyright file="EnrollCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli.Commands
{
    using System;
    using System.Collections.Immutable;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Corvus.Json.Serialization;
    using Corvus.Tenancy;
    using Corvus.Tenancy.Exceptions;

    using Marain.TenantManagement.Configuration;
    using Marain.TenantManagement.EnrollmentConfiguration;
    using Marain.TenantManagement.Exceptions;

    /// <summary>
    /// Enrolls client tenants to use services.
    /// </summary>
    public class EnrollCommand : Command
    {
        private readonly ITenantStore tenantStore;
        private readonly IJsonSerializerOptionsProvider serializerOptionsProvider;

        /// <summary>
        /// Creates a new instance of the <see cref="EnrollCommand"/> class.
        /// </summary>
        /// <param name="tenantStore">The tenant store.</param>
        /// <param name="serializerOptionsProvider">
        /// The <see cref="IJsonSerializerOptionsProvider"/> to use when reading manifest files.
        /// </param>
        public EnrollCommand(ITenantStore tenantStore, IJsonSerializerOptionsProvider serializerOptionsProvider)
            : base("enroll", "Enrolls the specified client for the service.")
        {
            this.tenantStore = tenantStore;
            this.serializerOptionsProvider = serializerOptionsProvider;

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

            this.Handler = CommandHandler.Create((string clientTenantId, string serviceTenantId, FileInfo? config) => this.HandleCommand(clientTenantId, serviceTenantId, config));
        }

        private async Task<int> HandleCommand(string enrollingTenantId, string serviceTenantId, FileInfo? config)
        {
            EnrollmentConfigurationEntry enrollmentConfig;

            if (config != null)
            {
                string configJson = await File.ReadAllTextAsync(config.FullName);
                enrollmentConfig = JsonSerializer.Deserialize<EnrollmentConfigurationEntry>(configJson, this.serializerOptionsProvider.Instance)!;
            }
            else
            {
                enrollmentConfig = new EnrollmentConfigurationEntry(ImmutableDictionary<string, ConfigurationItem>.Empty, null);
            }

            try
            {
                await this.tenantStore.EnrollInServiceAsync(enrollingTenantId, serviceTenantId, enrollmentConfig).ConfigureAwait(false);

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