using Qowaiv.DomainModel.Diagnostics.Contracts;
using Qowaiv.DomainModel.Dynamic;
using System.Collections.Generic;
using System.Linq;

namespace Qowaiv.DomainModel.Projections
{
    /// <summary>Defines the Project method on a <see cref="Projector{TProjection}"/>.</summary>
    public static class Projector
    {
        /// <summary>Create a projection based on the provided events.</summary>
        /// <typeparam name="TProjection">
        /// The type of the projection.
        /// </typeparam>
        /// <param name="projector">
        /// The projector.
        /// </param>
        /// <param name="events">
        /// The events to replay.
        /// </param>
        [Impure]
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
}
