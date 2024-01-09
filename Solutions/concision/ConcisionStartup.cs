using Me.Concision.ProjectEuler.Cli;
using Microsoft.Extensions.DependencyInjection;
using Net.ProjectEuler.Framework;
using Net.ProjectEuler.Framework.Cli;
using Net.ProjectEuler.Framework.Cli.Commands;

namespace Me.Concision.ProjectEuler;

public class ConcisionStartup : ProjectEulerStartup
{
    protected override void BootstrapServiceCollection(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<CliCommand, DemoCliCommand>();
    }

    public static async Task<int> Main(params string[] args)
    {
        return await Main<ConcisionStartup>(args);
    }
}