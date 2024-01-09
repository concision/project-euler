using Net.ProjectEuler.Framework.Api;
using Net.ProjectEuler.Framework.Cli.Commands;

namespace Net.ProjectEuler.Framework.Hooks;

/// <summary>
/// TODO: implementations for Project Euler attachments, matrices, and local files
/// </summary>
public interface IIncludeProvider
{
    Task<string> FetchAsync(SolutionMethod solution, IncludeAttribute request);
}