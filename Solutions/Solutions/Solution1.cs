using Net.ProjectEuler.Framework.Api;

namespace Dev.Concision.ProjectEuler.Solutions;

[ProjectEuler(1)]
public sealed class Solution1 : Solver<ulong>
{
    [Parameter] // [Sign(NonNegative)]
    public required int Target { get; set; } = 1000;

    [Parameter] // [Set] [Coprime] [Sign(Positive)]
    public required int[] Multiples { get; set; } = [3, 5];

    public override TestCase<ulong>[] TestCases =>
    [
        new() {Parameters = {[nameof(Target)] = 10, [nameof(Multiples)] = (int[]) [3, 5]}, Answer = 23},
        new() {Parameters = {[nameof(Target)] = 1000, [nameof(Multiples)] = (int[]) [3, 5]}, Answer = 233168},
    ];

    [Solution("Naive Iteration", Date = "2016-03-20")]
    public void NaiveIteration()
    {
        for (uint n = 1; n < Target; n++)
        {
            foreach (var multiple in Multiples)
            {
                if (n % multiple == 0)
                {
                    Answer += n;
                    break;
                }
            }
        }
    }

    [Solution("Arithmetic Sum")]
    public int ArithmeticSum()
    {
        throw new NotImplementedException();
    }
}
