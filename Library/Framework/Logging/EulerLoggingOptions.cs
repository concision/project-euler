using Microsoft.Extensions.Logging.Console;

namespace Net.ProjectEuler.Framework.Logging;

public sealed class EulerLoggingOptions : ConsoleFormatterOptions
{
    public bool ExceptionIndentation { get; set; } = true;
}