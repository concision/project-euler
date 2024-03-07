using System.Drawing;
using System.Reflection;
using CommandLine;
using Microsoft.Extensions.Logging;
using Net.ProjectEuler.Framework.Api;
using Net.ProjectEuler.Framework.Api.Hooks;
using Net.ProjectEuler.Framework.Model;
using Net.ProjectEuler.Framework.Service;
using Pastel;
using static Crayon.Output;
using static Net.ProjectEuler.Framework.Logging.EulerConsoleFormatter;

namespace Net.ProjectEuler.Framework.Cli.Commands;

/// <summary>
/// CLI arguments for <see cref="TestBenchmarkCliCommand"/>.
/// </summary>
[Verb("test", HelpText = "Executes a solution")]
public sealed class TestBenchmarkSolutionArgs : CliArgs
{
    // TODO: implement flags for:
    // - [Parameter] injection
    // - benchmark execution (with an iteration limit and/or time limit)
}

// TODO: split up a lot of these SolutionMethod implementation stuff to services so other CLI commands can utilize them
public class TestBenchmarkCliCommand(
    IServiceProvider serviceProvider,
    ILogger logger,
    IStopwatchService stopwatchService,
    IBenchmarkService benchmarkService,
    IBenchmarkGrouper benchmarkGrouper
) : CliCommand<TestBenchmarkSolutionArgs>
{
    public override async Task<int> ExecuteAsync(TestBenchmarkSolutionArgs args)
    {
        var discoveredSolutions = await benchmarkService.ProvideBenchmarksAsync();
        var filteredSolutions = await benchmarkService.FilterBenchmarksAsync(discoveredSolutions);

        if (!filteredSolutions.Any())
        {
            logger.LogError("No benchmark methods were found");
            return 1;
        }
        
        var groupedSolutions = benchmarkGrouper.GroupExecutions(filteredSolutions);
        

        foreach (var group in groupedSolutions)
        {
            DrawLine(group.Key);
            var solutions = group.ToArray();

            for (var i = 0; i < solutions.Length; i++)
            {
                var solution = solutions[i];
                logger.LogInformation("Invoking benchmark method '" + Bold().Yellow($"{solution.Method.Name}") + "'");
                if (solution.Attribute?.DisabledByDefault ?? false)
                    continue;
                try
                {
                    await ExecuteSolution(solution);
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "An exception occurred while executing the solution method");
                }
                if (i != solutions.Length - 1)
                    logger.LogInformation("");
            }
        }
        if (filteredSolutions.Any())
            DrawLine();

        return 0;
    }

    private void DrawLine(string text = "")
    {
        var width = (128.0 - text.Length) / 2.0 - (!string.IsNullOrWhiteSpace(text) ? 1 : 0);
        logger.LogInformation(
            NoLoggingPrefix + Blue(new string('=', (int) Math.Floor(width)))
                            + (!string.IsNullOrWhiteSpace(text) ? $" {Bold().Yellow(text)} " : "")
                            + Blue(new string('=', (int) Math.Ceiling(width)))
        );
    }

    private async Task ExecuteSolution(Benchmark solution)
    {
        await benchmarkService.HydrateBenchmarkAsync(solution);

        foreach (var parameter in solution.Parameters)
        {
            // parameter.Include?.Url
            logger.LogInformation("Property '".Pastel(Color.White) + parameter.Member.Name.Pastel(Color.Cyan) + "': ".Pastel(Color.White) +
                                  (parameter.Member switch
                                  {
                                      PropertyInfo property => property.GetValue(solution.Instance),
                                      FieldInfo field => field.GetValue(solution.Instance),
                                      _ => throw new ArgumentOutOfRangeException(),
                                  })?.ToString().Pastel(Color.LawnGreen));
        }

        // TODO: set [Include]s
        // TODO: override [Parameter]s and print
        // TODO: invoke [Prepare]

        // invoke benchmark method
        object? answer;
        ulong iterations = 0;
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
                iterations = ((dynamic) solution.Instance).Iterations;
                break;
            }
        // TODO: serialize objects to string with formatting
        logger.LogInformation(Yellow("Answer") + White(": ") +
                              (answer is not null ? Green(answer.ToString()!) : Red("<none>")));
        if (0 < iterations)
            logger.LogInformation(Yellow("Iterations") + White(": ") + Green(iterations.ToString()));
        logger.LogInformation(Yellow("Elapsed Time") + White(": ") + Green(stopwatchService.LastElapsed.ToString()));
    }
}
