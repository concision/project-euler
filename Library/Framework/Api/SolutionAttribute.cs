namespace Net.ProjectEuler.Framework.Api;

[AttributeUsage(AttributeTargets.Method)]
public class SolutionAttribute : Attribute
{
    public string Date { get; set; }

    public SolutionAttribute()
    {
    }

    public SolutionAttribute(string displayName)
    {
    }
}