using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Qowaiv.DomainModel
{
    /// <summary>Represents a collection of entity properties.</summary>
    [Serializable]
    public class PropertyCollection : Dictionary<string, object>
    {
        /// <summary>Initializes a new instance of the <see cref="PropertyCollection"/> class.</summary>
        /// <remarks>
        /// No publice empty constructor.
        /// </remarks>
        protected PropertyCollection(int capacity) : base(capacity) { }

        /// <summary>Initializes a new instance of the <see cref="PropertyCollection"/> class.</summary>
        protected PropertyCollection(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <summary>Clones the property collection.</summary>
        internal PropertyCollection Clone()
        {
            var clone = new PropertyCollection(Count);
            foreach (var kvp in this)
            {
                clone[kvp.Key] = kvp.Value;
            }
            return clone;
        }

        /// <summary>Creates the properties for the type.</summary>
        public static PropertyCollection Create(Type type)
        {
            Guard.NotNull(type, nameof(type));

            if (!Collections.TryGetValue(type, out var properties))
            {
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(IsEditableProperty)
                    .ToArray();

                properties = new PropertyCollection(props.Length);

                foreach (var desc in props)
                {
                    properties[desc.Name] = desc.PropertyType.IsValueType ? Activator.CreateInstance(desc.PropertyType) : null;
                }

                Collections.TryAdd(type, properties);
            }

            return properties.Clone();
        }

        private static bool IsEditableProperty(PropertyInfo prop)
        {
            // CanWrite only works on public set, but internal and private set are also needed.
            // Obviously, we don't want write only properies.
            return prop.GetSetMethod(true) != null
                && prop.CanRead;
        }

        /// <remarks>For performance, we cache the structure of the property collections.</remarks>
        private static readonly ConcurrentDictionary<Type, PropertyCollection> Collections = new ConcurrentDictionary<Type, PropertyCollection>();
    }
}
