using System.Runtime.CompilerServices;

namespace Net.ProjectEuler.Framework.Api;

/// <summary>
/// <para>
/// A special derived type of <see cref="BenchmarkMethodAttribute"/> that annotates a benchmark method as a solution to
/// a ProjectEuler problem, referred to as a "solution method". This attribute contains additional metadata, such as the
/// initial implementation date of the solution, and any tags that indicate characteristics about the solution's method
/// implementation. The declaring class of the method must be annotated with <see cref="ProjectEulerAttribute"/>.
/// </para>
/// <para>
/// The declaring class is expected (however is not required) to derive from <see cref="Solver{T}"/> or
/// <see cref="Solver"/>. The method implementation is expected to modify <see cref="Solver{T}.Answer"/> or
/// <see cref="Solver{T}.Iterations"/>.
/// </para>
/// </summary>
/// <remarks>
/// There are default implementations of the API hooks that provide additional functionality Project Euler problems.
/// </remarks>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class SolutionAttribute : BenchmarkMethodAttribute
{
    /// <summary>
    /// Indicates the implementation date in ISO format, i.e. YYYY-MM-DD.
    /// </summary>
    /// <remarks>
    /// While <c>git blame</c> technically contains this data, this data is useful to have for ease of data manipulation.
    /// </remarks>
    public string? Date { get; set; }

    /// <summary>
    /// A collection of <see cref="SolutionTag"/>'s that indicate characteristics about the method's implementation.
    /// </summary>
    /// <remarks>
    /// Some tags may control how a solution method is discovered or executed.
    /// </remarks>
    public SolutionTag[] Tags { get; set; } = [];

    /// <inheritdoc cref="SolutionAttribute"/>
    /// <param name="displayName">A human-readable display name.</param>
    /// <param name="invokingMemberName">
    /// If <paramref name="displayName"/> is <c>null</c> (i.e. unspecified), the invoking method that has the
    /// <see cref="SolutionAttribute"/> method name will be used as the name.
    /// </param>
    public SolutionAttribute(string? displayName = null, [CallerMemberName] string? invokingMemberName = null)
    {
        DisplayName = displayName ?? invokingMemberName ?? null;
    }
}

/// <summary>
/// Indicates characteristics of a <see cref="SolutionAttribute"/>-annotated method. See
/// <see cref="SolutionAttribute.Tags"/>.
/// </summary>
public enum SolutionTag
{
    /// <summary>
    /// Indicates that the <see cref="SolutionAttribute"/>-annotated method is the first successful implementation for
    /// solving the problem.
    /// </summary>
    FirstSuccessfulAttempt,

    /// <summary>
    /// Indicates that the <see cref="SolutionAttribute"/>-annotated method is a brute force implementation.
    /// </summary>
    BruteForce,

    /// <summary>
    /// Indicates that the <see cref="SolutionAttribute"/>-annotated method is primarily used for data analysis during
    /// the problem solving process, and is not actually useful for solving the problem with the problem's large
    /// parameters.
    /// </summary>
    /// <remarks>
    /// By default, solutions with this tag will not be executed unless an override flag is specified. This is because
    /// the implementation may be expensive or an unbounded data search.
    /// </remarks>
    DataAnalysis,

    /// <summary>
    /// Indicates that the <see cref="SolutionAttribute"/>-annotated method implementation uses
    /// multi-threading/parallelization.
    /// </summary>
    /// <remarks>
    /// This flag may be used for scheduling multiple solutions to be executed concurrently without causing too much CPU
    /// thrash between problems.
    /// </remarks>
    Parallelized,
}
