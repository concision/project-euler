using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Net.ProjectEuler.Framework.Logging;

namespace Net.ProjectEuler.Framework;

public class ProjectEulerStartup : IDisposable
{
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


    public IServiceProvider? ServiceProvider { get; private set; }

    private bool isDisposed;

    protected virtual void Bootstrap(IServiceCollection? serviceCollection = null, Action<IServiceCollection>? setupCollection = null)
    {
        serviceCollection ??= new ServiceCollection();
        serviceCollection.AddEulerBenchmarkFramework();

        serviceCollection.AddSingleton<ILoggerFactory>(_ => LoggerFactory.Create(builder =>
        {
            builder.AddFilter("*", LogLevel.Trace);
            builder.AddConsoleFormatter<EulerConsoleFormatter, CustomOptions>();

            builder.AddConsole(options =>
            {
                options.FormatterName = nameof(EulerConsoleFormatter);
                options.LogToStandardErrorThreshold = LogLevel.Warning;
            });
        }));
        serviceCollection.AddSingleton<ILogger>(serviceProvider => serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(IProjectEulerCli)));

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