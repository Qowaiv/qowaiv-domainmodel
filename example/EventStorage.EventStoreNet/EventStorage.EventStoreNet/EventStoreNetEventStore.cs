using EventStorage.Abstractions;
using EventStore.ClientAPI;
using Qowaiv.DomainModel;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EventStorage.EventStoreNet
{
    public class EventStoreNetEventStore<TId> : IEventStore<TId>, IDisposable
    {
        private readonly IEventStoreConnection connection;
        private readonly TypeMap typeMap;
        private readonly JsonSerializerOptions options;
        private const int ReadBuffer = 1024;

        public EventStoreNetEventStore([NotNull]IEventStoreConnection connection, [NotNull]TypeMap typeMap, [MaybeNull]JsonSerializerOptions options)
        {
            this.connection = connection;
            this.typeMap = typeMap;
            this.options = options ?? new JsonSerializerOptions();
        }

        /// <inheritdoc/>
        public Task<int> GetCurrentVersionAsync([NotNull]TId aggregateId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<EventBuffer<TId>> LoadAsync([NotNull]TId aggregateId, int firstEvent)
        {
            
            await GetOpenConnectionAsync();

            var position = firstEvent - 1L;
            var buffer = new EventBuffer<TId>(aggregateId, firstEvent);

            StreamEventsSlice slice;
            do
            {
                slice = await connection.ReadStreamEventsForwardAsync(aggregateId.ToString(), position, ReadBuffer, false);
                position += ReadBuffer;
                buffer.AddRange(slice.Events.Select(FromResolvedEvent));
            }
            while (!slice.IsEndOfStream);
            buffer.MarkAllAsCommitted();
            return buffer;
        }

        /// <inheritdoc/>
        public async Task SaveAsync([NotNull]EventBuffer<TId> buffer)
        {
            var expectedVersion = buffer.CommittedVersion - 1L;
            var eventData = buffer.SelectUncommitted(AsEventData);
            await (await GetOpenConnectionAsync()).AppendToStreamAsync(buffer.AggregateId.ToString(), expectedVersion, eventData);
            buffer.MarkAllAsCommitted();
        }

        private EventData AsEventData(TId aggregateId, int version, object @event)
        {
            var type = typeMap.TryGetName(@event.GetType());
            var json = JsonSerializer.Serialize(@event, @event.GetType(), options);
            var bytes = Encoding.UTF8.GetBytes(json);
            return new EventData(Guid.NewGuid(), type, true, bytes, Array.Empty<byte>());
        }

        private object FromResolvedEvent(ResolvedEvent resolvedEvent)
        {
            var @event = resolvedEvent.OriginalEvent;
            var data = @event.Data;
            var type = typeMap.TryGetType(@event.EventType);
            return JsonSerializer.Deserialize(data, type, options);
        }

        /// <summary>Gets an open <see cref="IEventStoreConnection"/>.</summary>
        private async Task<IEventStoreConnection> GetOpenConnectionAsync()
        {
            await connection.ConnectAsync();
            return connection;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
