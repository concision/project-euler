using System.Drawing;
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
    public new static readonly string Name = nameof(EulerConsoleFormatter);
    public static readonly string NoLoggingPrefix = new((char) 0x7, 1);

    private readonly IDisposable? optionsReloadToken;
    private EulerLoggingOptions options;

    public EulerConsoleFormatter(IOptionsMonitor<EulerLoggingOptions> options) : base(Name)
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

        if (!message.StartsWith(NoLoggingPrefix, StringComparison.Ordinal))
        {
            textWriter.Write(Output.Bold().Black("["));
            textWriter.Write(time.ToString("dd HH:mm:ss.ffffff").Pastel(Color.DodgerBlue));
            textWriter.Write(Output.Bold().Black("] "));
        }

        var (logLevelPrefix, messageColor) = logEntry.LogLevel switch
        {
            LogLevel.Trace => ("TRACE".Pastel(Color.DarkCyan), Color.Gray),
            LogLevel.Debug => ("DEBUG".Pastel(Color.DarkCyan), Color.Gray),
            LogLevel.Information => ("INFO ".Pastel(Color.GreenYellow), Color.White),
            LogLevel.Warning => ("WARN ".Pastel(Color.Yellow), Color.Yellow),
            LogLevel.Error => ("ERROR".Pastel(Color.Red), Color.Red),
            LogLevel.Critical => (Output.Bold("ERROR".Pastel(Color.Red)), Color.Red),
            _ => (Output.Bold("?????".Pastel(Color.White)), Color.White),
        };
        if (!message.StartsWith(NoLoggingPrefix, StringComparison.Ordinal))
            textWriter.Write($"{logLevelPrefix} ");
        textWriter.WriteLine(message.Pastel(messageColor));
        if (logEntry.Exception is not null)
            foreach (var line in LineRegex().Split(logEntry.Exception.ToString()))
                textWriter.WriteLine(new string(' ', options.ExceptionIndentation ? 27 : 0) + line.Pastel(messageColor));
    }

    public void Dispose() => optionsReloadToken?.Dispose();

    [GeneratedRegex("\r\n|\r|\n")]
    private static partial Regex LineRegex();
}