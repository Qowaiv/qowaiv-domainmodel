using Qowaiv.DomainModel;
using Qowaiv.Validation.Abstractions;
using Qowaiv.Validation.Messages;

namespace Qowaiv.Validation.Guarding
{
    /// <summary>Extensions on <see cref="Must{TSubject}"/>.</summary>
    public static class DomainModelExtensions
    {
        /// <summary>Guards the <see cref="AggregateRoot{TAggregate, TId}"/> to have the expected version;
        /// otherwise return a <see cref="ConcurrencyIssue"/>.
        /// </summary>
        /// <typeparam name="TAggregate">
        /// The type of the aggregate.
        /// </typeparam>
        /// <param name="must">
        /// The must to extend on.
        /// </param>
        /// <param name="expected">
        /// The expected version.
        /// </param>
        /// <remarks>
        /// A dynamic is used, as the TId generic can not be resolved by usage.
        /// It would be inconvenient to have specify the generics on this
        /// extension.
        /// </remarks>
        public static Result<TAggregate> HaveVersion<TAggregate>(this Must<TAggregate> must, int expected)
            where TAggregate : AggregateRoot<TAggregate>, new()
        {
            Guard.NotNull(must, nameof(must));
            Guard.NotNegative(expected, nameof(expected));
            int actual = ((dynamic)must.Subject).Version;
            return must.Be(actual == expected, ConcurrencyIssue.VersionMismatch(expected, actual)); 
        }
    }
}
