using Microsoft.Extensions.Logging.Console;

namespace Net.ProjectEuler.Framework.Logging;

/// <summary>
/// 
/// </summary>
public sealed class EulerLoggingOptions : ConsoleFormatterOptions
{
    /// <summary>
    /// 
    /// </summary>
    public bool ExceptionIndentation { get; set; } = true;
}