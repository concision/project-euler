using Microsoft.Extensions.DependencyInjection;
using Net.ProjectEuler.Framework;

namespace Me.TheNintendoDude.ProjectEuler;

public sealed class ConcisionStartup : ProjectEulerStartup
{
    protected override void BootstrapServiceCollection(IServiceCollection serviceCollection)
    {
    }
    
    public new static async Task<int> Main(params string[] args)
    {
        return await Main<ConcisionStartup>(args);
    }
}
