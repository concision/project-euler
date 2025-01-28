using Dev.Concision.ProjectEuler.Solutions.Cli;
using Microsoft.Extensions.DependencyInjection;
using Net.ProjectEuler.Framework;
using Net.ProjectEuler.Framework.Cli;

namespace Dev.Concision.ProjectEuler.Solutions;

public sealed class ConcisionStartup : ProjectEulerStartup
{
    protected override void BootstrapServiceCollection(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<CliCommand, DemoCliCommand>();
    }

    public new static async Task<int> Main(params string[] args)
    {
        return await Main<ConcisionStartup>(args);
    }
}
