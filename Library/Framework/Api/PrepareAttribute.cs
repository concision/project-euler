namespace Net.ProjectEuler.Framework.Api;

[AttributeUsage(AttributeTargets.Method)]
public class PrepareAttribute : Attribute
{
    public int Priority { get; set; }
}