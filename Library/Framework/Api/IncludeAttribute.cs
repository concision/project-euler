namespace Net.ProjectEuler.Framework.Api;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class IncludeAttribute : Attribute
{
    public IncludeAttribute()
    {
    }
    
    public IncludeAttribute(string url)
    {
    }
}