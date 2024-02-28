﻿using Dev.Concision.ProjectEuler.Cli;
using Microsoft.Extensions.DependencyInjection;
using Net.ProjectEuler.Framework;
using Net.ProjectEuler.Framework.Cli;

namespace Dev.Concision.ProjectEuler;

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
