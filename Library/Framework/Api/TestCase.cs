using System.ComponentModel.DataAnnotations;

namespace Net.ProjectEuler.Framework.Api;

/// <summary>
/// A test case that contains a collection of parameter values and the expected answer for a Project Euler problem.
/// These test cases are tested against the problem class' solution methods' <see cref="Solver{T}.Answer"/>s. Custom
/// test cases for a problem can be implemented by deriving <see cref="Solver{T}"/> and overriding
/// <see cref="Solver{T}.TestCases"/>.
/// </summary>
/// <typeparam name="T">The <see cref="Solver{T}"/>'s generic <see cref="Answer"/> type.</typeparam>
public struct TestCase<T>
{
    /// <summary>
    /// A dictionary of the parameter names and their values to inject for the <see cref="Solver{T}"/>'s
    /// <see cref="ParameterAttribute"/>-annotated fields/properties. If a parameter is not specified, the default value
    /// that the <see cref="Solver{T}"/> declares is used.
    /// </summary>
    [Required]
    public IDictionary<string, object> Parameters { get; init; }

    /// <summary>
    /// The expected answer for the solution method. The solution method's <see cref="Solver{T}.Answer"/> is expected to
    /// match this value for the configured <see cref="Parameters"/>.
    /// </summary>
    public required T Answer { get; init; }
}
