using Net.ProjectEuler.Framework.Api;
using Net.ProjectEuler.Framework.Api.Hooks;
using Net.ProjectEuler.Framework.Model;

namespace Net.ProjectEuler.Framework.Hooks;

/// <summary>
/// Groups benchmark methods by their Project Euler problem number.
/// </summary>
public class ProjectEulerBenchmarkGrouper : IBenchmarkGrouper
{
    public IEnumerable<IGrouping<string, Benchmark>> GroupExecutions(IEnumerable<Benchmark> benchmarks)
    {
        return benchmarks.GroupBy(benchmark => benchmark.BenchmarkAttribute switch
        {
            ProjectEulerAttribute attribute => $"Problem {attribute.Id}" + (attribute.DisplayName is not null ? $": {attribute.DisplayName}" : ""),
            _ => benchmark.SolverType.FullName!,
        });
    }
}
