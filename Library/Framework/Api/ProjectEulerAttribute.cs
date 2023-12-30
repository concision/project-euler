namespace Net.ProjectEuler.Framework.Api;

[AttributeUsage(AttributeTargets.Class)]
public class ProjectEulerAttribute : Attribute
{
    public ProjectEulerAttribute(uint id)
    {
    }
}