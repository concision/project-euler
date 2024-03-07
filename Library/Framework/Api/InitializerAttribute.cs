namespace Net.ProjectEuler.Framework.Api;

/// <summary>
/// Annotates a class's method as an initializer method that should be invoked prior to any
/// <see cref="BenchmarkMethodAttribute"/>-annotated methods. The declaring class should be annotated with
/// <see cref="BenchmarkClassAttribute"/> (or a derived type). An initializer method may declare any number of
/// parameters (in any arbitrary order) that are annotated with <see cref="ResourceAttribute"/> or
/// <see cref="DependencyInjectAttribute"/> to indicate that the framework should provide a resource value or dependency
/// injection service, respectively. Unannotated parameters are considered an error by the default argument provider
/// implementation.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class InitializerAttribute : Attribute
{
    /// <summary>
    /// Indicates the execution priority of this initializer method. Initializer methods are executed in ascending order
    /// based on their priority (i.e. lower value methods are executed first). If two or more initializer methods have
    /// the same priority, the order of declaration takes precedence (i.e. the first declared method is executed first).
    /// </summary>
    /// <remarks>
    /// Ideally most benchmark classes will only have one initializer method, but this is useful for cases where
    /// multiple initializers may be required.
    /// </remarks>
    public int Priority { get; init; }
}
