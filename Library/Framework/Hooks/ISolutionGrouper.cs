using Net.ProjectEuler.Framework.Api;
using Net.ProjectEuler.Framework.Cli.Commands;

namespace Net.ProjectEuler.Framework.Hooks;

public interface ISolutionGrouper
{
    IEnumerable<(string Name, IReadOnlyList<SolutionMethod> Solutions)> Group(IEnumerable<SolutionMethod> solutions);
}

public class ProjectEulerAttributeSolutionGrouper : ISolutionGrouper
{
    public IEnumerable<(string Name, IReadOnlyList<SolutionMethod> Solutions)> Group(IEnumerable<SolutionMethod> solutions)
    {
        return solutions.GroupBy(solution => solution.BenchmarkAttribute switch
            {
                ProjectEulerAttribute attribute => $"Problem {attribute.Id}",
                _ => solution.SolverType.FullName!,
            })
            .Select(grouping => (Name: grouping.Key, Solutions: (IReadOnlyList<SolutionMethod>) grouping.ToArray()));
    }
}