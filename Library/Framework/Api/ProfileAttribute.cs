namespace Net.ProjectEuler.Framework.Api;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ProfileAttribute : Attribute
{
    public bool ForcedExecution { get; set; }

    public uint Iterations { get; set; }
}