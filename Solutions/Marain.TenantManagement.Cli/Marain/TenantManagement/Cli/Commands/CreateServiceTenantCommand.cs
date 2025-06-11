// <copyright file="CreateServiceTenantCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli.Commands
{
    using System;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Corvus.Json.Serialization;
    using Corvus.Tenancy;

    using Marain.TenantManagement.Exceptions;
    using Marain.TenantManagement.ServiceManifests;

    /// <summary>
    /// Creates new service tenants.
    /// </summary>
    public class CreateServiceTenantCommand : Command
    {
        private readonly ITenantStore tenantStore;
        private readonly IJsonSerializerOptionsProvider serializerOptionsProvider;

        /// <summary>
        /// Creates a new instance of the <see cref="CreateServiceTenantCommand"/> class.
        /// </summary>
        /// <param name="tenantStore">The tenant store.</param>
        /// <param name="serializerOptionsProvider">
        /// The <see cref="IJsonSerializerOptionsProvider"/> to use when reading manifest files.
        /// </param>
        public CreateServiceTenantCommand(ITenantStore tenantStore, IJsonSerializerOptionsProvider serializerOptionsProvider)
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
            this.tenantStore = tenantStore;
            this.serializerOptionsProvider = serializerOptionsProvider;
        }

        private async Task HandleCommand(FileInfo manifestFile)
        {
            ArgumentNullException.ThrowIfNull(manifestFile);

            string manifestJson = await File.ReadAllTextAsync(manifestFile.FullName);
            ServiceManifest manifest = JsonSerializer.Deserialize<ServiceManifest>(manifestJson, this.serializerOptionsProvider.Instance)!;

            try
            {
                await this.tenantStore.CreateServiceTenantAsync(manifest).ConfigureAwait(false);
            }
            catch (InvalidServiceManifestException ex)
            {
                Console.WriteLine(ex.Message);

                foreach (string current in ex.Errors)
                {
                    Console.Write("     ");
                    Console.WriteLine(current);
                }
            }
        }
    }
}