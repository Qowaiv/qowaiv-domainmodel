namespace Benchmarks.Collections;

[MemoryDiagnoser]
public class BatchCreation
{
    [Params(1000, 10_000, 100_000)]
    public int Count { get; set; }

    public Added[] Events { get; private set; } = Array.Empty<Added>();

    [GlobalSetup]
    public void Setup()
    {
        var rnd = new Random(Count);
        Events = new Added[Count];
        for (int i = 0; i < Count; i++)
        {
            Events[i] = new Added(rnd.NextDouble());
        }
    }

    [Benchmark(Description = "List", Baseline = true)]
    public List<object> List()
    {
        var list = new List<object>();
        list.AddRange(Events.OfType<object>());
        return list;
    }

    [Benchmark(Description = "Event Buffer")]
    public EventBuffer<int> EventBuffer()
    {
        var buffer = Qowaiv.DomainModel.EventBuffer.Empty(17);
        return buffer.Add(Events);
    }
}
