using System.Numerics;
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
    [Include("https://projecteuler.net/resources/documents/0081_matrix.txt")]
    [Include] // checks projecteuler.net problem for an embedded matrix or file
    public void Prepare(string rawMatrixText, int[][] autoConvertedFromMatrix)
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
    public void ArithmeticSum()
    {
        // imagine I actually did the combinatorics
        Answer = /* ... */ 0;
        BigInteger x = 3;
        var z = x * (x + 1) / 3;
    }
}