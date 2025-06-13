// <copyright file="TypeRegistrar.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Cli;

using System;

using Microsoft.Extensions.DependencyInjection;

using Spectre.Console.Cli;

/// <summary>
/// A type registrar that integrates Spectre.Console.Cli with Microsoft.Extensions.DependencyInjection.
/// </summary>
public sealed class TypeRegistrar : ITypeRegistrar
{
    private readonly ServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeRegistrar"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider to use for resolving dependencies.</param>
    public TypeRegistrar(ServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc />
    public ITypeResolver Build()
    {
        return new TypeResolver(this.serviceProvider);
    }

    /// <inheritdoc />
    public void Register(Type service, Type implementation)
    {
        // Not needed when using an existing service provider
    }

    /// <inheritdoc />
    public void RegisterInstance(Type service, object implementation)
    {
        // Not needed when using an existing service provider
    }

    /// <inheritdoc />
    public void RegisterLazy(Type service, Func<object> factory)
    {
        // Not needed when using an existing service provider
    }
}