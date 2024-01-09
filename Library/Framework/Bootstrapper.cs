using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Net.ProjectEuler.Framework.Cli;
using Net.ProjectEuler.Framework.Cli.Commands;
using Net.ProjectEuler.Framework.Hooks;
using Net.ProjectEuler.Framework.Service;

namespace Net.ProjectEuler.Framework;

public static class Bootstrapper
{
    /// <summary>
    /// High level flow:
    /// - Parse CLI arguments (if any)
    /// - Collect all methods to benchmark (solutions to run)
    /// - Benchmark Solutions
    /// 
    /// High-level Project Euler Framework Flow:
    /// - Parse CLI arguments from Main(string[])
    /// - Collect all benchmark methods (MethodInfo) from ISolutionProvider[]s (maybe verify it derives from the Solver class?)
    /// - Filter benchmark methods using ISolverSelector (verify only a proper MethodInfo subset was returned)
    /// - Hydrate MethodInfo into BenchmarkMethod
    /// - Build Execution Plan using IExecutionPlanner (group problems by [ProjectEuler].Id)
    /// - Foreach benchmark method group:
    ///   - Foreach benchmark method:
    ///     - Fetch any requested includes
    ///     - if 1 < benchmarkMethod.iterations():
    ///       - use no-op logger for Solver<T> property
    ///     - For each benchmarkMethod.iterations():
    ///       - Initialize all inherited Solver<T> properties
    ///       - Initialize all [Parameter]-annotated fields/properties (which may use a requested include)
    ///       - Execute all [Prepare]-annotated methods (which may use a requested include)
    ///       - If state caching is on, cache the state of the entire
    ///       - Invoke corresponding [Solution] method
    ///   - Render table of data for all
    ///   - If on, render tree view of recursive timing stacks (e.g. initialization overhead [includes, prepares, solution] & solution timings)
    /// Levels of Solver&lt;T&gt; state caching:
    /// - DISCARD_INSTANCE: Discards each Solver instance immediately
    /// - DISCARD_MEMBERS: After each [Solution] invocation, all public fields/properties are reset to their deeply cloned values after [Prepare]s
    /// - NONE: do not discard state
    /// </summary>
    /// <param name="serviceCollection"></param>
    public static TServiceCollection AddEulerBenchmarkFramework<TServiceCollection>(this TServiceCollection serviceCollection)
        where TServiceCollection : IServiceCollection
    {
        // overridden by consumer
        serviceCollection.AddSingleton<ILogger>(_ => NullLogger<IProjectEulerCli>.Instance);

        // command line parsing
        serviceCollection.AddSingleton<Parser>(_ => new Parser(settings =>
        {
            // This disables CommandLineParser's default behavior of erroring out if it encounters an unknown CLI flag during CLI argument parsing
            settings.IgnoreUnknownArguments = true;
            settings.MaximumDisplayWidth = 120;
        }));
        serviceCollection.AddScoped<CliCommand, ExecuteSolutionCliCommand>();
        serviceCollection.AddSingleton<IProjectEulerCli, ProjectEulerCli>();

        // [Solution]-related services
        serviceCollection.AddTransient<ISolutionService, SolutionService>();
        serviceCollection.AddTransient<ISolutionProvider, SolutionProvider>();
        serviceCollection.AddTransient<ISolutionSelector, LastModifiedSourceFileSelector>();
        serviceCollection.AddTransient<ISolutionGrouper, ProjectEulerAttributeSolutionGrouper>();

        // generic utilities
        serviceCollection.AddTransient<IStopwatchService, StopwatchService>();
        serviceCollection.AddTransient<ITreeListRenderer, TreeListRenderer>();

        return serviceCollection;
    }
}