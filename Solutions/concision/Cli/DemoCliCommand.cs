using CommandLine;
using Microsoft.Extensions.Logging;
using Net.ProjectEuler.Framework.Cli;

namespace Dev.Concision.ProjectEuler.Cli;

[Verb("demo", HelpText = "Runs a demonstration of the CLI API")]
public sealed class DemoArgs : CliArgs
{
}

public class DemoCliCommand(ILogger logger) : CliCommand<DemoArgs>
{
    public override Task<int> ExecuteAsync(DemoArgs args)
    {
        logger.LogInformation("Information");
        logger.LogCritical("Critical");
        logger.LogDebug("Debug");
        logger.LogTrace("Trace");
        logger.LogWarning("Warning");
        try
        {
            throw new Exception("Inner exception");
        }
        catch (Exception exception)
        {
            logger.LogError(new InvalidOperationException("Invalid operation message", exception), "An exception occurred");
        }

        return Task.FromResult(0);
    }
}
