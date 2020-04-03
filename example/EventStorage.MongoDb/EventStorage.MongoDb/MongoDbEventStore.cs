using EventStorage.Abstractions;
using MongoDB.Driver;
using Qowaiv.DomainModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace EventStorage.MongoDb
{
    public sealed class MongoDbEventStore<TId> : IEventStore<TId>
    {
        private readonly IMongoDatabase database;
        private readonly TypeMap typeMap;
        private static readonly InsertManyOptions insertOptions = new InsertManyOptions { IsOrdered = true };

        public MongoDbEventStore([NotNull]IMongoDatabase database, [NotNull]TypeMap typeMap)
        {
            this.database = database;
            this.typeMap = typeMap;
        }

        /// <inheritdoc/>
        public async Task<int> GetCurrentVersionAsync([NotNull]TId aggregateId)
        {
            var cursor = await GetCollection()
                .FindAsync(filter => filter.AggregateId == aggregateId.ToString());

            return cursor.ToEnumerable().LastOrDefault()?.Version ?? 0;
        }

        /// <inheritdoc/>
        public async Task<EventBuffer<TId>> LoadAsync([NotNull]TId aggregateId, int firstEvent)
        {
            var cursor = await GetCollection()
               .FindAsync(filter => filter.AggregateId == aggregateId.ToString() && filter.Version >= firstEvent);

            var buffer = EventBuffer<TId>.FromStorage(aggregateId, cursor.ToEnumerable(), FromEventDocument);

            if (buffer.IsEmpty)
            {
                throw AggregateNotFoundException.ForId(aggregateId);
            }

            return buffer;
        }

        /// <inheritdoc/>
        public async Task SaveAsync([NotNull]EventBuffer<TId> buffer)
        {
            var documents = buffer.SelectUncommitted(AsEventDocument);
            await GetCollection().InsertManyAsync(documents, insertOptions);
            buffer.MarkAllAsCommitted();
        }

        private EventDocument AsEventDocument(TId aggregateId, int version, object @event)
        {
            return new EventDocument
            {
                AggregateId = aggregateId.ToString(),
                Version = version,
                EventType = typeMap.TryGetName(@event.GetType()),
                PayLoad = @event,
            };
        }

        private object FromEventDocument(EventDocument document) => document.PayLoad;

        private IMongoCollection<EventDocument> GetCollection() => database.GetCollection<EventDocument>("events");
    }
}
