using Benchmarks.Collections;

namespace Benchmarks;

public static class Program
{
    public static void Main(params string[] args)
    {
        BenchmarkRunner.Run<Creation>();
        BenchmarkRunner.Run<BatchCreation>();
        BenchmarkRunner.Run<Iteration>();
    }
}
