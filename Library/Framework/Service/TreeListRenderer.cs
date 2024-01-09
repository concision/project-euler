namespace Net.ProjectEuler.Framework.Service;

public interface ITreeListRenderer
{
    IEnumerable<string> Render(IEnumerable<(int Depth, string Text)> hierarchy);
}

public class TreeListRenderer : ITreeListRenderer
{
    public IEnumerable<string> Render(IEnumerable<(int Depth, string Text)> hierarchy)
    {
        // TODO: ascii tree view like https://ascii-tree-generator.com/
        throw new NotImplementedException();
    }
}