using EventStore.ClientAPI;
using Qowaiv.DomainModel.EventSourcing;
using Qowaiv.EventStorage.Messages;
using Qowaiv.Validation.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Qowaiv.EventStorage
{
    public class AggregateRootEventStore<TAggregate>
        where TAggregate : AggregateRoot<TAggregate>, new()
    {
        /// <summary>Creates a new instance of an <see cref="AggregateRootEventStore{TAggregate}"/>.</summary>
        /// <param name="connection">
        /// The Connection to talk to.
        /// </param>
        /// <param name="options">
        /// The JSON serialization options (optional).
        /// </param>
        public AggregateRootEventStore(IEventStoreConnectionFactory factory, JsonSerializerOptions options, TypeMap typeMap)
        {
            ConnectionFactory = Guard.NotNull(factory, nameof(factory));
            Options = options;
            Map = typeMap ?? new TypeMap();
        }

        /// <summary>Creates an open Event Store factory.</summary>
        protected IEventStoreConnectionFactory ConnectionFactory { get; }

        /// <summary>Gets the JSON serialization options.</summary>
        protected JsonSerializerOptions Options { get; }

        /// <summary>Gets the Type Map.</summary>
        protected TypeMap Map { get; }

        /// <summary>Loads the aggregate root based on its identifier.</summary>
        /// <param name="id">
        /// the identifier of the model.
        /// </param>
        public async Task<Result<TAggregate>> LoadAsync(Guid id)
        {
            Guard.NotEmpty(id, nameof(id));

            var eventMessages = new List<EventMessage>();
            var stream = GetStream(id);

            StreamEventsSlice slice;
            var position = StreamPosition.Start;

            using (var connection = await ConnectionFactory.CreateOpenConnectionAsync())
            {
                do
                {
                    slice = await connection.ReadStreamEventsForwardAsync(stream, position, ReadBuffer, false);
                    position += ReadBuffer;

                    foreach (var resolvedEvent in slice.Events)
                    {
                        eventMessages.Add(AsEventMessage(id, resolvedEvent));
                    }
                }
                while (!slice.IsEndOfStream);
            }

            if (!eventMessages.Any())
            {
                return Result.WithMessages<TAggregate>(EntityNotFound.ForId(id));
            }

            var eventStream = EventStream.FromMessages(eventMessages);
            return AggregateRoot.FromEvents<TAggregate>(eventStream);
        }

        /// <summary>Saves the uncommitted events of the aggregate.</summary>
        /// <param name="model">
        /// The model to store.
        /// </param>
        public async Task<Result> StoreAsync(TAggregate model)
        {
            var eventStream = Guard.NotNull(model, nameof(model)).EventStream;

            if (!eventStream.GetUncommitted().Any())
            {
                return Result.WithMessages(ValidationMessage.Warn("No uncommitted events to store."));
            }

            long expectedVersion = eventStream.CommittedVersion - 1;

            var stream = GetStream(eventStream.AggregateId);

            var eventData = new List<EventData>(eventStream.Version - eventStream.CommittedVersion);

            foreach (var eventMessage in eventStream.GetUncommitted())
            {
                eventData.Add(AsEventData(eventMessage));
            }

            using (var connection = await ConnectionFactory.CreateOpenConnectionAsync())
            {
                await connection.AppendToStreamAsync(stream, expectedVersion, eventData);
            }
            eventStream.MarkAllAsCommitted();
            eventStream.ClearCommitted();

            return Result.OK;
        }

        /// <summary>Gets the (ID of the) stream.</summary>
        /// <param name="id">
        /// The ID of the aggregate.
        /// </param>
        /// <returns>
        /// A concatenated <see cref="string"/> containing the type of the aggregate and its ID.
        /// </returns>
        protected virtual string GetStream(Guid id) => $"{typeof(TAggregate).Name} {id}";

        /// <summary>Represents an <see cref="EventMessage"/> as <see cref="EventData"/>.</summary>
        protected virtual EventData AsEventData(EventMessage eventMessage)
        {
            Guard.NotNull(eventMessage, nameof(eventMessage));

            var data = eventMessage.Event;
            var type = Map.TryGetName(eventMessage.Event.GetType());
            var json = JsonSerializer.Serialize(data, data.GetType(), Options);
            var bytes = Encoding.UTF8.GetBytes(json);
            return new EventData(Guid.NewGuid(), type, true, bytes, Array.Empty<byte>());
        }

        /// <summary>Represents an <see cref="ResolvedEvent"/> as <see cref="EventMessage"/>.</summary>
        protected virtual EventMessage AsEventMessage(Guid aggregateId, ResolvedEvent resolvedEvent)
        {
            var recordedEvent = resolvedEvent.Event;

            var info = new EventInfo((int)recordedEvent.EventNumber + 1, aggregateId);
            var type = Map.TryGetType(recordedEvent.EventType);
            var bytes = new ReadOnlySpan<byte>(recordedEvent.Data);
            var @event = JsonSerializer.Deserialize(bytes, type, Options);

            return new EventMessage(info, @event);
        }

        private const int ReadBuffer = 1024;
    }
}
