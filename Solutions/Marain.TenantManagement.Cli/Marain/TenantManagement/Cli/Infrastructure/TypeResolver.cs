// <copyright file="TypeResolver.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli;

using System;

using Microsoft.Extensions.DependencyInjection;

using Spectre.Console.Cli;

/// <summary>
/// A type resolver that integrates Spectre.Console.Cli with Microsoft.Extensions.DependencyInjection.
/// </summary>
public sealed class TypeResolver : ITypeResolver, IDisposable
{
    private readonly ServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeResolver"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider to use for resolving dependencies.</param>
    public TypeResolver(ServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc />
    public object? Resolve(Type? type)
    {
        if (type == null)
        {
            return null;
        }

        return this.serviceProvider.GetService(type);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.serviceProvider?.Dispose();
    }
}