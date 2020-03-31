// <copyright file="ExceptionHandlingSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Specs.Steps
{
    using System;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class ExceptionHandlingSteps
    {
        private readonly ScenarioContext scenarioContext;

        public ExceptionHandlingSteps(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }

        [Then(@"an '(.*)' is thrown")]
        public void ThenAnIsThrown(string exceptionTypeName)
        {
            if (!this.scenarioContext.TryGetValue(out Exception lastException))
            {
                Assert.Fail("No exception was thrown.");
            }

            string lastExceptionType = lastException.GetType().Name;
            Assert.AreEqual(
                exceptionTypeName,
                lastExceptionType,
                $"Expected an exception of type '{exceptionTypeName}' to have been thrown, but the last exception captured was of type '{lastExceptionType}'");
        }
    }
}
