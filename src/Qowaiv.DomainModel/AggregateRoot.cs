using Qowaiv.DomainModel.Tracking;
using Qowaiv.Validation.Abstractions;
using System;

namespace Qowaiv.DomainModel
{
    /// <summary>Represents an (domain-driven design) aggregate root.</summary>
    /// <typeparam name="TAggrgate">
    /// The type of the aggregate root itself.
    /// </typeparam>
    public abstract class AggregateRoot<TAggrgate> : Entity<TAggrgate>
        where TAggrgate : AggregateRoot<TAggrgate>
    {
        /// <summary>Initializes a new instance of the <see cref="AggregateRoot{TAggrgate}"/> class.</summary>
        /// <param name="validator">
        /// A custom validator.
        /// </param>
        protected AggregateRoot(IValidator<TAggrgate> validator)
            : this(Guid.NewGuid(), validator) { }

        /// <summary>Initializes a new instance of the <see cref="AggregateRoot{TAggrgate}"/> class.</summary>
        /// <param name="id">
        /// The identifier of the aggregate root.
        /// </param>
        /// <param name="validator">
        /// A custom validator.
        /// </param>
        protected AggregateRoot(Guid id, IValidator<TAggrgate> validator)
            : base(id, new ChangeTracker<TAggrgate>())
        {
            Tracker.Init((TAggrgate)this, validator);
        }

        /// <summary>Initializes multiple properties simultaneously, without triggering validation.</summary>
        /// <param name="initializeAction">
        /// The action trying to initialize the state of the properties.
        /// </param>
        /// <remarks>
        /// Should only be called via the constructor.
        /// </remarks>
        protected void Initialize(Action initializeAction)
        {
            Guard.NotNull(initializeAction, nameof(initializeAction));

            Tracker.Intialize();
            initializeAction();
            Tracker.NoBuffering();
        }

        /// <summary>Sets multiple properties simultaneously.</summary>
        /// <param name="action">
        /// The action trying to update the state of the properties.
        /// </param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the entity or the messages.
        /// </returns>
        protected Result<TAggrgate> Update(Action<TAggrgate> action)
        {
            Guard.NotNull(action, nameof(action));

            Tracker.BufferChanges();
            action((TAggrgate)this);
            return Tracker.Process();
        }

        /// <summary>Gets the change tracker.</summary>
        protected new ChangeTracker<TAggrgate> Tracker => (ChangeTracker<TAggrgate>)base.Tracker;
    }
}
