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
namespace Marain.TenantManagement.Specs.Features.ServiceManifests
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Service manifest validation")]
    [NUnit.Framework.CategoryAttribute("perScenarioContainer")]
    [NUnit.Framework.CategoryAttribute("useInMemoryTenantProvider")]
    public partial class ServiceManifestValidationFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = new string[] {
                "perScenarioContainer",
                "useInMemoryTenantProvider"};
        
#line 1 "Validation.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features/ServiceManifests", "Service manifest validation", "\tIn order to maintain the integrity of the system\r\n\tAs a developer\r\n\tI want to be" +
                    " able to validate Service Manifests", ProgrammingLanguage.CSharp, featureTags);
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
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Manifest name not set")]
        public void ManifestNameNotSet()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Manifest name not set", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 12
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
#line 13
 testRunner.Given("I have a service manifest called \'Manifest\' with no service name", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 14
 testRunner.When("I validate the service manifest called \'Manifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 15
 testRunner.Then("an \'InvalidServiceManifestException\' is thrown", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Missing well-known tenant GUID")]
        public void MissingWell_KnownTenantGUID()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Missing well-known tenant GUID", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 17
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
#line 18
 testRunner.Given("I have a service manifest called \'Operations Manifest\' for a service called \'Oper" +
                        "ations v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 19
 testRunner.And("the well-known tenant Guid for the manifest called \'Operations Manifest\' is \'0000" +
                        "0000-0000-0000-0000-000000000000\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 20
 testRunner.When("I validate the service manifest called \'Operations Manifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 21
 testRunner.Then("an \'InvalidServiceManifestException\' is thrown", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Well known tenant GUID is already in use")]
        public void WellKnownTenantGUIDIsAlreadyInUse()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Well known tenant GUID is already in use", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 23
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
#line 24
 testRunner.Given("I have a service manifest called \'Operations Manifest\' for a service called \'Oper" +
                        "ations v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 25
 testRunner.And("the well-known tenant Guid for the manifest called \'Operations Manifest\' is \'085f" +
                        "50fa-5006-4fca-aac1-cf1f74b0198e\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 26
 testRunner.Given("I have a service manifest called \'Operations Manifest 2\' for a service called \'Op" +
                        "erations v2\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 27
 testRunner.And("the well-known tenant Guid for the manifest called \'Operations Manifest 2\' is \'08" +
                        "5f50fa-5006-4fca-aac1-cf1f74b0198e\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 28
 testRunner.And("I have used the tenant store to create a service tenant with manifest \'Operations" +
                        " Manifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 29
 testRunner.When("I validate the service manifest called \'Operations Manifest 2\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 30
 testRunner.Then("an \'InvalidServiceManifestException\' is thrown", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Creating a manifest with invalid service names")]
        [NUnit.Framework.TestCaseAttribute("Empty string", "\"\"", null)]
        [NUnit.Framework.TestCaseAttribute("Spaces only", "\"  \"", null)]
        [NUnit.Framework.TestCaseAttribute("Tabs only", "\"\t\"", null)]
        public void CreatingAManifestWithInvalidServiceNames(string scenarioDescription, string serviceName, string[] exampleTags)
        {
            string[] tagsOfScenario = exampleTags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("Scenario Description", scenarioDescription);
            argumentsOfScenario.Add("Service Name", serviceName);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Creating a manifest with invalid service names", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 32
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
#line 33
 testRunner.Given(string.Format("I have a service manifest called \'Manifest\' for a service called \'{0}\'", serviceName), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 34
 testRunner.When("I validate the service manifest called \'Manifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 35
 testRunner.Then("an \'InvalidServiceManifestException\' is thrown", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Duplicate service names are allowed as long as the well known tenant GUIDs are un" +
            "ique")]
        public void DuplicateServiceNamesAreAllowedAsLongAsTheWellKnownTenantGUIDsAreUnique()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Duplicate service names are allowed as long as the well known tenant GUIDs are un" +
                    "ique", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 43
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
#line 44
 testRunner.Given("I have a service manifest called \'Operations Manifest\' for a service called \'Oper" +
                        "ations\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 45
 testRunner.Given("I have a service manifest called \'Operations Manifest 2\' for a service called \'Op" +
                        "erations\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 46
 testRunner.And("I have used the tenant store to create a service tenant with manifest \'Operations" +
                        " Manifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 47
 testRunner.When("I validate the service manifest called \'Operations Manifest 2\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 48
 testRunner.Then("no exception is thrown", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Dependent service does not exist")]
        public void DependentServiceDoesNotExist()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Dependent service does not exist", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 50
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
#line 51
 testRunner.Given("I have a service manifest called \'Workflow Manifest\' for a service called \'Workfl" +
                        "ow v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                            "Service Id",
                            "Expected Name"});
                table1.AddRow(new string[] {
                            "3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858",
                            ""});
#line 52
 testRunner.And("the service manifest called \'Workflow Manifest\' has the following dependencies", ((string)(null)), table1, "And ");
#line hidden
#line 55
 testRunner.When("I validate the service manifest called \'Workflow Manifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 56
 testRunner.Then("an \'InvalidServiceManifestException\' is thrown", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Dependent service without an expected name is valid")]
        public void DependentServiceWithoutAnExpectedNameIsValid()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Dependent service without an expected name is valid", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 58
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
#line 59
 testRunner.Given("I have a service manifest called \'Operations Manifest\' for a service called \'Oper" +
                        "ations v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 60
 testRunner.And("the well-known tenant Guid for the manifest called \'Operations Manifest\' is \'4f52" +
                        "2924-b6e7-48cc-a265-a307407ec858\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 61
 testRunner.And("I have used the tenant store to create a service tenant with manifest \'Operations" +
                        " Manifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 62
 testRunner.And("I have a service manifest called \'Workflow Manifest\' for a service called \'Workfl" +
                        "ow v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                            "Service Id",
                            "Expected Name"});
                table2.AddRow(new string[] {
                            "3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858",
                            ""});
#line 63
 testRunner.And("the service manifest called \'Workflow Manifest\' has the following dependencies", ((string)(null)), table2, "And ");
#line hidden
#line 66
 testRunner.When("I validate the service manifest called \'Workflow Manifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 67
 testRunner.Then("no exception is thrown", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Dependent service with an expected name that matches the service name is valid")]
        public void DependentServiceWithAnExpectedNameThatMatchesTheServiceNameIsValid()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Dependent service with an expected name that matches the service name is valid", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 69
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
#line 70
 testRunner.Given("I have a service manifest called \'Operations Manifest\' for a service called \'Oper" +
                        "ations v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 71
 testRunner.And("the well-known tenant Guid for the manifest called \'Operations Manifest\' is \'4f52" +
                        "2924-b6e7-48cc-a265-a307407ec858\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 72
 testRunner.And("I have used the tenant store to create a service tenant with manifest \'Operations" +
                        " Manifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 73
 testRunner.And("I have a service manifest called \'Workflow Manifest\' for a service called \'Workfl" +
                        "ow v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                            "Service Id",
                            "Expected Name"});
                table3.AddRow(new string[] {
                            "3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858",
                            "Operations v1"});
#line 74
 testRunner.And("the service manifest called \'Workflow Manifest\' has the following dependencies", ((string)(null)), table3, "And ");
#line hidden
#line 77
 testRunner.When("I validate the service manifest called \'Workflow Manifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 78
 testRunner.Then("no exception is thrown", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Dependent service with an expected name that does not match the service name is i" +
            "nvalid")]
        public void DependentServiceWithAnExpectedNameThatDoesNotMatchTheServiceNameIsInvalid()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Dependent service with an expected name that does not match the service name is i" +
                    "nvalid", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 80
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
#line 81
 testRunner.Given("I have a service manifest called \'Operations Manifest\' for a service called \'Oper" +
                        "ations v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 82
 testRunner.And("the well-known tenant Guid for the manifest called \'Operations Manifest\' is \'4f52" +
                        "2924-b6e7-48cc-a265-a307407ec858\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 83
 testRunner.And("I have used the tenant store to create a service tenant with manifest \'Operations" +
                        " Manifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 84
 testRunner.And("I have a service manifest called \'Workflow Manifest\' for a service called \'Workfl" +
                        "ow v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                            "Service Id",
                            "Expected Name"});
                table4.AddRow(new string[] {
                            "3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858",
                            "Operations v2"});
#line 85
 testRunner.And("the service manifest called \'Workflow Manifest\' has the following dependencies", ((string)(null)), table4, "And ");
#line hidden
#line 88
 testRunner.When("I validate the service manifest called \'Workflow Manifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 89
 testRunner.Then("an \'InvalidServiceManifestException\' is thrown", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Duplicate Ids in list of dependencies (regardless of expected names)")]
        public void DuplicateIdsInListOfDependenciesRegardlessOfExpectedNames()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Duplicate Ids in list of dependencies (regardless of expected names)", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 91
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
#line 92
 testRunner.Given("I have a service manifest called \'Operations Manifest\' for a service called \'Oper" +
                        "ations v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 93
 testRunner.And("the well-known tenant Guid for the manifest called \'Operations Manifest\' is \'4f52" +
                        "2924-b6e7-48cc-a265-a307407ec858\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 94
 testRunner.And("I have used the tenant store to create a service tenant with manifest \'Operations" +
                        " Manifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 95
 testRunner.And("I have a service manifest called \'Workflow Manifest\' for a service called \'Workfl" +
                        "ow v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                            "Service Id",
                            "Expected Name"});
                table5.AddRow(new string[] {
                            "3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858",
                            "Operations v1"});
                table5.AddRow(new string[] {
                            "3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858",
                            ""});
                table5.AddRow(new string[] {
                            "3633754ac4c9be44b55bfe791b1780f12429524fe7b6cc48a265a307407ec858",
                            "Operations v1"});
#line 96
 testRunner.And("the service manifest called \'Workflow Manifest\' has the following dependencies", ((string)(null)), table5, "And ");
#line hidden
#line 101
 testRunner.When("I validate the service manifest called \'Workflow Manifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 102
 testRunner.Then("an \'InvalidServiceManifestException\' is thrown", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Invalid required configuration items - Blob storage")]
        [NUnit.Framework.TestCaseAttribute("Missing key", "", "description", "container", "1", null)]
        [NUnit.Framework.TestCaseAttribute("Missing description", "key", "", "container", "1", null)]
        [NUnit.Framework.TestCaseAttribute("Missing container", "key", "description", "", "1", null)]
        [NUnit.Framework.TestCaseAttribute("Missing everything", "", "", "", "3", null)]
        public void InvalidRequiredConfigurationItems_BlobStorage(string scenarioDescription, string key, string description, string containerName, string expectedErrorCount, string[] exampleTags)
        {
            string[] tagsOfScenario = exampleTags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("Scenario Description", scenarioDescription);
            argumentsOfScenario.Add("Key", key);
            argumentsOfScenario.Add("Description", description);
            argumentsOfScenario.Add("Container Name", containerName);
            argumentsOfScenario.Add("Expected Error Count", expectedErrorCount);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Invalid required configuration items - Blob storage", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 104
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
#line 105
 testRunner.Given("I have a service manifest called \'Workflow Manifest\' for a service called \'Workfl" +
                        "ow v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Description",
                            "Container Name"});
                table6.AddRow(new string[] {
                            string.Format("{0}", key),
                            string.Format("{0}", description),
                            string.Format("{0}", containerName)});
#line 106
 testRunner.And("the service manifest called \'Workflow Manifest\' has the following Azure Blob Stor" +
                        "age configuration entries", ((string)(null)), table6, "And ");
#line hidden
#line 109
 testRunner.When("I validate the service manifest called \'Workflow Manifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 110
 testRunner.Then("an \'InvalidServiceManifestException\' is thrown", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 111
 testRunner.And(string.Format("the list of errors attached to the InvalidServiceManifestException contains {0} e" +
                            "ntries", expectedErrorCount), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Invalid required configuration items - Table storage")]
        [NUnit.Framework.TestCaseAttribute("Missing key", "", "description", "table", "1", null)]
        [NUnit.Framework.TestCaseAttribute("Missing description", "key", "", "table", "1", null)]
        [NUnit.Framework.TestCaseAttribute("Missing table", "key", "description", "", "1", null)]
        [NUnit.Framework.TestCaseAttribute("Missing everything", "", "", "", "3", null)]
        public void InvalidRequiredConfigurationItems_TableStorage(string scenarioDescription, string key, string description, string tableName, string expectedErrorCount, string[] exampleTags)
        {
            string[] tagsOfScenario = exampleTags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("Scenario Description", scenarioDescription);
            argumentsOfScenario.Add("Key", key);
            argumentsOfScenario.Add("Description", description);
            argumentsOfScenario.Add("Table Name", tableName);
            argumentsOfScenario.Add("Expected Error Count", expectedErrorCount);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Invalid required configuration items - Table storage", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 120
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
#line 121
 testRunner.Given("I have a service manifest called \'Workflow Manifest\' for a service called \'Workfl" +
                        "ow v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table7 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Description",
                            "Table Name"});
                table7.AddRow(new string[] {
                            string.Format("{0}", key),
                            string.Format("{0}", description),
                            string.Format("{0}", tableName)});
#line 122
 testRunner.And("the service manifest called \'Workflow Manifest\' has the following Azure Table Sto" +
                        "rage configuration entries", ((string)(null)), table7, "And ");
#line hidden
#line 125
 testRunner.When("I validate the service manifest called \'Workflow Manifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 126
 testRunner.Then("an \'InvalidServiceManifestException\' is thrown", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 127
 testRunner.And(string.Format("the list of errors attached to the InvalidServiceManifestException contains {0} e" +
                            "ntries", expectedErrorCount), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Invalid required configuration items - CosmosDb storage")]
        [NUnit.Framework.TestCaseAttribute("Missing key", "", "description", "database", "container", "1", null)]
        [NUnit.Framework.TestCaseAttribute("Missing description", "key", "", "database", "container", "1", null)]
        [NUnit.Framework.TestCaseAttribute("Missing database", "key", "description", "", "container", "1", null)]
        [NUnit.Framework.TestCaseAttribute("Missing container", "key", "description", "database", "", "1", null)]
        [NUnit.Framework.TestCaseAttribute("Missing everything", "", "", "", "", "4", null)]
        public void InvalidRequiredConfigurationItems_CosmosDbStorage(string scenarioDescription, string key, string description, string databaseName, string containerName, string expectedErrorCount, string[] exampleTags)
        {
            string[] tagsOfScenario = exampleTags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("Scenario Description", scenarioDescription);
            argumentsOfScenario.Add("Key", key);
            argumentsOfScenario.Add("Description", description);
            argumentsOfScenario.Add("Database Name", databaseName);
            argumentsOfScenario.Add("Container Name", containerName);
            argumentsOfScenario.Add("Expected Error Count", expectedErrorCount);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Invalid required configuration items - CosmosDb storage", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 136
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
#line 137
 testRunner.Given("I have a service manifest called \'Workflow Manifest\' for a service called \'Workfl" +
                        "ow v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table8 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Description",
                            "Database Name",
                            "Container Name"});
                table8.AddRow(new string[] {
                            string.Format("{0}", key),
                            string.Format("{0}", description),
                            string.Format("{0}", databaseName),
                            string.Format("{0}", containerName)});
#line 138
 testRunner.And("the service manifest called \'Workflow Manifest\' has the following Azure CosmosDb " +
                        "Storage configuration entries", ((string)(null)), table8, "And ");
#line hidden
#line 141
 testRunner.When("I validate the service manifest called \'Workflow Manifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 142
 testRunner.Then("an \'InvalidServiceManifestException\' is thrown", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 143
 testRunner.And(string.Format("the list of errors attached to the InvalidServiceManifestException contains {0} e" +
                            "ntries", expectedErrorCount), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
