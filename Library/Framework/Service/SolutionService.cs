using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Net.ProjectEuler.Framework.Api;
using Net.ProjectEuler.Framework.Cli.Commands;
using Net.ProjectEuler.Framework.Hooks;

namespace Net.ProjectEuler.Framework.Service;

public interface ISolutionService
{
    Task<IReadOnlyList<SolutionMethod>> DiscoverSolutions();

    Task<IReadOnlyList<SolutionMethod>> FilterSolution(IReadOnlyList<SolutionMethod> solutionMethods);

    Task HydrateSolutionAsync(SolutionMethod solution);
}

// TODO: move logging to caller
public class SolutionService(
    IServiceProvider serviceProvider,
    ILogger logger,
    IEnumerable<ISolutionProvider> solutionProviders,
    IEnumerable<ISolutionSelector> benchmarkSelectors,
    IEnumerable<IIncludeProvider> includeProviders
) : ISolutionService
{
    private readonly IReadOnlyList<ISolutionProvider> solutionProviders = solutionProviders.ToArray();
    private readonly IReadOnlyList<ISolutionSelector> benchmarkSelectors = benchmarkSelectors.ToArray();
    private readonly IReadOnlyList<IIncludeProvider> includeProviders = includeProviders.ToArray();

    public async Task<IReadOnlyList<SolutionMethod>> DiscoverSolutions()
    {
        var methods = new List<MethodInfo>();
        foreach (var solutionProvider in solutionProviders)
        {
            try
            {
                var discoveredMethods = (await solutionProvider.DiscoverAllSolutions()).ToArray();
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

    public async Task<IReadOnlyList<SolutionMethod>> FilterSolution(IReadOnlyList<SolutionMethod> solutionMethods)
    {
        var allSelectedBenchmarks = new List<SolutionMethod>();
        foreach (var benchmarkSelector in benchmarkSelectors)
        {
            try
            {
                var selectedBenchmarks = (await benchmarkSelector.SelectBenchmarksAsync(solutionMethods)).ToArray();
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

    public async Task HydrateSolutionAsync(SolutionMethod solution)
    {
        solution.Instance = BootstrapSolver(solution);

        // TODO: finish
        var includes = solution.Parameters.Select(parameter => parameter.Include)
            .Concat(solution.Prepares.SelectMany(prepare => prepare.Includes.Values))
            .Where(include => include is not null)
            .Cast<IncludeAttribute>()
            .Distinct()
            .ToArray();

        var cachedIncludes = new Dictionary<string, string?>();
        foreach (var include in includes)
        {
            string? content = null;
            foreach (var includeProvider in includeProviders)
            {
                content ??= await includeProvider.FetchAsync(solution, include);
                if (content is not null)
                    break;
            }
            cachedIncludes[include?.Key ?? ""] = content;
        }
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
}