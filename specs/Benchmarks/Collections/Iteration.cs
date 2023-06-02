namespace Benchmarks.Collections;

public class Iteration
{
    [Params(1000, 10_000, 100_000)]
    public int Count { get; set; }

    private object[] array = System.Array.Empty<object>();
    private List<object> list = new();
    private EventBuffer<int> buffer = Qowaiv.DomainModel.EventBuffer.Empty(17);

    [GlobalSetup]
    public void Setup()
    {
        var creation = new Creation() { Count = Count };
        creation.Setup();
        array = creation.Events;
        list = creation.List();
        buffer = creation.EventBuffer();
    }

    [Benchmark(Description = "array", Baseline = true)]
    public double Array() => Sum(array);

    [Benchmark(Description = "event buffer")]
    public double EventBuffer() => Sum(buffer);

    [Benchmark]
    public double List() => Sum(list);

    [Benchmark]
    public double Aggregate() => AggregateRoot.FromStorage<Aggregate_id, int>(buffer).Sum;

    private static double Sum(IEnumerable<object> events)
    {
        var sum = 0.0;
        foreach (var e in events.OfType<Added>())
        {
            sum += e.Addition;
        }
        return sum;
    }
}
