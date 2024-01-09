using Net.ProjectEuler.Framework.Cli.Commands;
using Net.ProjectEuler.Framework.Service;

namespace Net.ProjectEuler.Framework.Hooks;

/// <summary>
/// TODO: implementations for Windows/Linux reading IDE process data to infer the best solution to run
/// </summary>
public interface ISolutionSelector
{
    Task<IEnumerable<SolutionMethod>> SelectBenchmarksAsync(IEnumerable<SolutionMethod> methods);
}

public class LastModifiedSourceFileSelector : ISolutionSelector
{
    public Task<IEnumerable<SolutionMethod>> SelectBenchmarksAsync(IEnumerable<SolutionMethod> methods)
    {
        var methodsByLastModifiedSourceFile = methods
            .Select(benchmarkMethod => (
                Method: benchmarkMethod,
                LastModified: benchmarkMethod.BenchmarkAttribute is not null && File.Exists(benchmarkMethod.BenchmarkAttribute.File)
                    ? File.GetLastWriteTime(benchmarkMethod.BenchmarkAttribute.File)
                    : DateTime.MinValue
            ))
            .Where(method => method.LastModified != DateTime.MinValue)
            .OrderByDescending(method => method.LastModified)
            .ToArray();
        var lastModifiedSolverType = methodsByLastModifiedSourceFile.FirstOrDefault().Method?.SolverType;
        return Task.FromResult(methodsByLastModifiedSourceFile.Where(method => method.Method.SolverType == lastModifiedSolverType)
            .Select(method => method.Method));
    }
}