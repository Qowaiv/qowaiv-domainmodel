using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Qowaiv.DomainModel.EventSourcing
{
    /// <summary>Represents an event an its info.</summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class EventMessage
    {
        /// <summary>Initializes a new instance of the <see cref="EventMessage"/> class.</summary>
        /// <param name="info">
        /// The generic info about the event.
        /// </param>
        /// <param name="event">
        /// The actual event (data).
        /// </param>
        public EventMessage(EventInfo info, object @event)
        {
            Info = Guard.NotDefault(info, nameof(info));
            Event = Guard.NotNull(@event, nameof(@event));
        }

        /// <summary>Gets the event info.</summary>
        public EventInfo Info { get; }

        /// <summary>Gets the event.</summary>
        public object Event { get; }

        /// <summary>Represents the event message as a DEBUG <see cref="string"/>.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal string DebuggerDisplay
        {
            get
            {
                var props = Event.GetType()
                 .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                 .ToArray();

                // Get rid of Event suffix.
                const string suffix = nameof(Event);
                var eventName = Event.GetType().Name;
                if (eventName.EndsWith(suffix, StringComparison.InvariantCulture))
                {
                    eventName = eventName.Substring(0, eventName.Length - suffix.Length);
                }

                var sb = new StringBuilder()
                    .AppendFormat(eventName)
                    .AppendFormat(", Version: {0}", Info.Version)
                    .AppendFormat(", Props[{0}] {{ ", props.Length);

                for (var i = 0; i < props.Length; i++)
                {
                    var prop = props[i];
                    if (i != 0)
                    {
                        sb.Append(", ");
                    }
                    sb.AppendFormat("{0}: {1}", prop.Name, prop.GetValue(Event));
                }

                sb.Append(" }");
                sb.AppendFormat(@", Aggregate: {0:B}", Info.AggregateId);
                return sb.ToString();
            }
        }
    }
}
