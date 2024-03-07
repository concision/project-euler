namespace Net.ProjectEuler.Framework.Api;

/// <summary>
/// Annotates that a field, property, <see cref="InitializerAttribute"/>-annotated method parameter,
/// <see cref="InitializerAttribute"/>-annotated method parameter should have its value provided by the framework's
/// built dependency injection container (i.e. <see cref="IServiceProvider"/>).
/// </summary>
/// <remarks>
/// This attribute is useful for omitting a constructor for brevity, or for when a service is only needed during
/// initialization, or execution of one benchmark method.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class DependencyInjectAttribute : Attribute
{
    /// <summary>
    /// By default, the field, property, or parameter type is used to resolve the service from the
    /// <see cref="IServiceProvider"/>. However, if a more precise type is necessary, this property may be set to
    /// indicate the exact type to resolve from the <see cref="IServiceProvider"/>.
    /// </summary>
    public Type? TypeOverride { get; init; }

    /// <inheritdoc cref="DependencyInjectAttribute"/>
    /// <param name="typeOverride">Sets <see cref="TypeOverride"/>.</param>
    public DependencyInjectAttribute(Type? typeOverride = null)
    {
        TypeOverride = typeOverride;
    }
}
