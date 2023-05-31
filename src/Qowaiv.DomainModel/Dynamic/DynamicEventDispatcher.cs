using System.Dynamic;

namespace Qowaiv.DomainModel.Dynamic;

/// <summary>Factory for creating <see cref="DynamicEventDispatcher{TDispatcher}"/>'s.</summary>
public abstract class DynamicEventDispatcher : DynamicObject
{
    /// <summary>Gets the supported event types.</summary>
    public abstract ReadOnlySet<Type> SupportedEventTypes { get; }

    /// <summary>Creates a new instance of the <see cref="DynamicEventDispatcher{TDispatcher}"/> class.</summary>
    [Pure]
    public static DynamicEventDispatcher New(object dispatcher)
    {
        Guard.NotNull(dispatcher, nameof(dispatcher));
        var type = typeof(DynamicEventDispatcher<>).MakeGenericType(dispatcher.GetType());
        return (DynamicEventDispatcher)Activator.CreateInstance(type, dispatcher)!;
    }
}
