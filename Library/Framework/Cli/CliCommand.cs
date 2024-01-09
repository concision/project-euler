namespace Net.ProjectEuler.Framework.Cli;

public abstract class CliCommand
{
    public Type ArgumentType
    {
        get
        {
            for (Type? type = GetType(); type?.BaseType != null; type = type.BaseType)
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(CliCommand<>))
                    return type.GetGenericArguments()[0];
            throw new InvalidOperationException($"{nameof(CliCommand)} implementations must implement {typeof(CliCommand<>).FullName}");
        }
    }

    public async Task<int> ExecuteAsync(object args)
    {
        var argumentType = ArgumentType;
        if (!argumentType.IsInstanceOfType(args))
            throw new ArgumentException($"'{GetType().FullName}.{nameof(ExecuteAsync)}(...)' argument must be of type '{argumentType.FullName}'", nameof(args));

        return await (Task<int>) typeof(CliCommand<>).MakeGenericType(argumentType)
            .GetMethod(nameof(ExecuteAsync), [argumentType])!
            .Invoke(this, [args])!;
    }
}

public abstract class CliCommand<TArgumentType> : CliCommand
    where TArgumentType : CliArgs
{
    public abstract Task<int> ExecuteAsync(TArgumentType args);
}