using Net.ProjectEuler.Framework.Model;
using Net.ProjectEuler.Framework.Service;

namespace Net.ProjectEuler.Framework.Api.Hooks;

// TODO: implementations for Project Euler attachments, matrices, and local files
/// <summary>
/// Provides a resource for a benchmark method to consume, specified by <see cref="ResourceAttribute.Key"/>.
/// </summary>
public interface IResourceProvider
{
    /// <inheritdoc cref="IResourceProvider"/>
    /// <param name="benchmark">The benchmark method requesting the resource.</param>
    /// <param name="request">A <see cref="ResourceAttribute.Key"/> request.</param>
    /// <returns>A <see cref="object"/> representation of the fetched resource.</returns>
    Task<object?> FetchAsync(Benchmark benchmark, ResourceAttribute request);
}
