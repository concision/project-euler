using Microsoft.Extensions.Logging;

namespace Net.ProjectEuler.Framework.Api;

/// <summary>
/// An optional base class for Project Euler problems that provides additional helpful functionalities that affect
/// logging output. A derived class is expected to be annotated with <see cref="ProjectEulerAttribute"/> and contain one
/// or more <see cref="SolutionAttribute"/>-annotated solution methods. Solution methods are expected to modify
/// <see cref="Solver{T}.Answer"/> or <see cref="Solver{T}.Iterations"/>.
/// </summary>
/// <typeparam name="T"> Sets the <see cref="Answer"/> type.</typeparam>
public abstract class Solver<T>
    where T : notnull
{
    /// <summary>
    /// A <see cref="ILogger"/> to log any messages during the execution of the solution method.
    /// </summary>
    /// <remarks>
    /// This <see cref="ILogger"/> may NO-OP when the solution method is executed in a non-logging environment, such as
    /// a benchmarking environment (i.e. executing the solution repeatedly to analyze runtime performance).
    /// </remarks>
    [DependencyInject]
    public required ILogger Logger { get; init; }


    /// <summary>
    /// <para>
    /// If the derived class is annotated with <see cref="ProjectEulerAttribute"/> with
    /// <see cref="ProjectEulerAttribute.ExpectedAnswer"/> set, then there exists a default test case exists where
    /// <see cref="TestCase{T}.Parameters"/> is set to all <see cref="ParameterAttribute"/>-annotated fields/properties
    /// with their default values and <see cref="TestCase{T}.Answer"/> set to
    /// <see cref="ProjectEulerAttribute.ExpectedAnswer"/>. This is the typical case for a Project Euler problem that
    /// has a known answer.
    /// </para>
    /// <para>
    /// Some Problem Euler problems provide a list of computationally-smaller test cases to validate the result of a
    /// solution method's implementation. This property may provide additional test cases to validate the correctness of
    /// a solution method's implementation. If empty, the solution method is only executed with the optional default
    /// test case.
    /// </para>
    /// </summary>
    public virtual TestCase<T>[] TestCases { get; } = [];


    /// <summary>
    /// The number of iterations executed when invoking the solution method. This number should be incremented by the
    /// invoked solution method for every inner-most operation (e.g. the inner-most nested for loop, a recursive method
    /// base case).
    /// </summary>
    /// <remarks>
    /// Note that this number counter is not necessarily comparable across different solution implementations, as the
    /// number of iterations may be different for different algorithm implementations. It is only useful for analyzing
    /// the time complexity of the solution as parameters are scaled up.
    /// </remarks>
    public ulong Iterations { get; protected set; }

    /// <summary>
    /// The solution method's answer for the given parameters (<see cref="ParameterAttribute"/>-annotated
    /// fields/properties).
    /// </summary>
    /// <remarks>
    /// This generic-typed property may be useful in reducing duplicate variable declarations, e.g. when the answer is
    /// an accumulation/sum of an iterated search space, reducing redundant <c>long sum = ...;</c> <c>Answer = sum;</c>
    /// statements. This permits derived classes to syntactically-easily 'build' an answer during execution.
    /// </remarks>
    public T Answer { get; protected set; } = default!;

    protected void Log(object? obj = null)
    {
        Logger.LogInformation(obj == null ? "" : obj.ToString());
    }
}

/// <summary>
/// A shorthand of <see cref="Solver{T}"/> where the generic argument type is <see cref="object"/>.
/// </summary>
/// <remarks>
/// This is useful during development when the problem's solution method's do not have a known answer type.
/// </remarks>
public abstract class Solver : Solver<object>
{
}
