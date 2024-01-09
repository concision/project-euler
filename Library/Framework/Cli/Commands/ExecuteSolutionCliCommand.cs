using System.Diagnostics;
using System.Reflection;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Net.ProjectEuler.Framework.Api;
using Net.ProjectEuler.Framework.Hooks;
using static Crayon.Output;

namespace Net.ProjectEuler.Framework.Cli.Commands;

[Verb("execute", HelpText = "Executes a specific solution")]
public sealed class ExecuteSolutionArgs : CliArgs
{
}

/// <summary>
/// TODO: split up a lot of these SolutionMethod implementation stuff to services so other CLI commands can utilize them
/// TODO: document this
/// </summary>
/// <param name="serviceProvider"></param>
/// <param name="logger"></param>
/// <param name="solutionProviders"></param>
/// <param name="benchmarkSelectors"></param>
public class ExecuteSolutionCliCommand(
    IServiceProvider serviceProvider,
    ILogger logger,
    IEnumerable<ISolutionProvider> solutionProviders,
    IEnumerable<ISolutionSelector> benchmarkSelectors,
    ISolutionGrouper solutionGrouper
) : CliCommand<ExecuteSolutionArgs>
{
    private readonly IReadOnlyList<ISolutionProvider> solutionProviders = solutionProviders.ToArray();
    private readonly IReadOnlyList<ISolutionSelector> benchmarkSelectors = benchmarkSelectors.ToArray();
    private readonly ISolutionGrouper solutionGrouper = solutionGrouper;

    public override async Task<int> ExecuteAsync(ExecuteSolutionArgs args)
    {
        var discoverSolutions = await DiscoverSolutions(args);
        var filteredSolutions = await FilterSolution(args, discoverSolutions);
        var groupedSolutions = solutionGrouper.Group(filteredSolutions);

        foreach ((string displayName, IReadOnlyList<SolutionMethod> solutions) in groupedSolutions)
        {
            drawLine(displayName);

            for (var i = 0; i < solutions.Count; i++)
            {
                var solution = solutions[i];
                logger.LogInformation("Invoking '" + Yellow().Text($"{solution.SolverType.FullName}.{solution.Method.Name}") + "'");
                await ExecuteSolution(solution);
                if (i != solutions.Count - 1)
                    logger.LogInformation("");
            }
        }
        if (filteredSolutions.Any())
            drawLine();

        return 0;
    }

    private void drawLine(string text = "")
    {
        var width = (128.0 - text.Length - (!string.IsNullOrWhiteSpace(text) ? 2 : 0)) / 2.0;
        // TODO: figure out how to remove timestamp prefix
        logger.LogInformation(
            Blue().Text(new string(new char[(int) Math.Floor(width)]).Replace((char) 0, '='))
            + (!string.IsNullOrWhiteSpace(text) ? $" {Bold().Yellow().Text(text)} " : "")
            + Blue().Text(new string(new char[(int) Math.Ceiling(width)]).Replace((char) 0, '='))
        );
    }

    private async Task<IReadOnlyList<SolutionMethod>> DiscoverSolutions(ExecuteSolutionArgs args)
    {
        var methods = new List<MethodInfo>();
        foreach (var solutionProvider in solutionProviders)
        {
            try
            {
                var discoveredMethods = (await solutionProvider.DiscoverAllSolutions(args)).ToArray();
                logger.LogTrace($"Discovered {discoveredMethods.Length} benchmark method(s) from class '{solutionProvider.GetType().FullName}'");
                methods.AddRange(discoveredMethods);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, $"Failed to collect solutions from invoking '{nameof(ISolutionProvider)}.{nameof(ISolutionProvider.DiscoverAllSolutions)}()'" +
                                           $" for class '{solutionProvider.GetType().FullName}'");
            }
        }
        var discoveredBenchmarks = methods.Distinct().Select(solution => new SolutionMethod(solution)).ToArray();
        logger.LogTrace($"Discovered {discoveredBenchmarks.Length} unique benchmark method(s)");
        return discoveredBenchmarks;
    }

    private async Task<IReadOnlyList<SolutionMethod>> FilterSolution(ExecuteSolutionArgs args, IReadOnlyList<SolutionMethod> solutionMethods)
    {
        var allSelectedBenchmarks = new List<SolutionMethod>();
        foreach (var benchmarkSelector in benchmarkSelectors)
        {
            try
            {
                var selectedBenchmarks = (await benchmarkSelector.SelectBenchmarksAsync(args, solutionMethods)).ToArray();
                logger.LogTrace($"Selected {selectedBenchmarks.Length} benchmark(s) from class '{benchmarkSelector.GetType().FullName}':");
                foreach (var benchmarkMethod in selectedBenchmarks)
                    logger.LogTrace($"- {benchmarkMethod.SolverType.FullName}.{benchmarkMethod.Method.Name}() ({benchmarkMethod.BenchmarkAttribute?.File ?? "<unknown source>"})");
                allSelectedBenchmarks.AddRange(selectedBenchmarks);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, $"Failed to select solutions from invoking '{nameof(ISolutionSelector)}.{nameof(ISolutionSelector.SelectBenchmarksAsync)}()'" +
                                           $" for class '{benchmarkSelector.GetType().FullName}'");
            }
        }
        allSelectedBenchmarks = allSelectedBenchmarks.DistinctBy(method => method.Method).ToList();
        logger.LogTrace($"Selected {allSelectedBenchmarks.Count} unique benchmark method(s) to execute");
        return allSelectedBenchmarks;
    }

    private object BootstrapSolver(SolutionMethod solution)
    {
        // initialize an object of the method's declaring class
        var instance = ActivatorUtilities.CreateInstance(serviceProvider, solution.SolverType);

        // inject all properties annotated with [InjectService] attribute
        const BindingFlags flags = BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.NonPublic;
        var members = Enumerable.Concat<MemberInfo>(
                solution.SolverType.GetProperties(flags),
                solution.SolverType.GetFields(flags)
            )
            .Where(property => property.GetCustomAttributes<InjectServiceAttribute>().Any());
        foreach (var member in members)
            if (member is FieldInfo field)
                field.SetValue(instance, serviceProvider.GetService(field.FieldType));
            else if (member is PropertyInfo property)
                property.SetValue(instance, serviceProvider.GetService(property.PropertyType));

        return instance;
    }

    private async Task ExecuteSolution(SolutionMethod solution)
    {
        object instance = BootstrapSolver(solution);
        // TODO: set [Include]s
        // TODO: override [Parameter]s
        // TODO: invoke [Prepare]

        // invoke benchmark method
        Stopwatch stopwatch = Stopwatch.StartNew();
        // TODO: inject parameters based on IServiceProvider
        object? answer = solution.Method.Invoke(instance, solution.Method.GetParameters().Select(parameter => serviceProvider.GetService(parameter.ParameterType)).ToArray());
        if (answer is Task task)
            answer = task.GetType()
                .GetProperty("Result", BindingFlags.Instance | BindingFlags.Public)!
                .GetValue(task);
        var elapsed = stopwatch.Elapsed;

        // generate summary
        // TODO: generate table
        logger.LogInformation(Yellow().Text("Elapsed Time") + White().Text(": ") + Green().Text(elapsed.ToString()));
        for (var type = instance.GetType(); type != null; type = type.BaseType)
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Solver<>))
            {
                answer ??= ((dynamic) instance).Answer;
                break;
            }
        logger.LogInformation(Yellow().Text("Answer") + White().Text(": ") +
                              (answer is not null ? Green().Text(answer.ToString()!) : Red().Text("<none>")));
    }
}