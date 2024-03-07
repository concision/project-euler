using System.Reflection;

namespace Net.ProjectEuler.Framework.Api.Hooks;

/// <summary>
/// Provides a list of benchmark methods that may be executed.
/// </summary>
public interface IBenchmarkProvider
{
    /// <inheritdoc cref="IBenchmarkProvider"/>
    /// <returns>A list of benchmark method's <see cref="MethodInfo"/>.</returns>
    Task<IEnumerable<MethodInfo>> ProvideBenchmarksAsync();
}
