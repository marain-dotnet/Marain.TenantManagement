// <copyright file="TenantProviderContainerBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Specs.Bindings
{
    using Corvus.Testing.ReqnRoll;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Reqnroll;

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

                collection.AddMarainTenantManagement();
                collection.AddMarainTenantManagementForBlobStorage();
                collection.AddMarainTenantManagementForTableStorage();
                collection.AddMarainTenantManagementForCosmosDb();
            });
        }

        [BeforeScenario("useInMemoryTenantProvider", Order = ContainerBeforeScenarioOrder.PopulateServiceCollection)]
        public static void UseInMemoryTenantProvider(ScenarioContext scenarioContext)
        {
            ContainerBindings.ConfigureServices(scenarioContext, collection => collection.AddInMemoryTenantProvider());
        }
    }
}