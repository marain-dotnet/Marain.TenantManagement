// <copyright file="InMemoryTenantProvider.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Specs.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.Tenancy;
    using Corvus.Tenancy.Exceptions;

    /// <summary>
    /// In-memory implementation of ITenantProvider.
    /// </summary>
    public class InMemoryTenantProvider : ITenantProvider
    {
        private readonly IJsonSerializerSettingsProvider jsonSerializerSettingsProvider;
        private readonly List<ITenant> allTenants = new List<ITenant>();
        private readonly Dictionary<ITenant, List<ITenant>> tenantsByParent = new Dictionary<ITenant, List<ITenant>>();

        public InMemoryTenantProvider(RootTenant rootTenant, IJsonSerializerSettingsProvider jsonSerializerSettingsProvider)
        {
            this.Root = rootTenant;
            this.allTenants.Add(this.Root);
            this.jsonSerializerSettingsProvider = jsonSerializerSettingsProvider;
        }

        public ITenant Root { get; }

        public async Task<ITenant> CreateChildTenantAsync(string parentTenantId, string name)
        {
            ITenant parent = await this.GetTenantAsync(parentTenantId).ConfigureAwait(false);
            var newTenant = new Tenant(this.jsonSerializerSettingsProvider)
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
            };

            List<ITenant> childrenList = this.GetChildren(parent);
            childrenList.Add(newTenant);
            this.allTenants.Add(newTenant);

            return newTenant;
        }

        public Task DeleteTenantAsync(string tenantId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<TenantCollectionResult> GetChildrenAsync(string tenantId, int limit = 20, string? continuationToken = null)
        {
            ITenant parent = await this.GetTenantAsync(tenantId);

            List<ITenant> children = this.GetChildren(parent);

            int skip = 0;

            if (!string.IsNullOrEmpty(continuationToken))
            {
                skip = int.Parse(continuationToken);
            }

            IEnumerable<string> tenants = children.Skip(skip).Take(limit).Select(x => x.Id);

            continuationToken = tenants.Count() == limit ? (skip + limit).ToString() : null;

            return new TenantCollectionResult(tenants, continuationToken);
        }

        public Task<ITenant> GetTenantAsync(string tenantId, string? eTag = null)
        {
            ITenant? tenant = this.allTenants.Find(x => x.Id == tenantId);

            if (tenant == null)
            {
                throw new TenantNotFoundException();
            }

            return Task.FromResult(tenant);
        }

        public Task<ITenant> UpdateTenantAsync(ITenant tenant)
        {
            throw new System.NotImplementedException();
        }

        public ITenant? GetTenantByName(string name)
        {
            return this.allTenants.Find(x => x != this.Root && x.Name == name);
        }

        public List<ITenant> GetChildren(ITenant parent)
        {
            if (!this.tenantsByParent.TryGetValue(parent, out List<ITenant>? children))
            {
                children = new List<ITenant>();
                this.tenantsByParent.Add(parent, children);
            }

            return children;
        }
    }
}
