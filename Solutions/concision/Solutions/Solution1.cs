using Net.ProjectEuler.Framework.Api;

namespace Me.Concision.ProjectEuler.Solutions;

[ProjectEuler(1)]
public sealed class Solution1 : Solver<ulong>
{
    [Parameter]
    public int Target { get; set; } = 1000;

    [Parameter]
    public int[] Multiples { get; set; } = [3, 5];

    [Parameter]
    [Include] // checks projecteuler.net problem for embedded matrix or file, and intelligent parses it
    public int[][] Matrix { get; set; }

    // or alternatively using a prepare
    [Prepare]
    public void Prepare(
        [Include("https://projecteuler.net/resources/documents/0081_matrix.txt")]
        string rawMatrixText,
        [Include] // checks projecteuler.net problem for an embedded matrix or file
        int[][] autoConvertedFromMatrix
    )
    {
        Matrix = null;
        // manually parse matrix maybe?
    }

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
        return -1;
    }
}