// <copyright file="TenancyContainerBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Services.Tenancy.Specs.Bindings
{
    using Corvus.Configuration;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Corvus.SpecFlow.Extensions;
    using Marain.Tenancy.Client;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using TechTalk.SpecFlow;

    [Binding]
    public static class TenancyContainerBindings
    {
        [BeforeFeature("@perFeatureContainer", Order = ContainerBeforeFeatureOrder.PopulateServiceCollection)]
        public static void SetupContainer(FeatureContext featureContext)
        {
            ContainerBindings.ConfigureServices(
                featureContext,
                services =>
                {
                    var configBuilder = new ConfigurationBuilder();
                    configBuilder.AddTestConfiguration("appsettings.json", null);
                    IConfigurationRoot config = configBuilder.Build();

                    services.AddSingleton<IConfiguration>(config);

                    services.AddLogging();

                    services.AddJsonSerializerSettings();

                    var msiTokenSourceOptions = new AzureManagedIdentityTokenSourceOptions
                    {
                        AzureServicesAuthConnectionString = config["AzureServicesAuthConnectionString"],
                    };

                    services.AddAzureManagedIdentityBasedTokenSource(msiTokenSourceOptions);

                    TenancyClientOptions tenancyClientOptions = config.GetSection("TenancyClient").Get<TenancyClientOptions>();
                    services.AddSingleton(tenancyClientOptions);

                    services.AddTenantProviderServiceClient();
                    services.AddTenancyClient();
                    services.AddMarainTenantManagement();
                });
        }
    }
}
