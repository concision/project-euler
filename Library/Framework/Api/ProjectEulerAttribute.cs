using System.Runtime.CompilerServices;

namespace Net.ProjectEuler.Framework.Api;

[AttributeUsage(AttributeTargets.Class)]
public class ProjectEulerAttribute : BenchmarkAttribute
{
    public uint Id { get; }

    public ProjectEulerAttribute(uint id, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        Id = id;
        File = file;
        Line = line;
    }
}