# TODO

A non-exhaustive list of planned features and libraries.

## GitHub Repository Configuration

- [x] `.gitignore` for C#, C++, and various IDEs/OSes
- [ ] `.gitattributes`
- [ ] `dependabot.yml`
- [ ] `.github` CI/CD
- **Documentation**:
    - [ ] `README.md`
    - [ ] `CHANGELOG.md`
    - [ ] `LICENSE.md`
    - [ ] `CODE_OF_CONDUCT.md`
    - [ ] `CONTRIBUTORS.md`
    - [ ] `ACKNOWLEDGEMENTS.md`
    - [ ] `CONTRIBUTING.md`

## C# Project Configuration

- [ ] C++ Native Integration with C#

## Libraries

### Profiling Framework

- [ ] Benchmark API (similar to MSTest API)
    - [ ] `[BenchmarkClass]` attribute (on classes only): declares `[Benchmark]` methods
    - [ ] `[Resource]` attribute (on fields, properties, or initializer/benchmark/solution method parameters):
      Requests a resource from a provider and automatic conversion between the resource representation and annotated
      type.
    - [ ] `[Initializer]` attribute (on methods only): initializes the benchmark class prior to a benchmark method
    - [ ] `[Benchmark]` attribute (on methods only): A benchmark method that performs some computation.
    - [ ] `IDisposable` and `IAsyncDisposable` support for cleanup

- [ ] Project Euler Solver API (extension of Benchmark API):
    - [ ] `[ProjectEuler]` attribute: declares a class as a Project Euler problem
    - [ ] `[Parameter]` support (on fields and properties): an input parameter that changes the behavior of the
      benchmark/solution method.
    - `[Parameter]` constraints:
        - [ ] `[Set]` - All elements in the set/list are unique
        - [ ] `[Coprime]` - All elements in the set/list are coprime
        - [ ] `[Prime]` - The value is, or elements in the set/list are prime
        - [ ] `[Sign]` - Non-negative, Negative, Positive, Non-positive
        - [ ] `[Monotonic]` - Increasing, Decreasing, Non-increasing, Non-decreasing
        - [ ] `[Clusivity]` - The parameter range is inclusive or exclusive
        - [ ] `[Parameter] public Expression Formula { get; set; }` support for math equations
    - `[Solution]` attribute (on methods only): a "solution method" which is a special type of benchmark method that
      computes a solution to a Project Euler problem.
    - [ ] Parallel-Chunked Execution API (`ParallelSolution`)
        - [ ] API for splitting work into chunks and aggregating work chunks into an answer
        - [ ] Pool of remotely networked workers that receive the work chunks and return the results
        - [ ] Resume-able execution / automatic reconnects to workers

- [ ] Command Line Interface:
    - **Global Parameters**:
        - [ ] `--verbosity`: Minimum logging level verbosity (e.g. TRACE, DEBUG, WARN, ERROR, FATAL)
        - [ ] `--selector`: Benchmark/solution method selection
            - [ ] `all`: Executes all benchmark classes
            - [x] `latest`: Executes the last modified benchmark class
            - [ ] `ide`: Executes any benchmark classes that are open in a supported IDE editor (e.g. Rider, Visual
              Studio Code)
            - [ ] `euler`: Project Euler range, e.g. `113-203,403`
            - [ ] `filter`: Additional CLI argument filters
        - [ ] `--timings`: Render a tree view of execution time for the requested command
    - **Commands**:
        - `list`: list all discovered benchmark/solution methods; with no selector, all discovered classes/methods will
          be listed in an ascii tree list
            - `--format [table | tree]`
        - `test`:
            - Arguments:
                - [ ] `--benchmark`: Executes all selected benchmark/solution methods repeatedly within a time limit or
                  iteration limit.
                - [ ] `--limit-iterations <integer>`: The maximum number of iterations to execute the benchmark/solution
                  method.
                - [ ] `--limit-time <time>`: The maximum amount of time to execute the benchmark/solution method. All
                  selected benchmark/solution methods will be executed at least once, even if the time limit is
                  exceeded.
                    - [ ] Advanced table result summary

- [ ] Logging:
    - [ ] Print object structures with colored JSON-like formatting
    - [ ] Automatic flood protection against repeated log messages
    - [ ] Ansi color support
    - [ ] Estimation of execution time based on current progress percentage (e.g. `Solver<T>.Completed`) using
      regression
    - [ ] Publish results to web server (with color preserved)
- [ ] VSTest adapter for IDE integration executing benchmark/solution methods as "tests",
  e.g. `Dev.Concision.ProjectEuler.TestAdapter`

### Ansi

- [ ] ANSI coloring library
    - [ ] Simple syntax `Color.Blue.Bold("Text").Bold.White.(": ").Color(YELLOW, "a").Color("a", r, g, b)`
    - [ ] Extension method support: `"text".Bold().Rgb(r, g, b)`
    - [ ] Extension method support: `object.Color()`
- [ ] Tree List Renderer: Renders ascii tree lists
- [ ] Table:
    - ANSI color formatting
    - Headers:
    - Column alignment: left, right, center
    - Multi-Cell spanning rows/columns
    - AddColumn() (with data)
    - AddRow() (with data)
    - Dynamic editing of cells

### Math

- `INumber` types
    - [ ] `Fraction<T>` type (e.g. BigInteger) representing a numerator and denominator
    - [ ] `Modular<T>` type for modular arithmetic integer types
- Generic math functions
    - [ ] Factorial
    - [ ] IsPerfectSquare
    - [ ] GCD/LCM
    - [ ] Chinese Remainder Theorem
    - [ ] Fast Exponentiation
- Generators
    - [ ] Palindrome (generation in order, any base, isPalindrome)
    - [ ] Pascal Triangle
- [ ] Polynomial Regression
- [ ] Binary Search on data or search spaces with a predicate
- [ ] Expression Engine
    - [ ] `Expression` class
    - [ ] Expression Parser
    - [ ] SI Units with automatic conversions
- Sieves:
    - [ ] Sieve of Eratosthenes (& Segmented)
    - [ ] Sieve of Atkins
    - [ ] Factorization Sieve
    - [ ] Divisor Count Sieve
    - [ ] Totient Sieve

#### Combinatorics

- **numbers**:
    - [ ] [**factorials**](https://en.wikipedia.org/wiki/Factorial):
        - [ ] [factorial](https://en.wikipedia.org/wiki/Factorial)
        - [ ] [falling factorial](https://en.wikipedia.org/wiki/Falling_and_rising_factorials)
        - [ ] [rising factorial](https://en.wikipedia.org/wiki/Falling_and_rising_factorials)
        - [ ] [primorial](https://en.wikipedia.org/wiki/Primorial)
    - [ ] [Multinomial coefficients](https://en.wikipedia.org/wiki/Multinomial_theorem#Multinomial_coefficients)
    - [ ] [Catalan numbers](https://en.wikipedia.org/wiki/Catalan_number)
    - [ ] [Fuss–Catalan numbers](https://en.wikipedia.org/wiki/Fuss%E2%80%93Catalan_number)
    - [ ] [Stirling numbers of the 1st kind](https://en.wikipedia.org/wiki/Stirling_numbers_of_the_first_kind)
    - [ ] [Stirling numbers of the 2nd kind](https://en.wikipedia.org/wiki/Stirling_numbers_of_the_second_kind)
    - [ ] [Lah numbers](https://en.wikipedia.org/wiki/Lah_number) ([Stirling numbers of the 3rd kind](https://en.wikipedia.org/wiki/Stirling_number#Lah_numbers))
    - [ ] [Bell numbers](https://en.wikipedia.org/wiki/Bell_number)
    - [ ] [Ordered Bell numbers](https://en.wikipedia.org/wiki/Ordered_Bell_number)
    - [ ] [Eulerian numbers](https://en.wikipedia.org/wiki/Eulerian_number)
    - [ ] [Lobb numbers](https://en.wikipedia.org/wiki/Lobb_number)
    - [ ] [Narayana numbers](https://en.wikipedia.org/wiki/Narayana_number)
    - [ ] [Motzkin numbers](https://en.wikipedia.org/wiki/Motzkin_number)
    - [ ] [Schröder numbers](https://en.wikipedia.org/wiki/Schr%C3%B6der_number)
- [**triangles**](https://en.wikipedia.org/wiki/Triangular_array): with triangular array generation, direct lookup
    - [ ] [Pascal's triangle](https://en.wikipedia.org/wiki/Pascal%27s_triangle)
    - [ ] [Catalan's triangle](https://en.wikipedia.org/wiki/Catalan%27s_triangle) ([Catalan's trapezoids](https://en.wikipedia.org/wiki/Catalan%27s_triangle#Generalization))
    - [ ] [Bell triangle](https://en.wikipedia.org/wiki/Bell_triangle)
    - [ ] [Euler's triangle](https://en.wikipedia.org/wiki/Eulerian_number#Basic_properties)
- [**cartesian products**](https://en.wikipedia.org/wiki/Cartesian_product):
    - [ ] counting cartesian products
    - [ ] cartesian product generator
    - [ ] next cartesian product (lexicographically ordered)
    - [ ] sample random cartesian product
- **sets**:
    - [subsets](https://en.wikipedia.org/wiki/Subset):
        - [ ] count subsets? (i.e. `2^|cardinality|`)
        - [ ] subset generator (i.e. [power set](https://en.wikipedia.org/wiki/Power_set) enumeration)
        - [ ] next subset (lexicographically ordered)
        - [ ] sample random subset
    - [multisets](https://en.wikipedia.org/wiki/Multiset):
        - [ ] [count multisets](https://en.wikipedia.org/wiki/Multiset#Counting_multisets)
        - [ ] length-bounded multiset generator
        - [ ] next length-bounded multiset (lexicographically ordered)
        - [ ] sample random length-bounded multiset
    - [set partitions](https://en.wikipedia.org/wiki/Partition_of_a_set)
        - [ ] [count set partitions](https://en.wikipedia.org/wiki/Partition_of_a_set#Counting_partitions)
        - [ ] set partitions generator
        - [ ] next set partition (lexicographically ordered)
        - [ ] sample random set partition
    - [ ] [ordered partitions](https://en.wikipedia.org/wiki/Weak_ordering#Ordered_partitions)
- [**permutations**](https://en.wikipedia.org/wiki/Permutation):
    - [permutations without repetitions](https://en.wikipedia.org/wiki/Permutation)
        - [ ] count permutations without repetitions `(n permutation k)`
        - [ ] permutations without repetitions generator
        - [ ] next permutation (lexicographically ordered)
        - [ ] sample random permutation permutations with repetitions
    - [permutations with repetitions](https://en.wikipedia.org/wiki/Permutation#Permutations_with_repetition)
        - [ ] count permutations with repetitions
        - [ ] permutations with repetitions generator
        - [ ] next permutations with repetitions (lexicographically ordered)
        - [ ] sample random permutation with repetitions
- [**combinations**](https://en.wikipedia.org/wiki/Combination):
    - [combinations without repetitions](https://en.wikipedia.org/wiki/Combination)
        - [ ] count
          combinations `(n choose k)` ([binomial coefficient](https://en.wikipedia.org/wiki/Binomial_coefficient))
        - [ ] simple combinations generator
        - [ ] next combination (lexicographically ordered)
        - [ ] sample random combination permutations with repetitions
    - [combinations with repetitions](https://en.wikipedia.org/wiki/Combination#Number_of_combinations_with_repetition)
        - [ ] count combinations with repetitions (n choose k)
        - [ ] combinations with repetitions generator
        - [ ] next combination with repetitions (lexicographically ordered)
        - [ ] sample random combination with repetitions (with seeded rand support)
- [**derangements**](https://en.wikipedia.org/wiki/Derangement):
    - [ ] count derangements (subfactorial, n-th de Montmort number)
    - [ ] derangements generator
    - [ ] next derangement (lexicographically ordered)
    - [ ] sample random derangement (with seeded rand support)
- **variations**:
    - [ ] [k-permutations of n](https://en.wikipedia.org/wiki/Permutation#k-permutations_of_n) (variations without
      repetition)
    - [ ] [n-tuples of m-sets](https://en.wikipedia.org/wiki/Tuple#n-tuples_of_m-sets) (variations with repetition)
- [**integer partitions**](https://en.wikipedia.org/wiki/Partition_(number_theory)):
    - [ ] [count integer partitions](https://en.wikipedia.org/wiki/Partition_(number_theory)#Partition_function)
    - [ ] integer partitions generator
- [**integer compositions**](https://en.wikipedia.org/wiki/Composition_(combinatorics)):
    - [ ] [count integer compositions](https://en.wikipedia.org/wiki/Composition_(combinatorics)#Number_of_compositions)
    - [ ] integer compositions generator
    - [ ] [count length-bounded weak integer compositions](https://en.wikipedia.org/wiki/Composition_(combinatorics)#Number_of_compositions)
    - [ ] length-bounded weak compositions generator
