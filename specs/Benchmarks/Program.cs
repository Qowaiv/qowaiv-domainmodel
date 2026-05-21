using Benchmarks.Collections;

namespace Benchmarks;

public static class Program
{
    public static void Main()
    {
        BenchmarkRunner.Run<Creation>();
        BenchmarkRunner.Run<BatchCreation>();
        BenchmarkRunner.Run<Iteration>();
    }
}
