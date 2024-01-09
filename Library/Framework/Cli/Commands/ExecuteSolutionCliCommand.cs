using System.Drawing;
using System.Reflection;
using CommandLine;
using Microsoft.Extensions.Logging;
using Net.ProjectEuler.Framework.Api;
using Net.ProjectEuler.Framework.Hooks;
using Net.ProjectEuler.Framework.Service;
using Pastel;
using static Crayon.Output;
using static Net.ProjectEuler.Framework.Logging.EulerConsoleFormatter;

namespace Net.ProjectEuler.Framework.Cli.Commands;

[Verb("execute", HelpText = "Executes a specific solution")]
public sealed class ExecuteSolutionArgs : CliArgs
{
}

/// <summary>
/// TODO: split up a lot of these SolutionMethod implementation stuff to services so other CLI commands can utilize them
/// TODO: document this
/// </summary>
/// <param name="serviceProvider"></param>
/// <param name="logger"></param>
/// <param name="solutionProviders"></param>
/// <param name="benchmarkSelectors"></param>
public class ExecuteSolutionCliCommand(
    IServiceProvider serviceProvider,
    ILogger logger,
    IStopwatchService stopwatchService,
    ISolutionService solutionService,
    ISolutionGrouper solutionGrouper
) : CliCommand<ExecuteSolutionArgs>
{
    public override async Task<int> ExecuteAsync(ExecuteSolutionArgs args)
    {
        var discoveredSolutions = await solutionService.DiscoverSolutions();
        var filteredSolutions = await solutionService.FilterSolution(discoveredSolutions);
        var groupedSolutions = solutionGrouper.Group(filteredSolutions);

        foreach ((string displayName, IReadOnlyList<SolutionMethod> solutions) in groupedSolutions)
        {
            drawLine(displayName);

            for (var i = 0; i < solutions.Count; i++)
            {
                var solution = solutions[i];
                logger.LogInformation("Invoking '" + Bold().Yellow($"{solution.SolverType.FullName}.{solution.Method.Name}") + "'");
                try
                {
                    await ExecuteSolution(solution);
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "An exception occurred while executing the solution method");
                }
                if (i != solutions.Count - 1)
                    logger.LogInformation("");
            }
        }
        if (filteredSolutions.Any())
            drawLine();

        return 0;
    }

    private void drawLine(string text = "")
    {
        var width = (128.0 - text.Length) / 2.0 - (!string.IsNullOrWhiteSpace(text) ? 1 : 0);
        // TODO: figure out how to remove timestamp prefix
        logger.LogInformation(
            NoPrefix + Blue(new string('=', (int) Math.Floor(width)))
                     + (!string.IsNullOrWhiteSpace(text) ? $" {Bold().Yellow(text)} " : "")
                     + Blue(new string('=', (int) Math.Ceiling(width)))
        );
    }

    private async Task ExecuteSolution(SolutionMethod solution)
    {
        await solutionService.HydrateSolutionAsync(solution);

        foreach (var parameter in solution.Parameters)
        {
            // parameter.Include?.Url
            logger.LogInformation("Property '".Pastel(Color.White) + parameter.Member.Name.Pastel(Color.Cyan) + "': ".Pastel(Color.White) + (parameter.Member switch
            {
                PropertyInfo property => property.GetValue(solution.Instance),
                FieldInfo field => field.GetValue(solution.Instance),
            }).ToString().Pastel(Color.LawnGreen));
        }

        // TODO: set [Include]s
        // TODO: override [Parameter]s and print
        // TODO: invoke [Prepare]

        // invoke benchmark method
        object? answer;
        using (stopwatchService.Start(solution.SolverType.FullName + "." + solution.Method.Name + "()"))
        {
            // TODO: faster reflection invocation to minimize overhead
            answer = solution.Method.Invoke(
                solution.Instance,
                solution.Method.GetParameters()
                    .Select(parameter => serviceProvider.GetService(parameter.ParameterType))
                    .ToArray()
            );
            if (answer is Task task)
                answer = task.GetType().GetProperty(nameof(Task<object>.Result), BindingFlags.Instance | BindingFlags.Public)!
                    .GetValue(task);
        }

        // generate summary
        // TODO: generate table
        for (var type = solution.Instance!.GetType(); type != null; type = type.BaseType)
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Solver<>))
            {
                answer ??= ((dynamic) solution.Instance).Answer;
                break;
            }
        // TODO: serialize objects to string with formatting
        logger.LogInformation(Yellow("Answer") + White(": ") +
                              (answer is not null ? Green(answer.ToString()!) : Red("<none>")));
        logger.LogInformation(Yellow("Elapsed Time") + White(": ") + Green(stopwatchService.LastElapsed.ToString()));
    }
}