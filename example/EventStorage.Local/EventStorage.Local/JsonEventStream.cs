using System;
using System.Collections.Generic;

namespace EventStorage.Local
{
    /// <summary>Represents a JSON Event Stream.</summary>
    internal class JsonEventStream
    {
        /// <summary>Gets the version of the Event stream.</summary>
        public int Version => Events?.Count ?? default;

        /// <summary>Gets the events.</summary>
        public List<object> Events { get; set; } = new List<object>();

        /// <summary>Gets the types of the events.</summary>
        public List<string> Types { get; set; } = new List<string>();

        public IEnumerable<Tuple<string, object>> GetCombined()
        {
            for (var i = 0; i < Events.Count; i++)
            {
                yield return new Tuple<string, object>(Types[i], Events[i]);
            }
        }
    }
}
