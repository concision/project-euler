namespace Net.ProjectEuler.Framework.Api;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class InjectServiceAttribute : Attribute
{
    public InjectServiceAttribute()
    {
    }
}