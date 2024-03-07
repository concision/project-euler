using System.Reflection;
using Net.ProjectEuler.Framework.Api;
using Net.ProjectEuler.Framework.Api.Hooks;

namespace Net.ProjectEuler.Framework.Hooks;

/// <summary>
/// Provides a list of methods declared in a <see cref="BenchmarkClassAttribute"/>-annotated class that are marked with
/// the <see cref="BenchmarkMethodAttribute"/> attribute.
/// </summary>
public class BenchmarkProvider : IBenchmarkProvider
{
    public Task<IEnumerable<MethodInfo>> ProvideBenchmarksAsync()
    {
        return Task.FromResult<IEnumerable<MethodInfo>>(AppDomain.CurrentDomain.GetAssemblies()
            .AsParallel()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.GetCustomAttributes().Any(attribute => attribute is BenchmarkClassAttribute))
            .SelectMany(type => type
                .GetMethods()
                .Where(method => method.CustomAttributes
                    .Any(attribute => attribute.AttributeType.IsAssignableTo(typeof(BenchmarkMethodAttribute)))
                )
            ));
    }
}
