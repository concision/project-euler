using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.Logging;
using Net.ProjectEuler.Framework.Cli;

namespace Net.ProjectEuler.Framework;

public interface IProjectEulerCli
{
    Task<int> ExecuteCliCommand(params string[] cliArguments);
}
public class ProjectEulerCli : IProjectEulerCli
{
    private readonly ILogger logger;

    private readonly Parser parser;
    private readonly IReadOnlyList<CliCommand> cliCommands;

    public ProjectEulerCli(
        ILogger logger,
        Parser parser,
        IEnumerable<CliCommand> cliCommands
    )
    {
        this.logger = logger;
        this.parser = parser;
        this.cliCommands = cliCommands.ToArray();
    }

    public async Task<int> ExecuteCliCommand(params string[] cliArguments)
    {
        var cliParseResult = parser.ParseArguments(cliArguments, cliCommands.Select(subcommand => subcommand.ArgumentType).ToArray());

        if (cliParseResult.Errors.Any())
        {
            await Console.Error.WriteAsync(HelpText.AutoBuild(cliParseResult, helpText => helpText, example => example));
            return -1;
        }

        var command = cliCommands.FirstOrDefault(cliCommand => cliCommand.ArgumentType.IsInstanceOfType(cliParseResult.Value));
        if (command is null)
        {
            logger.LogTrace($"No such CLI command found to handle parsed argument structure '{cliParseResult.Value?.GetType().FullName ?? "null"}'");
            return -1;
        }

        return await command.ExecuteAsync(cliParseResult.Value);
    }
}