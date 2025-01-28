using System.Collections;

namespace Dev.Concision.ProjectEuler.Extensions;

/// <summary>
/// Extension methods for <see cref="string"/>.
/// </summary>
internal static class StringExtensions
{
    /// <param name="values"><see cref="IEnumerable"/> to concatenate.</param>
    /// <param name="separator">String separator to join between <paramref name="values"/>.</param>
    /// <inheritdoc cref="string.Join(string,object?[])"/>
    public static string Join<T>(this IEnumerable<T> values, string separator)
    {
        return string.Join(separator, values.ToArray());
    }
}
