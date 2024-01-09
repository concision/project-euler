namespace Net.ProjectEuler.Framework;

public interface IProjectEulerCli
{
    Task<int> ExecuteCliCommand(params string[] cliArguments);
}