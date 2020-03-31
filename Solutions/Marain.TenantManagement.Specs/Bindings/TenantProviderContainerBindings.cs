// <copyright file="TenantProviderContainerBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Specs.Bindings
{
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Marain.TenantManagement.Specs.Mocks;
    using Microsoft.Extensions.DependencyInjection;
    using TechTalk.SpecFlow;

    [Binding]
    public static class TenantProviderContainerBindings
    {
        [BeforeScenario("useInMemoryTenantProvider", Order = ContainerBeforeScenarioOrder.PopulateServiceCollection)]
        public static void UseInMemoryTenantProvider(ScenarioContext scenarioContext)
        {
            ContainerBindings.ConfigureServices(scenarioContext, collection =>
            {
                collection.AddRootTenant();
                collection.AddJsonSerializerSettings();
                collection.AddSingleton<ITenantProvider, InMemoryTenantProvider>();

                collection.AddMarainTenantManagement();
            });
        }
    }
}
