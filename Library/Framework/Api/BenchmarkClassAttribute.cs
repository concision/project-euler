using System.Runtime.CompilerServices;
using Net.ProjectEuler.Framework.Api.Hooks;
using Net.ProjectEuler.Framework.Hooks;

namespace Net.ProjectEuler.Framework.Api;

/// <summary>
/// <para>
/// Annotates a <see langword="class"/> as a "benchmark class", which contains
/// <see cref="BenchmarkMethodAttribute"/>-annotated "benchmark methods" that are invokable. This attribute
/// functionality is similar to MSTest's <c>[TestClass]</c> attribute - any class with this attribute will function as a
/// test class container that has test methods (<see cref="BenchmarkMethodAttribute"/>-annotated methods, or other
/// derived attribute types) eligible for discovery and execution.
/// </para>
/// 
/// <para>
/// Benchmark classes must be public and non-abstract. A new object instance is instantiated for each benchmark method
/// to ensure there is no state contamination across invocations. There are several supported lifecycle hooks for a
/// benchmark class object instance (which is executed for each individual benchmark method instance):
/// <list type="bullet">
/// <item><description>
/// <c>new(...)</c> constructor: An optional constructor that has its arguments automatically dependency injected
/// from the current dependency injection container (i.e. <see cref="IServiceProvider"/>). Note that all parameters are
/// expected to be dependency injectable services (i.e. <see cref="ResourceAttribute"/> is <i>not</i> supported).
/// </description></item>
/// <item><description>
/// <see cref="ResourceAttribute"/>: Any fields, properties, initializer method parameters, or benchmark method
/// parameters annotated with this attribute will be automatically provided a value from a
/// <see cref="IResourceProvider"/> implementation.
/// </description></item>
/// <item><description>
/// <see cref="DependencyInjectAttribute"/>: Any fields, properties, initializer method parameters, or benchmark method
/// parameters annotated with this attribute will be automatically dependency injected a service from the current
/// dependency injection container (i.e. <see cref="IServiceProvider"/>).
/// </description></item>
/// <item><description>
/// <see cref="InitializerAttribute"/>: An initializer method that is invoked before any benchmark method is invoked.
/// If there are multiple initializers, they are invoked in ascending order primarily by
/// <see cref="InitializerAttribute.Priority"/> and secondarily the declaration order. Initializers may declare
/// parameter dependencies with <see cref="ResourceAttribute"/> or <see cref="DependencyInjectAttribute"/>. Unannotated
/// parameters are not supported and will cause an exception.
/// </description></item>
/// <item><description>
/// <see cref="BenchmarkMethodAttribute"/>: An invokable method that is to be performance benchmarked by the framework.
/// The method implementation is expected to be a pure function (i.e. no side effects). This functionality is similar to
/// MSTest's <c>[TestMethod]</c> or BenchmarkDotNet's <c>[Benchmark]</c> attribute. Each benchmark method is executed
/// in isolation from other benchmark methods in the same class via isolated object instances.
/// </description></item>
/// </list>
/// </para>
/// 
/// <para>
/// The lifecycle hooks are executed in the following order to fully invoke a benchmark method:
/// <list type="number">
/// <item><description>
/// The benchmark class's <c>new(...)</c> constructor is invoked and dependency injected with services from
/// <see cref="IServiceProvider"/>.
/// </description></item>
/// <item><description>
/// Any fields or properties with <see cref="ResourceAttribute"/> or <see cref="DependencyInjectAttribute"/> are
/// automatically provided a value from <see cref="IResourceProvider"/> or <see cref="IServiceProvider"/>, respectively.
/// </description></item>
/// <item><description>
/// Any initializer methods (<see cref="InitializerAttribute"/>-annotated methods) will be invoked in the ascending
/// order by <see cref="InitializerAttribute.Priority"/>. Any parameters of the initializer annotated with
/// <see cref="ResourceAttribute"/> or <see cref="DependencyInjectAttribute"/> are automatically provided a value from
/// <see cref="IResourceProvider"/> or <see cref="IServiceProvider"/>, respectively. Unannotated parameters are not
/// supported and will cause an exception.
/// </description></item>
/// <item><description>
/// The relevant benchmark method (i.e. <see cref="BenchmarkMethodAttribute"/>-annotated method) is invoked. Any
/// parameters of the benchmark method annotated with <see cref="ResourceAttribute"/> or
/// <see cref="DependencyInjectAttribute"/> are automatically provided a value from <see cref="IResourceProvider"/> or
/// <see cref="IServiceProvider"/>, respectively. Unannotated parameters are not supported and will cause an exception.
/// </description></item> // TODO: document ParallelBenchmark type
/// <item><description>
/// If the annotated benchmark class implements <see cref="IAsyncDisposable"/>, then
/// <see cref="IAsyncDisposable.DisposeAsync"/> is invoked and awaited. Otherwise, if the annotated benchmark class
/// implements <see cref="IDisposable"/>, then <see cref="IDisposable.Dispose"/> is invoked.
/// </description></item>
/// </list>
/// </para>
/// </summary>
/// 
/// <remarks>
/// <para>
/// There is a derived type of this attribute for <a href="https://projecteuler.net/">Project Euler</a> problems, see
/// <see cref="ProjectEulerAttribute"/> documentation for the additional attributes and lifecycle hooks.
/// </para>
/// <para>
/// If this attribute is derived, the derived attribute class is required to implement <see cref="File"/> with
/// <see cref="CallerFilePathAttribute"/> and <see cref="Line"/> with <see cref="CallerLineNumberAttribute"/>.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public class BenchmarkClassAttribute : Attribute
{
    /// <summary>
    /// The annotated class's development source system file path.
    /// </summary>
    /// <example>On Windows: <c>C:\Users\Xyz\Desktop\ProjectEuler\SourceFile.cs</c></example>
    public string File { get; protected init; }

    /// <summary>
    /// The annotated class's source file line number.
    /// </summary>
    public int Line { get; protected init; }


    /// <summary>
    /// A human-friendly display name for this benchmark class.
    /// </summary>
    public string? DisplayName { get; init; }

#if FALSE // TODO: implement object lifecycle that is shared across benchmark method invocations
    /// <summary>
    /// Indicates the lifecycle of a benchmark class object instance when invoking a benchmark method (e.g. object
    /// instance may be reused, or disposed).
    /// </summary>
    public LifeCycle Instance { get; init; } = LifeCycle.New;

    public enum LifeCycle
    {
        /// <summary>
        /// A new object instance of the benchmark class is created for each benchmark method invocation. Any state
        /// changes from a benchmark method will not be preserved for the next benchmark method invocation.
        /// </summary>
        New,

        /// <summary>
        /// The same object instance of the benchmark class is used for each benchmark method invocation. Any state
        /// changes from a benchmark method will be preserved for the next benchmark method invocation.
        /// </summary>
        Shared,

        /// <summary>
        /// If the the object instance state of the benchmark class does not change between benchmark method invocations,
        /// the same object instance is reused. Otherwise if the state changes, a new object instance is for the next
        /// benchmark method invocation.
        /// </summary>
        Auto, // Analyzing for state changes may be more expensive than initializing a new object instance, so this might be a pointless flag
    }
#endif

    /// <summary><inheritdoc cref="BenchmarkClassAttribute" path="/summary"/></summary>
    /// <param name="file">Automatically provided by <see cref="CallerFilePathAttribute"/>; see <see cref="File"/>.</param>
    /// <param name="line">Automatically provided by <see cref="CallerLineNumberAttribute"/> ; see <see cref="Line"/>.</param>
    public BenchmarkClassAttribute([CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        File = file;
        Line = line;
    }
}
