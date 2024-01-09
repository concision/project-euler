using Crayon;
using Nito.Disposables;

namespace Net.ProjectEuler.Framework.Service;

public interface IStopwatchService
{
    IReadOnlyList<TimedNode> Hierarchy { get; }

    TimeSpan LastElapsed { get; }

    IDisposable Start(string text);

    IEnumerable<string> TreeHierarchyRender(bool toCurrentDepth, Func<TimeSpan, string, string>? formatter = null);
}

public class TimedNode
{
    public int Depth { get; set; }
    public string Text { get; set; }
    public DateTime Started { get; set; }
    public DateTime Ended { get; set; }
}

public class StopwatchService(ITreeListRenderer renderer) : IStopwatchService
{
    private readonly object stateLock = new();

    private int depth;
    private readonly List<TimedNode> hierarchy = [];

    public IReadOnlyList<TimedNode> Hierarchy => hierarchy;

    public TimeSpan LastElapsed
    {
        get
        {
            var entry = hierarchy[^1];
            return (entry.Ended != DateTime.MinValue ? entry.Ended : DateTime.Now) - entry.Started;
        }
    }


    public IDisposable Start(string text)
    {
        TimedNode timedNode;
        lock (stateLock)
        {
            timedNode = new TimedNode {Depth = depth++, Text = text};
            hierarchy.Add(timedNode);
        }
        var disposable = new Disposable(() =>
        {
            lock (stateLock)
            {
                timedNode.Ended = DateTime.Now;
                depth--;
            }
        });
        timedNode.Started = DateTime.Now;
        return disposable;
    }

    public IEnumerable<string> TreeHierarchyRender(bool toCurrentDepth, Func<TimeSpan, string, string>? formatter = null)
    {
        // TODO: improve default formatter
        formatter ??= (duration, text) => Output.Bold().Black("[") + duration + Output.Bold().Black("]") + Output.White($" {text}");
        return renderer.Render(hierarchy.Select(node => (node.Depth, Text: formatter(node.Ended - node.Started, node.Text))));
    }
}