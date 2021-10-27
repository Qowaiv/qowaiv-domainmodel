using Qowaiv.DomainModel.Dynamic;
using System.Collections.Generic;
using System.Linq;

namespace Qowaiv.DomainModel.Projections
{
    public static class Projector
    {
        public static TProjection Project<TProjection>(this IEnumerable<object> events, Projector<TProjection> projector)
        {
            Guard.NotNull(events, nameof(events));
            Guard.NotNull(projector, nameof(projector));

            var dispatcher = DynamicEventDispatcher.New(projector);
            dynamic dynamic = dispatcher;
            foreach(var @event in events.Where(e => dispatcher.SupportedEventTypes.Contains(e.GetType())))
            {
                dynamic.When(@event);
            }
            return projector.Projection();
        }

        public static TProjection Project<TProjection>(this Projector<TProjection> projector, IEnumerable<object> events)
        {
            Guard.NotNull(events, nameof(events));
            Guard.NotNull(projector, nameof(projector));

            var dispatcher = DynamicEventDispatcher.New(projector);
            dynamic dynamic = dispatcher;
            foreach (var @event in events.Where(e => dispatcher.SupportedEventTypes.Contains(e.GetType())))
            {
                dynamic.When(@event);
            }
            return projector.Projection();
        }
    }

    public interface Projector<out TProjection> 
    {
        TProjection Projection();
    }
}
