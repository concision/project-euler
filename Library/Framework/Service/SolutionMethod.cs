using System.Reflection;
using CommandLine;
using Net.ProjectEuler.Framework.Api;

namespace Net.ProjectEuler.Framework.Service;

public class SolutionMethod(MethodInfo method)
{
    public Type SolverType => Method.DeclaringType!;

    public BenchmarkAttribute? BenchmarkAttribute => SolverType
        .GetCustomAttributes(inherit: true)
        .FirstOrDefault(attribute => attribute is BenchmarkAttribute)
        .Cast<BenchmarkAttribute?>();

    public SolutionAttribute? Attribute => SolverType
        .GetCustomAttributes(inherit: true)
        .FirstOrDefault(attribute => attribute is SolutionAttribute)
        .Cast<SolutionAttribute?>();

    public MethodInfo Method { get; } = method;

    public IReadOnlyList<Parameter> Parameters { get; } = [];

    public IReadOnlyList<PrepareMethod> Prepares { get; } = [];

    public object? Instance { get; set; }
}

public sealed class Parameter
{
    public ParameterAttribute? Attribute { get; set; }

    public IncludeAttribute? Include { get; set; }

    public MemberInfo Member { get; }
}

public sealed class PrepareMethod
{
    public PrepareAttribute? Attribute { get; set; }

    public IDictionary<int, IncludeAttribute> Includes { get; } = new Dictionary<int, IncludeAttribute>();
    
    public MethodInfo Method { get; init; }
}