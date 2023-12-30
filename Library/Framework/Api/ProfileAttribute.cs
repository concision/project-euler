namespace Net.ProjectEuler.Framework.Api;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ProfileAttribute : Attribute
{
    public ProfileAttribute()
    {
    }
}