// <copyright file="TenantProviderContainerBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Specs.Bindings
{
    using Corvus.Testing.SpecFlow;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using TechTalk.SpecFlow;

    [Binding]
    public static class TenantProviderContainerBindings
    {
        [BeforeScenario("perScenarioContainer", Order = ContainerBeforeScenarioOrder.PopulateServiceCollection)]
        public static void StandardContainerConfiguration(ScenarioContext scenarioContext)
        {
            ContainerBindings.ConfigureServices(scenarioContext, collection =>
            {
                collection.AddLogging(config =>
                {
                    config.SetMinimumLevel(LogLevel.Debug);
                    config.AddConsole();
                });

                collection.AddJsonNetSerializerSettingsProvider();
                collection.AddJsonNetPropertyBag();
                collection.AddJsonNetCultureInfoConverter();
                collection.AddJsonNetDateTimeOffsetToIso8601AndUnixTimeConverter();
                collection.AddSingleton<JsonConverter>(new StringEnumConverter(true));

                collection.AddMarainTenantManagement();
            });
        }

        [BeforeScenario("useInMemoryTenantProvider", Order = ContainerBeforeScenarioOrder.PopulateServiceCollection)]
        public static void UseInMemoryTenantProvider(ScenarioContext scenarioContext)
        {
            ContainerBindings.ConfigureServices(scenarioContext, collection =>
            {
                collection.AddInMemoryTenantProvider();
            });
        }
    }
}
