using Net.ProjectEuler.Framework.Model;

namespace Net.ProjectEuler.Framework.Api.Hooks;

/// <summary>
/// Groups benchmark methods by a human-friendly display name.
/// </summary>
public interface IBenchmarkGrouper
{
    /// <inheritdoc cref="IBenchmarkGrouper"/>
    /// <param name="benchmarks">The benchmark methods to be grouped.</param>
    /// <returns>Benchmark methods grouped by a human-friendly display name.</returns>
    IEnumerable<IGrouping<string, Benchmark>> GroupExecutions(IEnumerable<Benchmark> benchmarks);
}
