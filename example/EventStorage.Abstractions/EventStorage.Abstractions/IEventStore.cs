using Qowaiv.DomainModel;
using System;
using System.Threading.Tasks;

namespace EventStorage.Abstractions
{
    /// <summary>An interface for loading and saving events from and to an <see cref="EventBuffer{TId}"/>.</summary>
    /// <typeparam name="TId">
    /// The type of the identifier of the event based aggregate (root).
    /// </typeparam>
    public interface IEventStore<TId>
    {
        /// <summary>Gets the current version of an aggregate.</summary>
        /// <param name="aggregateId">
        /// The identifier of the aggregate (root).
        /// </param>
        Task<int> GetCurrentVersionAsync(TId aggregateId);

        /// <summary>Loads events of the specified aggregate root.</summary>
        /// <param name="aggregateId">
        /// The identity of the aggregate (root).
        /// </param>
        Task<EventBuffer<TId>> LoadAsync(TId aggregateId) => LoadAsync(aggregateId, 1);

        /// <summary>Loads events of the specified aggregate root.</summary>
        /// <param name="aggregateId">
        /// The identity of the aggregate (root).
        /// </param>
        /// <param name="firstEvent">
        /// The version of the first event to include in the buffer.
        /// </param>
        Task<EventBuffer<TId>> LoadAsync(TId aggregateId, int firstEvent);

        /// <summary>Saves the uncommitted events of the <see cref="EventBuffer{TId}"/>.</summary>
        /// <param name="buffer">
        /// The <see cref="EventBuffer{TId}"/>.
        /// </param>
        Task SaveAsync(EventBuffer<TId> buffer);
    }
}
