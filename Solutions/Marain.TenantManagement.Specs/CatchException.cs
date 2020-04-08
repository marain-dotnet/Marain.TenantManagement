// <copyright file="CatchException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Specs
{
    using System;
    using System.Threading.Tasks;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Helpers for exception handling in specs.
    /// </summary>
    public static class CatchException
    {
        /// <summary>
        /// Executes the supplied action, catching any errors thrown and storing them in the supplied
        /// <see cref="ScenarioContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="ScenarioContext"/> in which any exception should be stored.</param>
        /// <param name="asyncAction">The action to execute.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task AndStoreInScenarioContextAsync(ScenarioContext context, Func<Task> asyncAction)
        {
            try
            {
                await asyncAction().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                context.Set(ex);
            }
        }

        /// <summary>
        /// Executes the supplied action, catching any errors thrown and storing them in the supplied
        /// <see cref="ScenarioContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="ScenarioContext"/> in which any exception should be stored.</param>
        /// <param name="action">The action to execute.</param>
        public static void AndStoreInScenarioContext(ScenarioContext context, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                context.Set(ex);
            }
        }
    }
}
