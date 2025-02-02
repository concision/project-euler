﻿using Microsoft.Extensions.DependencyInjection;

namespace Net.ProjectEuler.Framework;

/// <summary>
/// Start entrypoint for bootstrapping the Project Euler framework as a CLI application.
/// </summary> 
public class ProjectEulerStartup : IDisposable
{
    #region Entrypoints

    public static async Task<int> Main(params string[] args)
    {
        return await Main<ProjectEulerStartup>(args);
    }

    public static async Task<int> Main<T>(params string[] args)
        where T : ProjectEulerStartup, new()
    {
        using var startup = new T();
        startup.Bootstrap();
        return await startup.ExecuteCliCommand(args);
    }

    #endregion


    public IServiceProvider? ServiceProvider { get; private set; }

    protected virtual void Bootstrap(IServiceCollection? serviceCollection = null, Action<IServiceCollection>? setupCollection = null)
    {
        serviceCollection ??= new ServiceCollection();
        serviceCollection.AddEulerBenchmarkFramework();

        BootstrapServiceCollection(serviceCollection);
        setupCollection?.Invoke(serviceCollection);

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    protected virtual void BootstrapServiceCollection(IServiceCollection serviceCollection)
    {
    }

    public async Task<int> ExecuteCliCommand(params string[] cliArguments)
    {
        if (ServiceProvider is null)
            throw new InvalidOperationException($"{nameof(ProjectEulerStartup)} must be bootstrapped first via {nameof(Bootstrap)}()");

        var projectEuler = ServiceProvider.GetRequiredService<IProjectEulerCli>();
        return await projectEuler.ExecuteCliCommand(cliArguments);
    }


    #region IDisposable

    private bool isDisposed;

    protected virtual void Dispose(bool disposing)
    {
        if (isDisposed)
            return;

        isDisposed = true;
        if (ServiceProvider is IDisposable disposable)
            disposable.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~ProjectEulerStartup()
    {
        Dispose(false);
    }

    #endregion
}
