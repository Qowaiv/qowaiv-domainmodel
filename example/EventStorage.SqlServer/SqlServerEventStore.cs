using Dapper;
using Dapper.Contrib.Extensions;
using EventStorage.Abstractions;
using Qowaiv;
using Qowaiv.DomainModel;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace EventStorage.SqlServer
{
    public sealed class SqlServerEventStore<TId> : IEventStore<TId>, IDisposable
    {
        private readonly SqlConnection connection;
        private readonly TypeMap typeMap;
        private readonly JsonSerializerOptions options;

        public SqlServerEventStore([NotNull]SqlConnection connection, [NotNull]TypeMap typeMap, [MaybeNull]JsonSerializerOptions options)
        {
            this.connection = connection;
            this.typeMap = typeMap;
            this.options = options ?? new JsonSerializerOptions();
        }

        /// <inheritdoc/>
        public async Task<int> GetCurrentVersionAsync(TId aggregateId)
        {
            var conn = await GetOpenConnectionAsync();
            var par = new DynamicParameters();
            par.Add(nameof(aggregateId), aggregateId);
            return await conn.QueryFirstOrDefaultAsync<int>(Sql.SelectCurrentVersion, par);
        }

        /// <inheritdoc/>
        public async Task<EventBuffer<TId>> LoadAsync(TId aggregateId, int firstEvent)
        {
            var conn = await GetOpenConnectionAsync();
            var par = new DynamicParameters();
            par.Add(nameof(aggregateId), aggregateId);
            par.Add(nameof(firstEvent), firstEvent);

            var events = await conn.QueryAsync<EventRecord>(Sql.SelectEvents, par);

            if (!events.Any())
            {
                throw AggregateNotFoundException.ForId(aggregateId);
            }

            return EventBuffer<TId>.FromStorage(aggregateId, firstEvent, events, FromEventRecord);
        }

        /// <inheritdoc/>
        public async Task SaveAsync([NotNull]EventBuffer<TId> buffer)
        {
            var current = await GetCurrentVersionAsync(buffer.AggregateId);

            if (buffer.CommittedVersion != current)
            {
                throw new ConcurrencyException(current, buffer.CommittedVersion);
            }

            var conn = await GetOpenConnectionAsync();

            using (var transaction = conn.BeginTransaction())
            {
                var uncommitted = buffer.SelectUncommitted(AsEventRecord);

                foreach (var record in uncommitted)
                {
                    await conn.InsertAsync(record, transaction);
                }
                transaction.Commit();
            }
            buffer.MarkAllAsCommitted();
        }

        private EventRecord AsEventRecord(TId aggregateRoot, int version, object @event)
        {
            return new EventRecord
            {
                AggregateId = JsonSerializer.Serialize(aggregateRoot, options),
                Version = version,
                CreatedUtc = Clock.UtcNow(),
                EventType = typeMap.TryGetName(@event.GetType()),
                Data = JsonSerializer.Serialize(@event, @event.GetType(), options),
            };
        }

        private object FromEventRecord(EventRecord record)
        {
            var type = typeMap.TryGetType(record.EventType) ?? typeof(object);
            return JsonSerializer.Deserialize(record.Data, type, options);
        }

        /// <summary>Gets an open <see cref="SqlConnection"/>.</summary>
        private async Task<SqlConnection> GetOpenConnectionAsync()
        {
            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }
            return connection;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
