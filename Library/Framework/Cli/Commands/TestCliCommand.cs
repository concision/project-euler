using CommandLine;
using Microsoft.Extensions.Logging;

namespace Net.ProjectEuler.Framework.Cli.Commands;

[Verb("test", HelpText = "Executes logging test")]
public sealed class TestArgs : CliArgs
{
}

public class TestCliCommand(ILogger logger) : CliCommand<TestArgs>
{
    public override Task<int> ExecuteAsync(TestArgs args)
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