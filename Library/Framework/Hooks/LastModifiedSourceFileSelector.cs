using Net.ProjectEuler.Framework.Api.Hooks;
using Net.ProjectEuler.Framework.Model;

namespace Net.ProjectEuler.Framework.Hooks;

/// <summary>
/// Selects the benchmark methods with the most recently modified source file.
/// </summary>
public class LastModifiedSourceFileSelector : IBenchmarkSelector
{
    public Task<IEnumerable<Benchmark>> SelectBenchmarksAsync(IEnumerable<Benchmark> benchmarks)
    {
        var methodsByLastModified = benchmarks
            .Select(benchmarkMethod => (
                Method: benchmarkMethod,
                LastModified: benchmarkMethod.BenchmarkAttribute is not null && File.Exists(benchmarkMethod.BenchmarkAttribute.File)
                    ? File.GetLastWriteTime(benchmarkMethod.BenchmarkAttribute.File)
                    : DateTime.MinValue
            ))
            .Where(method => method.LastModified != DateTime.MinValue)
            .OrderByDescending(method => method.LastModified)
            .ToArray();
        var lastModifiedSolverType = methodsByLastModified.FirstOrDefault().Method?.SolverType;
        var benchmarksInFile = methodsByLastModified
            .Where(method => method.Method.SolverType == lastModifiedSolverType)
            .Select(method => method.Method);
        return Task.FromResult(benchmarksInFile);
    }
}
