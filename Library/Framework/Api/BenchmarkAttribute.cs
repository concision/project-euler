using Net.ProjectEuler.Framework.Api.Hooks;
using Net.ProjectEuler.Framework.Hooks;

namespace Net.ProjectEuler.Framework.Api;

/// <summary>
/// <para>
/// Annotates a class's method as a benchmark method that can be invoked. This functionality is similar to MSTest's
/// <c>[TestMethod]</c> or BenchmarkDotNet's <c>[Benchmark]</c> attribute. The declaring class should be annotated with
/// <see cref="BenchmarkClassAttribute"/> (or a derived type).
/// </para>
/// <para>
/// Any method annotated with this attribute (and its declaring class annotated) will be automatically detected by the
/// default <see cref="IBenchmarkProvider"/> implementation.
/// </para>
/// </summary>
/// 
/// <remarks>
/// <para>
/// This attribute may be derived to provide additional functionality for custom <see cref="IBenchmarkProvider"/>
/// implementations.
/// </para>
/// <para>
/// The method implementation is expected to be a pure function (i.e. no side effects).
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public class BenchmarkMethodAttribute : Attribute
{
    /// <summary>
    /// If <c>true</c>, indicates that this method should not be ignored by a <see cref="IBenchmarkProvider"/>.
    /// Otherwise if <c>false</c> (default), the method will be considered valid for discovery.
    /// </summary>
    /// <remarks>
    /// This is particularly useful for ignoring test benchmark methods that take too long to execute in a reasonable
    /// timeframe.
    /// </remarks>
    public bool DisabledByDefault { get; init; } = false;

    /// <summary>
    /// A human-friendly display name for this benchmark method.
    /// </summary>
    public string? DisplayName { get; init; }
}
