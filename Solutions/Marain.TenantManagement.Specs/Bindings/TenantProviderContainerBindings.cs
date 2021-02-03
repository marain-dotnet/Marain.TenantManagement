// <copyright file="TenantProviderContainerBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Specs.Bindings
{
    using Corvus.Testing.SpecFlow;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
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

                collection.AddJsonSerializerSettings();
                collection.AddRootTenant();
                collection.AddMarainTenantManagement();
            });
        }

        [BeforeScenario("useInMemoryTenantProvider", Order = ContainerBeforeScenarioOrder.PopulateServiceCollection)]
        public static void UseInMemoryTenantProvider(ScenarioContext scenarioContext)
        {
            ContainerBindings.ConfigureServices(scenarioContext, collection => collection.AddInMemoryTenantProvider());
        }
    }
}
