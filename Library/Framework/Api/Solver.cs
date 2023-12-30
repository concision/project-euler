namespace Net.ProjectEuler.Framework.Api;

public abstract class Solver<T>
    where T : notnull
{
    // protected Logger Logger { get; set; }

    protected ulong Iterations { get; set; } = 0;

    protected T Answer { get; set; }
}

public abstract class Solver : Solver<object>
{
}
