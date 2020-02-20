using EventStorage.Abstractions;
using Qowaiv.DomainModel;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace EventStorage.Local
{
    public class LocalEventStore<TId> : IEventStore<TId>
    {
        private readonly DirectoryInfo root;
        private readonly TypeMap typeMap;
        private readonly JsonSerializerOptions options;

        public LocalEventStore([NotNull]DirectoryInfo root, [NotNull]TypeMap typeMap, [AllowNull]JsonSerializerOptions options)
        {
            this.root = root;
            this.typeMap = typeMap;
            this.options = options ?? new JsonSerializerOptions();
        }

        /// <inheritdoc/>
        public async Task<int> GetCurrentVersionAsync([NotNull]TId aggregateId)
        {
            var stream = await LoadJsonEventStreamAsync(aggregateId);
            return stream.Version;
        }

        /// <inheritdoc/>
        public async Task<EventBuffer<TId>> LoadAsync([NotNull]TId aggregateId, int firstEvent)
        {
            var stream = await LoadJsonEventStreamAsync(aggregateId);
            if (stream.Version == default)
            {
                throw AggregateNotFoundException.ForId(aggregateId);
            }

            return EventBuffer<TId>.FromStorage(aggregateId, firstEvent, stream.GetCombined(), FromCombined);
        }

        /// <inheritdoc/>
        public async Task SaveAsync([NotNull]EventBuffer<TId> buffer)
        {
            var stream = await LoadJsonEventStreamAsync(buffer.AggregateId);

            if (buffer.CommittedVersion != stream.Version)
            {
                throw new ConcurrencyException(buffer.CommittedVersion, stream.Version);
            }
            
            foreach (var @event in buffer.Uncommitted)
            {
                stream.Events.Add(@event);
                stream.Types.Add(typeMap.TryGetName(@event.GetType()));
            }

            var location = GetLocation(buffer.AggregateId);

            using var fileStream = new FileStream(location.FullName, FileMode.OpenOrCreate, FileAccess.Write);
            await JsonSerializer.SerializeAsync(fileStream, stream, typeof(JsonEventStream), options);
            await fileStream.FlushAsync();
            buffer.MarkAllAsCommitted();
        }

        private object FromCombined(Tuple<string, object> combined)
        {
            var type = typeMap.TryGetType(combined.Item1);
            var element = (JsonElement)combined.Item2;
            return element.Deserialize(type, options);
        }

        private async Task<JsonEventStream> LoadJsonEventStreamAsync(TId aggregateId)
        {
            var file = GetLocation(aggregateId);

            if (!file.Exists)
            {
                return new JsonEventStream { };
            }
            using var stream = file.OpenRead();
            return await JsonSerializer.DeserializeAsync<JsonEventStream>(stream, options);
        }

        private FileInfo GetLocation(TId aggregateId) => new FileInfo(Path.Combine(root.FullName, $"{aggregateId}.json"));
    }
}
