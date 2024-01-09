namespace Net.ProjectEuler.Framework.Api;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field)]
public class IncludeAttribute : Attribute
{
    public string? Key { get; }

    public IncludeAttribute()
    {
    }

    public IncludeAttribute(string key)
    {
        Key = key;
    }
}