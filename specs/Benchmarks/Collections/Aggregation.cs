namespace Benchmarks.Collections;

public class Aggregation
{
    [Params(100, 1_000, 10_000)]
    public int Count { get; set; }

    private object[] Events = System.Array.Empty<object>();

    [GlobalSetup]
    public void Setup()
    {
        var creation = new Creation() { Count = Count };
        creation.Setup();
        Events = creation.Events;
    }

    [Benchmark]
    public double Aggregate()
    {
        var aggregate = new Aggregate_id();
        foreach(var e in Events)
        {
            aggregate = aggregate.Add(e);
        }
        return aggregate.Sum;
    }
}
