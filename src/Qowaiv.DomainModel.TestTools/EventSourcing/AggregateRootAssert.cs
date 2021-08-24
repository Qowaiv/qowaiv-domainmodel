using Qowaiv.Validation.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Qowaiv.DomainModel.TestTools.EventSourcing
{
    /// <summary>Assertions on the aggregate root.</summary>
    public static class AggregateRootAssert
    {
        /// <summary>Verifies that the <see cref="AggregateRoot{TAggregate}"/> has the expected uncommitted events.</summary>
        /// <typeparam name="TAggregate">
        /// The type of the aggregate.
        /// </typeparam>
        /// <typeparam name="TId">
        /// The type of the identifier of the aggregate.
        /// </typeparam>
        /// <param name="actualAggregate">
        /// The actual aggregate to verify.
        /// </param>
        /// <param name="expectedEvents">
        /// The expected event messages.
        /// </param>
        public static void HasUncommittedEvents<TAggregate, TId>(Result<TAggregate> actualAggregate, params object[] expectedEvents)
            where TAggregate : AggregateRoot<TAggregate, TId>, new()
        {
            Assert.IsNotNull(actualAggregate, nameof(actualAggregate));

            if (!actualAggregate.IsValid)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"The aggregate root {typeof(TAggregate).Name} is invalid:");
                foreach (var message in actualAggregate.Messages)
                {
                    sb.Append("- ").AppendLine(message.ToString());
                }
                Assert.Fail(sb.ToString());
            }
            HasUncommittedEvents<TAggregate, TId>(actualAggregate.Value, expectedEvents);
        }

        /// <summary>Verifies that the <see cref="AggregateRoot{TAggregate}"/> has the expected uncommitted events.</summary>
        /// <typeparam name="TAggregate">
        /// The type of the aggregate.
        /// </typeparam>
        /// <typeparam name="TId">
        /// The type of the identifier of the aggregate.
        /// </typeparam>
        /// <param name="actualAggregate">
        /// The actual aggregate to verify.
        /// </param>
        /// <param name="expectedEvents">
        /// The expected event messages.
        /// </param>
        public static void HasUncommittedEvents<TAggregate, TId>(TAggregate actualAggregate, params object[] expectedEvents)
            where TAggregate : AggregateRoot<TAggregate, TId>, new()
        {
            Assert.IsNotNull(actualAggregate, nameof(actualAggregate));

            HasUncommittedEvents(actualAggregate.Buffer, expectedEvents);
        }

        /// <summary>Verifies that the <see cref="EventBuffer{TId}"/> has the expected uncommitted events.</summary>
        /// <typeparam name="TId">
        /// The type of the identifier of the aggregate.
        /// </typeparam>
        /// <param name="actualBuffer">
        /// The actual aggregate to verify.
        /// </param>
        /// <param name="expectedEvents">
        /// The expected event messages.
        /// </param>
        public static void HasUncommittedEvents<TId>(EventBuffer<TId> actualBuffer, params object[] expectedEvents)
        {
            Guard.NotNull(expectedEvents, nameof(expectedEvents));

            Assert.IsNotNull(actualBuffer, nameof(actualBuffer));

            var sb = new StringBuilder();
            var failure = false;
            var offset = actualBuffer.CommittedVersion;
            var uncomitted = actualBuffer.Uncommitted.ToArray();
            var shared = Math.Min(expectedEvents.Length, uncomitted.Length);

            for (var i = 0; i < shared; i++)
            {
                failure |= sb.AppendEvents(offset + i, expectedEvents[i], uncomitted[i]);
            }

            failure |= sb.AppendExtraEvents(uncomitted, offset, shared, "Extra:  ");
            failure |= sb.AppendExtraEvents(expectedEvents, offset, shared, "Missing: ");

            if (failure)
            {
                sb.Insert(0, $"Assertion failed:{Environment.NewLine}");
                Assert.Fail(sb.ToString());
            }
        }

        private static bool AppendEvents(this StringBuilder sb, int index, object exp, object act)
        {
            if (sb.AppendDifferentTypes(index, exp, act))
            {
                return true;
            }

            if (sb.AppendDifferentEvents(index, exp, act))
            {
                return true;
            }

            return sb.AppendIdenticalEvents(index, act);
        }

        private static bool AppendDifferentTypes(this StringBuilder sb, int index, object exp, object act)
        {
            var actType = act.GetType();
            var expType = exp.GetType();

            if (actType != expType)
            {
                return sb.AppendExpectedActual(index, expType, actType);
            }

            return false;
        }

        private static bool AppendDifferentEvents(this StringBuilder sb, int index, object exp, object act)
        {
            var failure = false;

            var sbExp = new StringBuilder().Append("{ ");
            var sbAct = new StringBuilder().Append("{ ");

            var properties = exp.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                var e = prop.GetValue(exp, Array.Empty<object>());
                var a = prop.GetValue(act, Array.Empty<object>());

                if (prop.PropertyType.IsArray)
                {
                    var arrayE = (Array)e;
                    var arrayA = (Array)a;

                    var e_ = new object[arrayE.Length];
                    var a_ = new object[arrayA.Length];

                    Array.Copy(arrayE, e_, e_.Length);
                    Array.Copy(arrayA, a_, a_.Length);

                    if (!Enumerable.SequenceEqual(e_, a_))
                    {
                        failure = true;

                        sbExp.Append($"{prop.Name}: [ {string.Join(", ", e_)} ], ");
                        sbAct.Append($"{prop.Name}: [ {string.Join(", ", a_)} ], ");
                    }
                }
                else
                {
                    if (!Equals(e, a))
                    {
                        failure = true;
                        sbExp.Append($"{prop.Name}: {e}, ");
                        sbAct.Append($"{prop.Name}: {a}, ");
                    }
                }
            }

            sbExp.Remove(sbExp.Length - 2, 2);
            sbAct.Remove(sbAct.Length - 2, 2);

            sbExp.Append(" }");
            sbAct.Append(" }");

            if (failure)
            {
                return sb.AppendExpectedActual(index, sbExp, sbAct);
            }

            return false;
        }

        private static bool AppendIdenticalEvents(this StringBuilder sb, int index, object @event)
        {
            sb.AppendLine($"[{index}] {@event.GetType().Name}");
            return false;
        }

        private static bool AppendExtraEvents(this StringBuilder sb, IEnumerable<object> events, int offset, int skip, string prefix)
        {
            var index = offset + skip;

            var extra = false;

            foreach (var @event in events.Skip(skip))
            {
                sb.AppendLine($"[{index}] {prefix} {@event.GetType().Name}");
                index++;
                extra = true;
            }

            return extra;
        }

        private static bool AppendExpectedActual(this StringBuilder sb, int index, object expected, object actual)
        {
            var prefix = $"[{index}] ";
            var empty = new string(' ', prefix.Length);

            sb.Append(prefix + "Expected: ");
            sb.AppendLine(expected.ToString());
            sb.AppendLine($"{empty}Actual:   {actual}");

            return true;
        }
    }
}
