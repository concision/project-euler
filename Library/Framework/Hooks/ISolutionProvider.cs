using System.Reflection;
using Net.ProjectEuler.Framework.Api;

namespace Net.ProjectEuler.Framework.Hooks;

public interface ISolutionProvider
{
    Task<IEnumerable<MethodInfo>> DiscoverAllSolutions();
}

public class SolutionProvider : ISolutionProvider
{
    public Task<IEnumerable<MethodInfo>> DiscoverAllSolutions()
    {
        return Task.FromResult<IEnumerable<MethodInfo>>(AppDomain.CurrentDomain.GetAssemblies()
            .AsParallel()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.GetCustomAttributes().Any(attribute => attribute is BenchmarkAttribute))
            .SelectMany(type => type
                .GetMethods()
                .Where(method => method.GetCustomAttributes(typeof(SolutionAttribute), true).Length > 0)
            ));
    }
}