using System.Reflection;
using CommandLine;
using Net.ProjectEuler.Framework.Api;

namespace Net.ProjectEuler.Framework.Cli.Commands;

public class SolutionMethod
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

    public MethodInfo Method { get; private set; }


    public Initializer[] Initializers { get; }

    public PrepareMethod[] PrepareMethods { get; }


    public SolutionMethod(MethodInfo method)
    {
        Method = method;
    }
}

public sealed class Initializer
{
    public ParameterAttribute Parameter { get; }
    public MemberInfo Member { get; }

    public Initializer(MemberInfo memberInfo)
    {
    }
}

public sealed class PrepareMethod
{
}