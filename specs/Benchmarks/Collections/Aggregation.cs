namespace Benchmarks.Collections;

public class Aggregation
{
    [Params(/*100,*/ 10_000/*, 10_000*/)]
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
    public double Aggregate_ApplyEvent()
    {
        var aggregate = new Aggregate_id();
        foreach(var e in Events)
        {
            aggregate = aggregate.AddEvent(e);
        }
        return aggregate.Sum;
    }

    [Benchmark]
    public double Aggregate_ApplyEvents()
    {
        var aggregate = new Aggregate_id();
        foreach (var e in Events)
        {
            aggregate = aggregate.AddEvents(e);
        }
        return aggregate.Sum;
    }
}
