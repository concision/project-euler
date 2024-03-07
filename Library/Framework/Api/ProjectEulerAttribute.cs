using System.Runtime.CompilerServices;
using Net.ProjectEuler.Framework.Api.Hooks;
using Net.ProjectEuler.Framework.Hooks;

namespace Net.ProjectEuler.Framework.Api;

/// <summary>
/// <para>
/// A special derived type of <see cref="BenchmarkClassAttribute"/> that annotates a benchmark class as a Project Euler
/// problem <see href="https://projecteuler.net/"/>. This attribute contains additional metadata, such as the problem's
/// unique integer identifier and optionally the expected answer. The annotated class is recommended to derive from
/// <see cref="Solver{T}"/> or <see cref="Solver"/>, as these base classes are designed for Project Euler "solution
/// methods" (i.e. <see cref="SolutionAttribute"/>-annotated benchmark methods) in mind.
/// </para>
/// <para>
/// The other Project Euler specific APIs are:
/// <list type="bullet">
/// <item><description>
/// <see cref="ParameterAttribute"/>: a field or property that is an input parameter to a solution method. Changing the
/// value of a parameter may change the result of a solution method.
/// </description></item>
/// <item><description>
/// <see cref="SolutionAttribute"/>: a "solution method", which is derived type of benchmark method that is not a pure
/// function, computing some <see cref="Solver.Answer"/>, incrementing <see cref="Solver.Iterations"/>, or logging
/// execution information.
/// </description></item>
/// </list>
/// </para>
/// </summary>
/// <remarks>
/// There are default implementations of the API hooks that provide additional functionality Project Euler problems,
/// e.g. using a <see cref="ResourceAttribute"/> to fetch files from the website (backed by a
/// <see cref="IResourceProvider"/>).
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public class ProjectEulerAttribute : BenchmarkClassAttribute
{
    /// <summary>
    /// The problem's unique integer identifier, corresponding to <c>https://projecteuler.net/problem={Id}</c>.
    /// </summary>
    public uint Id { get; protected init; }

    /// <summary>
    /// The correct answer for the problem that is expected to be emitted by a solution (i.e. Euler benchmark method).
    /// If <c>null</c>, the answer is not known by the implementer yet. Note that the answer may be of any primitive
    /// type (e.g. answers may be strings, integers, or floating point numbers).
    /// </summary>
    /// <remarks>
    /// This is equivalent to an entry in <see cref="Solver{T}.TestCases"/> with the default values for any
    /// <see cref="ParameterAttribute"/> field/properties and <see cref="TestCase{T}.Answer"/> set to
    /// <see cref="ExpectedAnswer"/>. The default implementation will assert the returned value of a
    /// <see cref="BenchmarkMethodAttribute"/>-annotated (or derived Project Euler specific
    /// <see cref="SolutionAttribute"/>) method is <see cref="ExpectedAnswer"/>.
    /// </remarks>
    public object? ExpectedAnswer { get; init; }

    /// <inheritdoc cref="ProjectEulerAttribute"/>
    /// <param name="id">Sets <see cref="Id"/>.</param>
    /// <param name="displayName">Sets <see cref="BenchmarkClassAttribute.DisplayName"/>.</param>
    /// <param name="file"><inheritdoc cref="BenchmarkClassAttribute(string,int)" path="/param[@name='file']"/></param>
    /// <param name="line"><inheritdoc cref="BenchmarkClassAttribute(string,int)" path="/param[@name='line']"/></param>
    public ProjectEulerAttribute(
        uint id,
        string? displayName = null,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0
    )
    {
        Id = id;
        DisplayName = displayName;

        File = file;
        Line = line;
    }
}
