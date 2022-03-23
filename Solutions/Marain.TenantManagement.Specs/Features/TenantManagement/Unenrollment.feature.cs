﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.9.0.0
//      SpecFlow Generator Version:3.9.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Marain.TenantManagement.Specs.Features.TenantManagement
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Unenrollment")]
    [NUnit.Framework.CategoryAttribute("perScenarioContainer")]
    [NUnit.Framework.CategoryAttribute("useInMemoryTenantProvider")]
    public partial class UnenrollmentFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = new string[] {
                "perScenarioContainer",
                "useInMemoryTenantProvider"};
        
#line 1 "Unenrollment.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features/TenantManagement", "Unenrollment", "\tIn order to disallow a currently allowed a client from using a Marain service\r\n\t" +
                    "As an administrator\r\n\tI want to unenroll a tenant from a service", ProgrammingLanguage.CSharp, featureTags);
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
        }
        
        public void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 9
#line hidden
#line 10
 testRunner.Given("the tenancy provider has been initialised for use with Marain", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 11
 testRunner.And("I have loaded the manifest called \'SimpleManifestWithNoDependenciesOrConfiguratio" +
                    "n\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 12
 testRunner.And("I have used the tenant store to create a service tenant with manifest \'SimpleMani" +
                    "festWithNoDependenciesOrConfiguration\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 13
 testRunner.And("I have loaded the manifest called \'FooBarServiceManifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 14
 testRunner.And("I have used the tenant store to create a service tenant with manifest \'FooBarServ" +
                    "iceManifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 15
 testRunner.And("I have loaded the manifest called \'OperationsServiceManifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 16
 testRunner.And("I have used the tenant store to create a service tenant with manifest \'Operations" +
                    "ServiceManifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 17
 testRunner.And("I have loaded the manifest called \'WorkflowServiceManifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 18
 testRunner.And("I have used the tenant store to create a service tenant with manifest \'WorkflowSe" +
                    "rviceManifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 19
 testRunner.And("I have used the tenant store to create a new client tenant called \'Litware\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 20
 testRunner.And("I have used the tenant store to create a new client tenant called \'Contoso\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Unenroll from a service that the client has not previously enrolled in")]
        public void UnenrollFromAServiceThatTheClientHasNotPreviouslyEnrolledIn()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Unenroll from a service that the client has not previously enrolled in", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 22
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 9
this.FeatureBackground();
#line hidden
#line 23
 testRunner.When("I use the tenant store to unenroll the tenant called \'Litware\' from the service c" +
                        "alled \'Simple manifest with no dependencies or configuration\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 24
 testRunner.Then("an \'InvalidOperationException\' is thrown", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Basic unenrollment with no dependencies or configuration")]
        public void BasicUnenrollmentWithNoDependenciesOrConfiguration()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Basic unenrollment with no dependencies or configuration", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 26
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 9
this.FeatureBackground();
#line hidden
#line 27
 testRunner.Given("I have used the tenant store to enroll the tenant called \'Litware\' in the service" +
                        " called \'Simple manifest with no dependencies or configuration\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 28
 testRunner.When("I use the tenant store to unenroll the tenant called \'Litware\' from the service c" +
                        "alled \'Simple manifest with no dependencies or configuration\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 29
 testRunner.Then("the tenant called \'Litware\' should not have the id of the tenant called \'Simple m" +
                        "anifest with no dependencies or configuration\' in its enrollments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Basic unenrollment with configuration")]
        public void BasicUnenrollmentWithConfiguration()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Basic unenrollment with configuration", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 31
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 9
this.FeatureBackground();
#line hidden
#line 32
 testRunner.Given("I have enrollment configuration called \'FooBar config\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table24 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Account Name",
                            "Container"});
                table24.AddRow(new string[] {
                            "fooBarStore",
                            "blobaccount",
                            "blobcontainer"});
#line 33
 testRunner.And("the enrollment configuration called \'FooBar config\' contains the following Blob S" +
                        "torage configuration items", ((string)(null)), table24, "And ");
#line hidden
#line 36
 testRunner.And("I have used the tenant store with the enrollment configuration called \'FooBar con" +
                        "fig\' to enroll the tenant called \'Litware\' in the service called \'FooBar v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 37
 testRunner.When("I use the tenant store to unenroll the tenant called \'Litware\' from the service c" +
                        "alled \'FooBar v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 38
 testRunner.Then("the tenant called \'Litware\' should not have the id of the tenant called \'FooBar v" +
                        "1\' in its enrollments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 39
 testRunner.And("the tenant called \'Litware\' should not contain blob storage configuration for a b" +
                        "lob storage container definition with container name \'foobar\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Unenrollment with multiple levels of dependency and with the client tenant remain" +
            "ing directly enrolled in one of the dependent services")]
        public void UnenrollmentWithMultipleLevelsOfDependencyAndWithTheClientTenantRemainingDirectlyEnrolledInOneOfTheDependentServices()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Unenrollment with multiple levels of dependency and with the client tenant remain" +
                    "ing directly enrolled in one of the dependent services", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 41
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 9
this.FeatureBackground();
#line hidden
#line 104
 testRunner.Given("I have enrollment configuration called \'Workflow config\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table25 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Account Name",
                            "Container"});
                table25.AddRow(new string[] {
                            "fooBarStore",
                            "fbblobaccount",
                            "fbblobcontainer"});
                table25.AddRow(new string[] {
                            "operationsStore",
                            "opsblobaccount",
                            "opsblobcontainer"});
#line 105
 testRunner.And("the enrollment configuration called \'Workflow config\' contains the following Blob" +
                        " Storage configuration items", ((string)(null)), table25, "And ");
#line hidden
                TechTalk.SpecFlow.Table table26 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Account Uri",
                            "Database Name",
                            "Container Name"});
                table26.AddRow(new string[] {
                            "workflowStore",
                            "wfaccount",
                            "wfdb",
                            "wfcontainer"});
                table26.AddRow(new string[] {
                            "workflowInstanceStore",
                            "wfaccount",
                            "wfdb",
                            "wfinstancecontainer"});
#line 109
 testRunner.And("the enrollment configuration called \'Workflow config\' contains the following Cosm" +
                        "os configuration items", ((string)(null)), table26, "And ");
#line hidden
#line 113
 testRunner.And("I have enrollment configuration called \'Operations config\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table27 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Account Name",
                            "Container"});
                table27.AddRow(new string[] {
                            "fooBarStore",
                            "fbblobaccount2",
                            "fbblobcontainer2"});
                table27.AddRow(new string[] {
                            "operationsStore",
                            "opsblobaccount2",
                            "opsblobcontainer2"});
#line 114
 testRunner.And("the enrollment configuration called \'Operations config\' contains the following Bl" +
                        "ob Storage configuration items", ((string)(null)), table27, "And ");
#line hidden
#line 118
 testRunner.And("I have used the tenant store with the enrollment configuration called \'Workflow c" +
                        "onfig\' to enroll the tenant called \'Litware\' in the service called \'Workflow v1\'" +
                        "", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 119
 testRunner.And("I have used the tenant store with the enrollment configuration called \'Operations" +
                        " config\' to enroll the tenant called \'Litware\' in the service called \'Operations" +
                        " v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 120
 testRunner.When("I use the tenant store to unenroll the tenant called \'Litware\' from the service c" +
                        "alled \'Workflow v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 121
 testRunner.Then("the tenant called \'Litware\' should not have the id of the tenant called \'Workflow" +
                        " v1\' in its enrollments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 122
 testRunner.And("the tenant called \'Litware\' should not contain Cosmos configuration for a Cosmos " +
                        "container definition with database name \'workflow\' and container name \'definitio" +
                        "ns\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 123
 testRunner.And("the tenant called \'Litware\' should not contain Cosmos configuration for a Cosmos " +
                        "container definition with database name \'workflow\' and container name \'instances" +
                        "\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 124
 testRunner.And("the tenant called \'Litware\' should have the id of the tenant called \'Operations v" +
                        "1\' added to its enrollments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 125
 testRunner.And("the tenant called \'Litware\' should contain blob storage configuration for a blob " +
                        "storage container definition with container name \'operations\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 126
 testRunner.And("there should not be a child tenant called \'Workflow v1\\Litware\' of the service te" +
                        "nant called \'Workflow v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 127
 testRunner.And("the tenant called \'Litware\' should not have a delegated tenant for the service ca" +
                        "lled \'Workflow v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 128
 testRunner.And("there should not be a child tenant called \'Operations v1\\Workflow v1\\Litware\' of " +
                        "the service tenant called \'Operations v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 129
 testRunner.And("a new child tenant called \'Operations v1\\Litware\' of the service tenant called \'O" +
                        "perations v1\' has been created", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 130
 testRunner.And("the tenant called \'Operations v1\\Litware\' should have the id of the tenant called" +
                        " \'FooBar v1\' added to its enrollments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 131
 testRunner.And("the tenant called \'Operations v1\\Litware\' should contain blob storage configurati" +
                        "on for a blob storage container definition with container name \'foobar\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 132
 testRunner.And("the tenant called \'Litware\' should have the id of the tenant called \'Operations v" +
                        "1\\Litware\' set as the delegated tenant for the service called \'Operations v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
