namespace Dev.Concision.ProjectEuler.Library;

/// <summary>
/// TODO: rewrite this
/// </summary>
public class AllocatedPrimeGenerator
{
    // constraints
    private readonly long limit;

    // calculated primes
    private long[]? blocks;

    public AllocatedPrimeGenerator(long limit, bool generate)
    {
        this.limit = limit;

        if (generate)
        {
            GeneratePrimes();
        }
    }

    public void GeneratePrimes()
    {
        int blocksCount = (int) Math.Ceiling(limit / 128.0);
        long root = (int) Math.Ceiling(Math.Sqrt((long) blocksCount << 7));

        // block bit pattern, [127 125 ... 1, 255 253 .. 129, ...]
        blocks = new long[blocksCount]; // equivalent statement: (int) (((limit - 1) >>> 7) + 1)

        // mark 1 as non-prime
        blocks[0] = 1L;
        // exclude numbers above specified limit (some serious bitwise fuckery)
        blocks[^1] |= ~0L << 1 << (int) (((limit + 1) >>> 1 & 63) - 1);

        // current number
        long number = 1;

        // the block with the sqrt of the limit, generate OR wheels until then
        long cap = (long) Math.Ceiling((double) root / 128.0);

        // iterate blocks
        for (int b = 0; b < cap; b++)
        {
            long block = blocks[b];

            // iterate numbers
            for (int i = 0; i < 64; i++)
            {
                // check if prime
                if ((block & 1) == 0)
                {
                    long start = number * number;

                    if (number < 64)
                    {
                        // Optimization for primes less than 64

                        int startBlock = (int) (start >>> 7);
                        long startIndex = start >> 1 & 63;

                        int pos = (int) (startIndex % number);

                        int cycleLength = (int) number;
                        long[] bitCycle = new long[cycleLength];

                        // generate first block sequence
                        long bits = 0L;
                        do
                        {
                            bits |= 1L << pos;
                            pos += (int) number;
                        } while (pos < 64);
                        bitCycle[0] = bits;

                        // build rest of sequence by shifting
                        int shiftDistance = (int) ((64 / number + 1) * number % 64);
                        for (int c = 1; c < cycleLength; c++)
                        {
                            // reset position
                            pos %= 64;
                            // jump to next multiple of number above 64
                            pos += (int) (((64 - pos) / number + 1) * number);
                            // shift bits, and then add extra bit
                            bitCycle[c] = bitCycle[c - 1] << shiftDistance | 1L << (int) (pos % number);
                        }

                        // spin wheel
                        for (int c = 0, curBlock = startBlock + c; curBlock < blocksCount; c++)
                        {
                            blocks[curBlock] |= bitCycle[c % cycleLength];
                            curBlock++;
                        }
                        // special case
                        if (startBlock == b)
                        {
                            // mark number as prime
                            blocks[b] = blocks[b] & ~(1L << i);
                            // mark off numbers in current block as not prime
                            block |= bitCycle[0] >>> i;
                        }
                    }
                    else
                    {
                        // check off all multiples as non-prime
                        for (long m = start, twicePrime = number << 1; m < limit; m = m + twicePrime)
                        {
                            blocks[(int) (m >>> 7)] |= 1L << (int) ((m & 127) >> 1);
                        }
                    }
                }

                // count up odd number
                number += 2;
                // shift block
                block >>>= 1;
            }
        }
    }

    public ulong[] ListPrimes()
    {
        if (blocks is null)
            throw new InvalidOperationException($"Generate primes first via {nameof(GeneratePrimes)}()");
        var primes = new ulong[blocks.Select(block => long.PopCount(~block)).Sum() + 1];
        primes[0] = 2;

        int position = 1;

        ulong value = 1;
        foreach (long block in blocks)
        {
            var modifiedBlock = block;
            for (int i = 0; i < 64; i++)
            {
                if ((modifiedBlock & 1) == 0)
                {
                    primes[position++] = value;
                }
                value += 2;
                modifiedBlock >>>= 1;
            }
        }

        return primes;
    }

    public bool IsPrime(long n)
    {
        if (blocks is null)
            throw new InvalidOperationException($"Generate primes first via {nameof(GeneratePrimes)}()");
        long block = n >>> 7;
        if (n % 2 == 1)
        {
            return block < blocks.Length && (blocks[(int) block] & 1L << (int) ((n & 127) >> 1)) == 0;
        }
        else
        {
            return n == 2;
        }
    }

    public static ulong[] Primes(long limit)
    {
        return new AllocatedPrimeGenerator(limit, true).ListPrimes();
    }
    public static ulong[] NPrimes(int count)
    {
        return Primes(IncreasingSequenceSearch((long x) => (long) Math.Floor(x / Math.Log(x)), count)).Take(count).ToArray();
    }
    
    
    /**
     * Finds a x given a y such that `f(x) = y` in a strictly increasing function
     * Effectively a binary search without a specified upper bound
     */
    private static int IncreasingSequenceSearch(Func<long, long> sequence, int value) {
        int upperLimit = 1;
        long limit;
        do {
            upperLimit <<= 1;
            limit = sequence(upperLimit);
        } while (limit <= value);

        return BinarySearch(sequence, upperLimit >>> 1, upperLimit, value);
    }

    /**
     * Adaptation of Arrays#binarySearch, but with a function sequence
     */
    private static int BinarySearch(Func<long, long> sequence, int fromIndex, int toIndex, long key) {
        int low = fromIndex;
        int high = toIndex - 1;

        while (low <= high) {
            int mid = (low + high) >>> 1;
            long midVal = sequence(mid);

            if (midVal < key) {
                low = mid + 1;
            } else if (midVal > key) {
                high = mid - 1;
            } else {
                return mid; // key found
            }
        }
        return low + 1;
    }

    public bool[] LookupTable()
    {
        bool[] isPrime = new bool[(int) (limit + 1)];
        foreach (long prime in ListPrimes())
        {
            isPrime[(int) prime] = true;
        }
        return isPrime;
    }
}