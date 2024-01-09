using System.Text.RegularExpressions;
using Crayon;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Pastel;

namespace Net.ProjectEuler.Framework.Logging;

public sealed partial class EulerConsoleFormatter : ConsoleFormatter, IDisposable
{
    private readonly IDisposable? optionsReloadToken;
    private CustomOptions options;

    public EulerConsoleFormatter(IOptionsMonitor<CustomOptions> options) : base(nameof(EulerConsoleFormatter))
    {
        optionsReloadToken = options.OnChange(newOptions => this.options = newOptions);
        this.options = options.CurrentValue;
    }

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        DateTime time = DateTime.Now;
        string? message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);
        if (message is null)
            return;

        textWriter.Write(Output.Bold().Black().Text("["));
        textWriter.Write(Output.Blue().Text(time.ToString("dd HH:mm:ss.ffffff")));
        textWriter.Write(Output.Bold().Black().Text("] "));

        var (categoryColor, textColor, categoryLabel) = logEntry.LogLevel switch
        {
            LogLevel.Trace => (ConsoleColor.Cyan, ConsoleColor.Gray, "TRACE"),
            LogLevel.Debug => (ConsoleColor.DarkCyan, ConsoleColor.Gray, "DEBUG"),
            LogLevel.Information => (ConsoleColor.Green, ConsoleColor.White, "INFO"),
            LogLevel.Warning => (ConsoleColor.Yellow, ConsoleColor.Yellow, "WARN"),
            LogLevel.Error => (ConsoleColor.Red, ConsoleColor.Red, "ERROR"),
            LogLevel.Critical => (ConsoleColor.Red, ConsoleColor.Red, "CRIT"),
            LogLevel.None => (ConsoleColor.White, ConsoleColor.White, "???"),
        };
        textWriter.Write(logEntry.LogLevel is LogLevel.Error or LogLevel.Critical
            ? Output.Bold().Text($"{categoryLabel,-5} ".Pastel(categoryColor))
            : $"{categoryLabel,-5} ".Pastel(categoryColor));
        textWriter.WriteLine(message.Pastel(textColor));
        if (logEntry.Exception is not null)
        {
            var exceptionMessage = logEntry.Exception.ToString();
            foreach (var line in LineRegex().Split(exceptionMessage))
                textWriter.WriteLine(options.ExceptionIndentation ? new string(' ', 27) + line.Pastel(textColor) : line.Pastel(textColor));
        }
    }

    public void Dispose() => optionsReloadToken?.Dispose();

    [GeneratedRegex("\r\n|\r|\n")]
    private static partial Regex LineRegex();
}