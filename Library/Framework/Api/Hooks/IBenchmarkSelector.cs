using Net.ProjectEuler.Framework.Model;
using Net.ProjectEuler.Framework.Service;

namespace Net.ProjectEuler.Framework.Api.Hooks;

// TODO: implementations for Windows/Linux reading IDE process data to infer the best solution to run
/// <summary>
/// Filters a list of benchmark methods to be executed.
/// </summary>
public interface IBenchmarkSelector
{
    /// <inheritdoc cref="IBenchmarkSelector"/>
    /// <param name="benchmarks">A list of benchmark methods to select to execute.</param>
    /// <returns>A subset of <paramref name="benchmarks"/>.</returns>
    Task<IEnumerable<Benchmark>> SelectBenchmarksAsync(IEnumerable<Benchmark> benchmarks);
}
