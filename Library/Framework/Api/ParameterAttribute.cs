namespace Net.ProjectEuler.Framework.Api;

/// <summary>
/// Annotates a field or property as an input parameter that should influence the result of a solution method
/// <see cref="SolutionAttribute"/>. All input parameter members are expected to have a default value assigned, either
/// upon initialization or from a <see cref="ResourceAttribute"/>. The declaring class of the method must be annotated
/// with <see cref="ProjectEulerAttribute"/>. A parameter may have other constraint attributes present that indicate
/// a validation requirement for the parameter's value.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ParameterAttribute : Attribute
{
    /// <summary>
    /// A human-friendly description of the parameter's use and constraints.
    /// </summary>
    public string? Description { get; init; }
}
