using Qowaiv.DomainModel.Reflection;

namespace Qowaiv.DomainModel;

/// <summary>Factory for creating <see cref="ExpressionCompilingEventDispatcher{TDispatcher}"/>'s.</summary>
public interface EventDispatcher
{
    /// <summary>Gets the supported event types.</summary>
    public abstract ReadOnlySet<Type> SupportedEventTypes { get; }

    /// <summary>Invokes the When(@event) method.</summary>
    public abstract void When(object? @event);
}
