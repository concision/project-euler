using Microsoft.Extensions.DependencyInjection;
using Net.ProjectEuler.Framework;

namespace Me.Concision.ProjectEuler.Solutions;

public class ConcisionStartup : ProjectEulerStartup
{
    protected override void BootstrapServiceCollection(IServiceCollection serviceCollection)
    {
        // serviceCollection.AddTransient<ISolutionProvider>(_ => null!);
    }

    public static async Task<int> Main(params string[] args)
    {
        return await Main<ConcisionStartup>(args);
    }
}