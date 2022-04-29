// <copyright file="StorageConfigurationItem{TConfiguration}.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Configuration
{
    using System.Collections.Generic;

    /// <summary>
    /// Configuration item for tenanted storage config.
    /// </summary>
    /// <typeparam name="TConfiguration">The type of storage configuration being used.</typeparam>
    /// <remarks>
    /// TODO: Is there a good reason for this to be separate from the base ConfigurationItem? Won't every
    /// ConfigurationItem have a Configuration property? There's nothing about this that is special
    /// to storage. (There sort of was back when we had a notion of a container definition, but
    /// that has now gone.)
    /// The ConfigurationKey is under here, because it's not present in legacy ConfigurationItems,
    /// but that doesn't necessarily make sense. Anything except legacy config items should have that.
    /// </remarks>
    public abstract class StorageConfigurationItem<TConfiguration> : ConfigurationItem
        where TConfiguration : class
    {
        /// <summary>
        /// Gets or sets the key under which this configuration is stored in the tenant properties.
        /// </summary>
#nullable disable annotations
        public string ConfigurationKey { get; set; }
#nullable restore annotations

        /// <summary>
        /// Gets or sets the storage configuration.
        /// </summary>
#nullable disable annotations
        public TConfiguration Configuration { get; set; }
#nullable restore annotations

        /// <inheritdoc/>
        public override IList<string> Validate()
        {
            var errors = new List<string>();

            if (this.ConfigurationKey == null)
            {
                errors.Add("The configuration item does not contain a value for the ConfigurationKey property.");
            }

            if (this.Configuration == null)
            {
                errors.Add("The configuration item does not contain a value for the Configuration property.");
            }

            return errors;
        }
    }
}