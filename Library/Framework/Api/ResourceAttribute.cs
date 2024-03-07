using Net.ProjectEuler.Framework.Api.Hooks;
using Net.ProjectEuler.Framework.Hooks;

namespace Net.ProjectEuler.Framework.Api;

/// <summary>
/// Annotates that a field, property, <see cref="InitializerAttribute"/>-annotated method parameter,
/// <see cref="InitializerAttribute"/>-annotated method  as requesting an external resource by a specified
/// <see cref="Key"/>. Any <see cref="IResourceProvider"/> implementations may optionally provide a value for the
/// specified <see cref="Key"/> for the member's underlying type.
/// </summary>
/// <example>
/// A <see cref="BenchmarkClassAttribute"/>-annotated class may annotate an initializer <c>int[]</c> parameter with
/// <see cref="ResourceAttribute"/> where <c>Key="Data/File.txt"</c> to fetch a file's contents from a
/// <see cref="IResourceProvider"/>.
/// </example>
/// <example>
/// A <see cref="BenchmarkClassAttribute"/>-annotated class may annotate an <c>string</c> field with
/// <see cref="ResourceAttribute"/> where <c>Key="http://.../file.txt"</c> to fetch a web resource's contents from
/// a <see cref="IResourceProvider"/>.
/// </example>
/// <example>
/// A <see cref="ProjectEulerAttribute"/>-annotated class may annotate a <c>int[][]</c> property with
/// <see cref="ResourceAttribute"/> where <c>Key=null</c> to fetch a matrix from a Project Euler specific
/// <see cref="IResourceProvider"/>.
/// </example>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class ResourceAttribute : Attribute
{
    /// <summary>
    /// A key to request a resource from an <see cref="IResourceProvider"/>.
    /// </summary>
    public string? Key { get; init; }

    /// <param name="key">Sets <see cref="Key"/>.</param>
    /// <inheritdoc cref="ResourceAttribute"/>
    public ResourceAttribute(string? key = null)
    {
        Key = key;
    }
}
