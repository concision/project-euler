using System.Reflection;
using CommandLine;
using Net.ProjectEuler.Framework.Api;

namespace Net.ProjectEuler.Framework.Model;

public class Benchmark
{
    public Type SolverType => Method.DeclaringType!;

    public BenchmarkClassAttribute? BenchmarkAttribute => SolverType
        .GetCustomAttributes(inherit: true)
        .FirstOrDefault(attribute => attribute is BenchmarkClassAttribute)
        .Cast<BenchmarkClassAttribute?>();

    public SolutionAttribute? Attribute => Method
        .GetCustomAttributes(inherit: true)
        .FirstOrDefault(attribute => attribute is SolutionAttribute)
        .Cast<SolutionAttribute?>();

    public MethodInfo Method { get; }

    public IReadOnlyList<Parameter> Parameters { get; } = [];

    public IReadOnlyList<Initializer> Prepares { get; } = [];

    public object? Instance { get; set; }

    public Benchmark(MethodInfo method)
    {
        Method = method;
    }
}

public sealed class Parameter
{
    public ParameterAttribute? Attribute { get; set; }

    public ResourceAttribute? Include { get; set; }

    public required MemberInfo Member { get; init; }
}

public sealed class Initializer
{
    public InitializerAttribute? Attribute { get; set; }

    public IDictionary<int, ResourceAttribute> Includes { get; } = new Dictionary<int, ResourceAttribute>();

    public required MethodInfo Method { get; init; }
}
