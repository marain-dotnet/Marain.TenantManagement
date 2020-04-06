// <copyright file="CreateServiceTenantCommand.cs" company="Endjin Limited">
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
    using Marain.TenantManagement.ServiceManifests;
    using Newtonsoft.Json;

    /// <summary>
    /// Creates the initialisation commands.
    /// </summary>
    public class CreateServiceTenantCommand : Command
    {
        private readonly ITenantManagementService tenantManagementService;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;

        /// <summary>
        /// Creates a new instance of the <see cref="CreateServiceTenantCommand"/> class.
        /// </summary>
        /// <param name="tenantManagementService">The tenant management services.</param>
        /// <param name="serializerSettingsProvider">
        /// The <see cref="IJsonSerializerSettingsProvider"/> to use when reading manifest files.
        /// </param>
        public CreateServiceTenantCommand(
            ITenantManagementService tenantManagementService,
            IJsonSerializerSettingsProvider serializerSettingsProvider)
            : base("create-service", "Initialises the tenancy provider for use with Marain.")
        {
            var manifestFile = new Argument<FileInfo>("manifest")
            {
                Description = "The path to the manifest for the new service.",
                Arity = ArgumentArity.ExactlyOne,
            };

            manifestFile.ExistingOnly();

            this.AddArgument(manifestFile);

            this.Handler = CommandHandler.Create((FileInfo manifest) => this.HandleCommand(manifest));
            this.tenantManagementService = tenantManagementService;
            this.serializerSettingsProvider = serializerSettingsProvider;
        }

        private Task HandleCommand(FileInfo manifestFile)
        {
            if (manifestFile == null)
            {
                throw new ArgumentNullException(nameof(manifestFile));
            }

            string manifestJson = File.ReadAllText(manifestFile.FullName);
            ServiceManifest manifest =
                JsonConvert.DeserializeObject<ServiceManifest>(manifestJson, this.serializerSettingsProvider.Instance);

            return this.tenantManagementService.CreateServiceTenantAsync(manifest);
        }
    }
}
