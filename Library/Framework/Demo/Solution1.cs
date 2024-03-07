// using Net.ProjectEuler.Framework.Api;
//
// namespace Net.ProjectEuler.Framework.Demo;
//
// [ProjectEuler(1, ExpectedAnswer = 3434)]
// public sealed class Solution1 : Solver<ulong>
// {
//     [Parameter]
//     [Sign(NonNegative)]
//     public required int Target { get; set; } = 1000;
//
//     [Parameter] [Set] [Coprime] [Sign(Positive)]
//     public required int[] Multiples { get; set; } = [3, 5];
//
//     [Parameter]
//     [Resource] // checks projecteuler.net problem for embedded matrix or file, and intelligent parses it
//     public required int[][] Matrix { get; set; }
//     
//     public override TestCase<ulong>[] TestCases =>
//     [
//         new() {Parameters = {[nameof(Target)] = 3, [nameof(Multiples)] = (int[]) [3, 5]}, Answer = 34},
//         new() {Parameters = {[nameof(Target)] = 3, [nameof(Multiples)] = (int[]) [3, 5]}, Answer = 34},
//         new() {Parameters = {[nameof(Target)] = 3, [nameof(Multiples)] = (int[]) [3, 5]}, Answer = 9},
//     ];
//
//     [Initializer]
//     public void Prepare(
//         [Resource("https://projecteuler.net/resources/documents/0081_matrix.txt")]
//         string rawMatrixText,
//         [Resource] // checks projecteuler.net problem for an embedded matrix or file
//         int[][] autoConvertedFromMatrix
//     )
//     {
//         Matrix = default!;
//         // manually parse matrix maybe?
//     }
//
//     [Solution("Naive Iteration", Date = "2016-03-20")]
//     public class Parallel(Solution1 solution) : ParallelBenchmark<Solution1, long>(solution, RealTime)
//     {
//         public override ulong Chunks()
//         {
//             throw new NotImplementedException();
//         }
//
//         public override long Work(int chunk, int total)
//         {
//             throw new NotImplementedException();
//         }
//
//         public void Combine((int chunkId, long work) x)
//         {
//         }
//     }
//
//
//     [Solution("Naive Iteration", Date = "2016-03-20")]
//     public void NaiveIteration()
//     {
//         for (uint n = 1; n < Target; n++)
//         {
//             foreach (var multiple in Multiples)
//             {
//                 if (n % multiple == 0)
//                 {
//                     Answer += n;
//                     break;
//                 }
//             }
//         }
//     }
//
//     /// <summary>
//     /// Adds up all multiples of 3 and 5 under 1000, excluding overlap (i.e. multiples of 15)
//     /// </summary>
//     /// <returns></returns>
//     [Solution("Arithmetic Sum")]
//     public int ArithmeticSum()
//     {
//         throw new InvalidOperationException();
//     }
// }
