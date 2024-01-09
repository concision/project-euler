using System.Runtime.CompilerServices;

namespace Net.ProjectEuler.Framework.Api;

[AttributeUsage(AttributeTargets.Class)]
public class BenchmarkAttribute : Attribute
{
    public string File { get; protected set; }
    public int Line { get; protected set; }

    public BenchmarkAttribute([CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        File = file;
        Line = line;
    }
}