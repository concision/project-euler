using Microsoft.Extensions.Logging;

namespace Net.ProjectEuler.Framework.Api;

public abstract class Solver<T>
    where T : notnull
{
    [InjectService]
    protected ILogger Logger { get; set; }

    public ulong Iterations { get; set; } = 0;

    public T Answer { get; set; }
}

public abstract class Solver : Solver<object>
{
}