// <copyright file="HelloWorldSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Specs.Steps
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class HelloWorldSteps
    {
        private List<int> numbers = new List<int>();
        private int? result = null;

        [Given("I have entered (.*) into the calculator")]
        public void GivenIHaveEnteredIntoTheCalculator(int p0)
        {
            this.numbers.Add(p0);
        }

        [When("I press add")]
        public void WhenIPressAdd()
        {
            this.result = this.numbers.Sum();
        }

        [Then("the result should be (.*) on the screen")]
        public void ThenTheResultShouldBeOnTheScreen(int p0)
        {
            Assert.AreEqual(p0, this.result);
        }
    }
}
