namespace Benchmarks.Collections;

[MemoryDiagnoser]
public class BatchCreation
{
    [Params(1000, 10_000, 100_000)]
    public int Count { get; set; }

    public IReadOnlyCollection<object> Events { get; private set; } = Array.Empty<object>();

    [GlobalSetup]
    public void Setup() => Events = Added.Random(Count);

    [Benchmark(Baseline = true)]
    public List<object> List() 
        => new(Events.Where(o => o is { }));

    [Benchmark(Description = "Event Buffer")]
    public EventBuffer<int> EventBuffer()
    {
        var buffer = Qowaiv.DomainModel.EventBuffer.Empty(17);
        return buffer.Add(Events);
    }
}
