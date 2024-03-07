using CommandLine;
using Dev.Concision.Ascii;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Net.ProjectEuler.Framework.Api.Hooks;
using Net.ProjectEuler.Framework.Cli;
using Net.ProjectEuler.Framework.Cli.Commands;
using Net.ProjectEuler.Framework.Hooks;
using Net.ProjectEuler.Framework.Logging;
using Net.ProjectEuler.Framework.Service;

namespace Net.ProjectEuler.Framework;

/// <summary>
/// Bootstraps the Project Euler benchmarking framework.
/// </summary>
public static class Bootstrapper
{
    /// <inheritdoc cref="Bootstrapper"/>
    /// <param name="serviceCollection">A <see cref="IServiceCollection"/> to add framework dependencies.</param>
    public static TServiceCollection AddEulerBenchmarkFramework<TServiceCollection>(this TServiceCollection serviceCollection)
        where TServiceCollection : IServiceCollection
    {
        // logging
        serviceCollection.AddSingleton<ILoggerFactory>(_ => LoggerFactory.Create(builder =>
        {
            builder.AddFilter("*", LogLevel.Information);
            builder.AddConsoleFormatter<EulerConsoleFormatter, EulerLoggingOptions>(options => { options.ExceptionIndentation = false; });

            builder.AddConsole(options =>
            {
                options.FormatterName = EulerConsoleFormatter.Name;
                options.LogToStandardErrorThreshold = LogLevel.Warning;
            });
        }));
        serviceCollection.AddSingleton<ILogger>(serviceProvider => serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(IProjectEulerCli)));

        // command line parsing
        serviceCollection.AddSingleton<Parser>(_ => new Parser(settings =>
        {
            // This disables CommandLineParser's default behavior of erroring out if it encounters an unknown CLI flag during CLI argument parsing
            settings.IgnoreUnknownArguments = true;
            settings.MaximumDisplayWidth = 120;
        }));
        serviceCollection.AddScoped<CliCommand, TestBenchmarkCliCommand>();
        serviceCollection.AddSingleton<IProjectEulerCli, ProjectEulerCli>();

        // [Benchmark]-related services
        serviceCollection.AddTransient<IBenchmarkService, BenchmarkService>();
        serviceCollection.AddTransient<IBenchmarkProvider, BenchmarkProvider>();
        serviceCollection.AddTransient<IBenchmarkSelector, LastModifiedSourceFileSelector>();
        serviceCollection.AddTransient<IBenchmarkGrouper, ProjectEulerBenchmarkGrouper>();

        // generic utilities
        serviceCollection.AddTransient<IStopwatchService, StopwatchService>();
        serviceCollection.AddTransient<ITreeListRenderer, TreeListRenderer>();

        return serviceCollection;
    }
}
