﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:3.1.0.0
//      SpecFlow Generator Version:3.1.0.0
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
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.1.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Enrollment")]
    [NUnit.Framework.CategoryAttribute("perScenarioContainer")]
    [NUnit.Framework.CategoryAttribute("useInMemoryTenantProvider")]
    public partial class EnrollmentFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = new string[] {
                "perScenarioContainer",
                "useInMemoryTenantProvider"};
        
#line 1 "Enrollment.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Enrollment", "\tIn order to allow a client to use a Marain service\r\n\tAs an administrator\r\n\tI wan" +
                    "t to enroll a tenant to use that service", ProgrammingLanguage.CSharp, new string[] {
                        "perScenarioContainer",
                        "useInMemoryTenantProvider"});
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
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
 testRunner.And("I have a service manifest called \'FooBar Manifest\' for a service called \'FooBar v" +
                    "1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 12
 testRunner.And("I have used the tenant management service to create a service tenant with manifes" +
                    "t \'FooBar Manifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 13
 testRunner.And("I have a service manifest called \'Operations Manifest\' for a service called \'Oper" +
                    "ations v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "Service Name"});
            table4.AddRow(new string[] {
                        "FooBar v1"});
#line 14
 testRunner.And("the service manifest called \'Operations Manifest\' has the following dependencies", ((string)(null)), table4, "And ");
#line hidden
#line 17
 testRunner.And("I have used the tenant management service to create a service tenant with manifes" +
                    "t \'Operations Manifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 18
 testRunner.And("I have a service manifest called \'Workflow Manifest\' for a service called \'Workfl" +
                    "ow v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                        "Service Name"});
            table5.AddRow(new string[] {
                        "Operations v1"});
#line 19
 testRunner.And("the service manifest called \'Workflow Manifest\' has the following dependencies", ((string)(null)), table5, "And ");
#line hidden
#line 22
 testRunner.And("I have used the tenant management service to create a service tenant with manifes" +
                    "t \'Workflow Manifest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 23
 testRunner.And("I have used the tenant management service to create a new client tenant called \'L" +
                    "itware\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Basic enrollment without dependencies or configuration")]
        public virtual void BasicEnrollmentWithoutDependenciesOrConfiguration()
        {
            string[] tagsOfScenario = ((string[])(null));
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Basic enrollment without dependencies or configuration", null, ((string[])(null)));
#line 25
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 9
this.FeatureBackground();
#line hidden
#line 26
 testRunner.When("I use the tenant enrollment service to enroll the tenant called \'Litware\' in the " +
                        "service called \'FooBar v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 27
 testRunner.Then("the tenant called \'Litware\' should have the id of the tenant called \'FooBar v1\' a" +
                        "dded to its enrollments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Enrollment with a dependency")]
        public virtual void EnrollmentWithADependency()
        {
            string[] tagsOfScenario = ((string[])(null));
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Enrollment with a dependency", null, ((string[])(null)));
#line 29
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 9
this.FeatureBackground();
#line hidden
#line 61
 testRunner.When("I use the tenant enrollment service to enroll the tenant called \'Litware\' in the " +
                        "service called \'Operations v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 62
 testRunner.Then("the tenant called \'Litware\' should have the id of the tenant called \'Operations v" +
                        "1\' added to its enrollments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 63
 testRunner.And("a new child tenant called \'Operations v1\\Litware\' of the service tenant called \'O" +
                        "perations v1\' has been created", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 64
 testRunner.And("the tenant called \'Operations v1\\Litware\' should have the id of the tenant called" +
                        " \'FooBar v1\' added to its enrollments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 65
 testRunner.And("the tenant called \'Litware\' should have the id of the tenant called \'Operations v" +
                        "1\\Litware\' set as the delegated tenant for the service called \'Operations v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Enrollment with multiple levels of dependency")]
        public virtual void EnrollmentWithMultipleLevelsOfDependency()
        {
            string[] tagsOfScenario = ((string[])(null));
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Enrollment with multiple levels of dependency", null, ((string[])(null)));
#line 67
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 9
this.FeatureBackground();
#line hidden
#line 110
 testRunner.When("I use the tenant enrollment service to enroll the tenant called \'Litware\' in the " +
                        "service called \'Workflow v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 111
 testRunner.Then("the tenant called \'Litware\' should have the id of the tenant called \'Workflow v1\'" +
                        " added to its enrollments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 112
 testRunner.And("a new child tenant called \'Workflow v1\\Litware\' of the service tenant called \'Wor" +
                        "kflow v1\' has been created", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 113
 testRunner.And("the tenant called \'Workflow v1\\Litware\' should have the id of the tenant called \'" +
                        "Operations v1\' added to its enrollments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 114
 testRunner.And("the tenant called \'Litware\' should have the id of the tenant called \'Workflow v1\\" +
                        "Litware\' set as the delegated tenant for the service called \'Workflow v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 115
 testRunner.And("a new child tenant called \'Operations v1\\Workflow v1\\Litware\' of the service tena" +
                        "nt called \'Operations v1\' has been created", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 116
 testRunner.And("the tenant called \'Operations v1\\Workflow v1\\Litware\' should have the id of the t" +
                        "enant called \'FooBar v1\' added to its enrollments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 117
 testRunner.And("the tenant called \'Workflow v1\\Litware\' should have the id of the tenant called \'" +
                        "Operations v1\\Workflow v1\\Litware\' set as the delegated tenant for the service c" +
                        "alled \'Operations v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Enrollment with multiple levels of dependency and with the client tenant directly" +
            " enrolled in one of the dependent services")]
        public virtual void EnrollmentWithMultipleLevelsOfDependencyAndWithTheClientTenantDirectlyEnrolledInOneOfTheDependentServices()
        {
            string[] tagsOfScenario = ((string[])(null));
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Enrollment with multiple levels of dependency and with the client tenant directly" +
                    " enrolled in one of the dependent services", null, ((string[])(null)));
#line 119
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 9
this.FeatureBackground();
#line hidden
#line 164
 testRunner.When("I use the tenant enrollment service to enroll the tenant called \'Litware\' in the " +
                        "service called \'Workflow v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 165
 testRunner.And("I use the tenant enrollment service to enroll the tenant called \'Litware\' in the " +
                        "service called \'Operations v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 166
 testRunner.Then("the tenant called \'Litware\' should have the id of the tenant called \'Workflow v1\'" +
                        " added to its enrollments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 167
 testRunner.And("the tenant called \'Litware\' should have the id of the tenant called \'Operations v" +
                        "1\' added to its enrollments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 168
 testRunner.And("a new child tenant called \'Workflow v1\\Litware\' of the service tenant called \'Wor" +
                        "kflow v1\' has been created", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 169
 testRunner.And("the tenant called \'Workflow v1\\Litware\' should have the id of the tenant called \'" +
                        "Operations v1\' added to its enrollments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 170
 testRunner.And("the tenant called \'Litware\' should have the id of the tenant called \'Workflow v1\\" +
                        "Litware\' set as the delegated tenant for the service called \'Workflow v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 171
 testRunner.And("a new child tenant called \'Operations v1\\Workflow v1\\Litware\' of the service tena" +
                        "nt called \'Operations v1\' has been created", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 172
 testRunner.And("the tenant called \'Operations v1\\Workflow v1\\Litware\' should have the id of the t" +
                        "enant called \'FooBar v1\' added to its enrollments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 173
 testRunner.And("the tenant called \'Workflow v1\\Litware\' should have the id of the tenant called \'" +
                        "Operations v1\\Workflow v1\\Litware\' set as the delegated tenant for the service c" +
                        "alled \'Operations v1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 174
 testRunner.And("a new child tenant called \'Operations v1\\Litware\' of the service tenant called \'O" +
                        "perations v1\' has been created", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 175
 testRunner.And("the tenant called \'Operations v1\\Litware\' should have the id of the tenant called" +
                        " \'FooBar v1\' added to its enrollments", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 176
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
