namespace Net.ProjectEuler.Framework.Cli;

/// <summary>
/// A non-generic version of <see cref="CliCommand{TArgumentType}"/> that permits invoking
/// <see cref="ExecuteAsync(object)"/> with a <see cref="object"/> argument. This is used for invoking a
/// <see cref="ExecuteAsync"/> on <see cref="CliCommand{TArgumentType}"/>-derived classes without knowing the specific
/// argument type.
/// </summary>
public abstract class CliCommand
{
    /// <summary>
    /// The generic argument <see cref="Type"/> of the generic <see cref="CliCommand{TArgumentType}"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if <see langword="this"/> does not implement <see cref="CliCommand{TArgumentType}"/>.
    /// </exception>
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

    /// <summary>
    /// A non-generic equivalent of <see cref="CliCommand{TArgumentType}.ExecuteAsync(TArgumentType)"/>.
    /// </summary>
    /// <param name="args">A <see cref="CliArgs"/> of type <see cref="ArgumentType"/>.</param>
    /// <returns>A task representing the CLI exit code of the command.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="args"/> is not of type <see cref="ArgumentType"/>.</exception>
    public async Task<int> ExecuteAsync(CliArgs args)
    {
        var argumentType = ArgumentType;
        if (!argumentType.IsInstanceOfType(args))
            throw new ArgumentException(
                $"'{GetType().FullName}.{nameof(ExecuteAsync)}(...)' argument must be of type '{argumentType.FullName}'",
                nameof(args)
            );

        return await (Task<int>) typeof(CliCommand<>).MakeGenericType(argumentType)
            .GetMethod(nameof(ExecuteAsync), [argumentType])!
            .Invoke(this, [args])!;
    }
}

/// <summary>
/// Represents an executable CLI command that can be invoked from the command line.
/// </summary>
/// <typeparam name="TArgumentType">The CLI argument type, which must extent from the global type <see cref="CliArgs"/>.</typeparam>
public abstract class CliCommand<TArgumentType> : CliCommand
    where TArgumentType : CliArgs
{
    /// <summary>
    /// Executes the command with the given CLI arguments.
    /// </summary>
    /// <param name="args">
    /// A <see cref="CliArgs"/> of type <typeparamref name="TArgumentType"/> that is populated with command arguments.
    /// </param>
    /// <returns>A task representing the CLI exit code of the command.</returns>
    public abstract Task<int> ExecuteAsync(TArgumentType args);
}
