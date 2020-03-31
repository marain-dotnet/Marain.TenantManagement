// <copyright file="ManifestSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Specs.Steps
{
    using TechTalk.SpecFlow;

    [Binding]
    public class ManifestSteps
    {
        private readonly ScenarioContext scenarioContext;

        public ManifestSteps(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }

        [Given("I have a service manifest called '(.*)' for a service called '(.*)'")]
        public void GivenIHaveAServiceManifestCalled(string manifestName, string serviceName)
        {
            var manifest = new ServiceManifest(serviceName);
            this.scenarioContext.Set(manifest, manifestName);
        }

        [Given("the service manifest called '(.*)' has the following dependencies")]
        public void GivenTheServiceManifestCalledHasTheFollowingDependencies(string manifestName, Table dependencyTable)
        {
            ServiceManifest manifest = this.scenarioContext.Get<ServiceManifest>(manifestName);

            foreach (TableRow row in dependencyTable.Rows)
            {
                manifest.DependsOnServiceNames.Add(row[0]);
            }
        }
    }
}
