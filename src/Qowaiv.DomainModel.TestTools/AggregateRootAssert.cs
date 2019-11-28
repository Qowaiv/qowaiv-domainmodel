using Qowaiv.DomainModel.EventSourcing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Qowaiv.DomainModel.TestTools
{
    /// <summary>Assertions on the aggregate root.</summary>
    public static class AggregateRootAssert
    {
        /// <summary>Verifies that the <see cref="EventSourcedAggregateRoot{TAggregate}"/> has the expected uncommitted events.</summary>
        /// <typeparam name="TAggregate">
        /// The type of the aggregate.
        /// </typeparam>
        /// <param name="actualAggregate">
        /// The actual aggregate to verify.
        /// </param>
        /// <param name="expectedEvents">
        /// The expected event messages.
        /// </param>
        public static void HasUncommittedEvents<TAggregate>(TAggregate actualAggregate, params object[] expectedEvents) where TAggregate : EventSourcedAggregateRoot<TAggregate>
        {
            Assert.IsNotNull(actualAggregate, nameof(actualAggregate));
            HasUncommittedEvents(actualAggregate.EventStream, expectedEvents);
        }

        /// <summary>Verifies that the <see cref="EventStream"/> has the expected uncommitted events.</summary>
        /// <param name="actualStream">
        /// The actual aggregate to verify.
        /// </param>
        /// <param name="expectedEvents">
        /// The expected event messages.
        /// </param>
        public static void HasUncommittedEvents(EventStream actualStream, params object[] expectedEvents)
        {
            Guard.NotNull(expectedEvents, nameof(expectedEvents));

            Assert.IsNotNull(actualStream, nameof(actualStream));

            var offset = actualStream.CommittedVersion;

            var uncomitted = actualStream.GetUncommitted().ToArray();

            var shared = Math.Min(expectedEvents.Length, uncomitted.Length);

            var sb = new StringBuilder().AppendLine("Assertion failed:");

            // if different lengths 
            bool failure = expectedEvents.Length != uncomitted.Length;

            for (var i = 0; i < shared; i++)
            {
                var act = uncomitted[i].Event;
                var exp = expectedEvents[i];

                if (EventEqualityComparer.Instance.Equals(act, exp))
                {
                    sb.AppendIdenticalEvent(offset + i, act);
                }
                else
                {
                    failure = true;
                    sb.AppendDifferentEvents(offset + i, exp, act);
                }
            }

            sb.AppendExtraEvents(uncomitted.Select(m => m.Event), offset, shared, "Extra:  ");
            sb.AppendExtraEvents(expectedEvents, offset, shared, "Missing: ");

            Console.WriteLine(sb);

            if (failure)
            {
                Assert.Fail(sb.ToString());
            }
        }

        private static void AppendIdenticalEvent(this StringBuilder sb, int index, object @event)
        {
            sb.AppendLine($"[{index}] {@event.GetType().Name}");
        }

        private static void AppendDifferentEvents(this StringBuilder sb, int index, object exp, object act)
        {
            var actType = act.GetType();
            var expType = exp.GetType();

            var prefix = $"[{index}] ";
            var empty = new string(' ', prefix.Length);

            sb.Append(prefix + "Expected: ");

            if (actType != expType)
            {
                sb.AppendLine(expType.ToString());
                sb.AppendLine($"{empty}Actual:   {actType}");
            }
            else
            {
                var sbExp = new StringBuilder().Append("{ ");
                var sbAct = new StringBuilder().Append("{ ");

                var properties = expType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach(var prop in properties)
                {
                    var e = prop.GetValue(exp, Array.Empty<object>());
                    var a = prop.GetValue(act, Array.Empty<object>());

                    if (!Equals(e, a))
                    {
                        sbExp.Append($"{prop.Name}: {e}, ");
                        sbAct.Append($"{prop.Name}: {a}, ");
                    }
                }

                sbExp.Remove(sbExp.Length - 2, 2);
                sbAct.Remove(sbAct.Length - 2, 2);

                sbExp.Append(" }");
                sbAct.Append(" }");
                sb.AppendLine(sbExp.ToString());
                sb.AppendLine($"{empty}Actual:   {sbAct}");
            }
        }

        private static void AppendExtraEvents(this StringBuilder sb, IEnumerable<object> events, int offset, int skip, string prefix)
        {
            var index = offset + skip;

            foreach (var @event in events.Skip(skip))
            {
                sb.AppendLine($"[{index}] {prefix} {@event.GetType().Name}");
                index++;
            }
        }
    }
}
