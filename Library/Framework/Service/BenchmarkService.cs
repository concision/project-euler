using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Net.ProjectEuler.Framework.Api;
using Net.ProjectEuler.Framework.Api.Hooks;
using Net.ProjectEuler.Framework.Model;

namespace Net.ProjectEuler.Framework.Service;

public interface IBenchmarkService
{
    Task<IReadOnlyList<Benchmark>> ProvideBenchmarksAsync();

    Task<IReadOnlyList<Benchmark>> FilterBenchmarksAsync(IReadOnlyList<Benchmark> benchmarks);

    Task HydrateBenchmarkAsync(Benchmark benchmark);
}

public class BenchmarkService(
    IServiceProvider serviceProvider,
    ILogger logger,
    IEnumerable<IBenchmarkProvider> benchmarkProviders,
    IEnumerable<IBenchmarkSelector> benchmarkSelectors,
    IEnumerable<IResourceProvider> resourceProviders
) : IBenchmarkService
{
    private readonly IReadOnlyList<IBenchmarkProvider> solutionProviders = benchmarkProviders.ToArray();
    private readonly IReadOnlyList<IBenchmarkSelector> benchmarkSelectors = benchmarkSelectors.ToArray();
    private readonly IReadOnlyList<IResourceProvider> resourceProviders = resourceProviders.ToArray();

    public async Task<IReadOnlyList<Benchmark>> ProvideBenchmarksAsync()
    {
        var methods = new List<MethodInfo>();
        foreach (var solutionProvider in solutionProviders)
        {
            try
            {
                var discoveredMethods = (await solutionProvider.ProvideBenchmarksAsync()).ToArray();
                logger.LogTrace($"Discovered {discoveredMethods.Length} benchmark method(s) from class '{solutionProvider.GetType().FullName}'");
                methods.AddRange(discoveredMethods);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, $"Failed to collect solutions from invoking '{nameof(IBenchmarkProvider)}.{nameof(IBenchmarkProvider.ProvideBenchmarksAsync)}()'" +
                                           $" for class '{solutionProvider.GetType().FullName}'");
            }
        }
        var discoveredBenchmarks = methods.Distinct().Select(solution => new Benchmark(solution)).ToArray();
        logger.LogTrace($"Discovered {discoveredBenchmarks.Length} unique benchmark method(s)");
        return discoveredBenchmarks;
    }

    public async Task<IReadOnlyList<Benchmark>> FilterBenchmarksAsync(IReadOnlyList<Benchmark> benchmarks)
    {
        var allSelectedBenchmarks = new List<Benchmark>();
        foreach (var benchmarkSelector in benchmarkSelectors)
        {
            try
            {
                var selectedBenchmarks = (await benchmarkSelector.SelectBenchmarksAsync(benchmarks)).ToArray();
                logger.LogTrace($"Selected {selectedBenchmarks.Length} benchmark(s) from class '{benchmarkSelector.GetType().FullName}':");
                foreach (var benchmarkMethod in selectedBenchmarks)
                    logger.LogTrace($"- {benchmarkMethod.SolverType.FullName}.{benchmarkMethod.Method.Name}() ({benchmarkMethod.BenchmarkAttribute?.File ?? "<unknown source>"})");
                allSelectedBenchmarks.AddRange(selectedBenchmarks);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, $"Failed to select solutions from invoking '{nameof(IBenchmarkSelector)}.{nameof(IBenchmarkSelector.SelectBenchmarksAsync)}()'" +
                                           $" for class '{benchmarkSelector.GetType().FullName}'");
            }
        }
        allSelectedBenchmarks = allSelectedBenchmarks.DistinctBy(method => method.Method).ToList();
        logger.LogTrace($"Selected {allSelectedBenchmarks.Count} unique benchmark method(s) to execute");
        return allSelectedBenchmarks;
    }

    public async Task HydrateBenchmarkAsync(Benchmark benchmark)
    {
        benchmark.Instance = BootstrapSolver(benchmark);

        // TODO: finish include hydration
        var includes = benchmark.Parameters.Select(parameter => parameter.Include)
            .Concat(benchmark.Prepares.SelectMany(prepare => prepare.Includes.Values))
            .Where(include => include is not null)
            .Cast<ResourceAttribute>()
            .Distinct()
            .ToArray();

        var cachedIncludes = new Dictionary<string, object?>();
        foreach (var include in includes)
        {
            object? content = null;
            foreach (var resourceProvider in resourceProviders)
            {
                content ??= await resourceProvider.FetchAsync(benchmark, include);
                if (content is not null)
                    break;
            }
            cachedIncludes[include?.Key ?? ""] = content;
        }
    }

    private object BootstrapSolver(Benchmark benchmark)
    {
        // initialize an object of the method's declaring class
        var instance = ActivatorUtilities.CreateInstance(serviceProvider, benchmark.SolverType);

        // inject all properties annotated with [InjectService] attribute
        const BindingFlags flags = BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.SetProperty
                                   | BindingFlags.Public | BindingFlags.NonPublic;
        var members = Enumerable.Concat<MemberInfo>(
                benchmark.SolverType.GetProperties(flags),
                benchmark.SolverType.GetFields(flags)
            )
            .Where(property => property.GetCustomAttributes<DependencyInjectAttribute>().Any());
        foreach (var member in members)
            if (member is FieldInfo field)
                field.SetValue(instance, serviceProvider.GetService(field.FieldType));
            else if (member is PropertyInfo property)
                property.SetValue(instance, serviceProvider.GetService(property.PropertyType));

        return instance;
    }
}
