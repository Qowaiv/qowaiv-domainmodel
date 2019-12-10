#pragma warning disable S4035
// Classes implementing "IEquatable<T>" should be sealed
// The Implementation takes types into account, and uses an equality comparer.

using Qowaiv.DomainModel.Tracking;
using Qowaiv.Validation.Abstractions;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Qowaiv.DomainModel
{
    /// <summary>Represents an (domain-driven design) entity.</summary>
    /// <typeparam name="TEntity">
    /// The type of the entity itself.
    /// </typeparam>
    [DebuggerDisplay("{DebuggerDisplay}")]
    public abstract class Entity<TEntity> : IEntity<TEntity>
        where TEntity : Entity<TEntity>
    {
        private readonly PropertyCollection properties;

        /// <summary>Initializes a new instance of the <see cref="Entity{Tentity}"/> class.</summary>
        /// <param name="tracker">
        /// The change tracker of the entity.
        /// </param>
        protected Entity(ChangeTracker tracker) : this(Guid.NewGuid(), tracker) { }

        /// <summary>Initializes a new instance of the <see cref="Entity{TEntity}"/> class.</summary>
        /// <param name="id">
        /// The identifier of the entity.
        /// </param>
        /// <param name="tracker">
        /// The change tracker of the entity.
        /// </param>
        protected Entity(Guid id, ChangeTracker tracker)
        {
            Id = Guard.NotEmpty(id, nameof(id));
            properties = PropertyCollection.Create(GetType());
            Tracker = Guard.NotNull(tracker, nameof(tracker));
        }

        /// <inheritdoc />
        public virtual Guid Id { get; }

        /// <summary>Gets the change tracker.</summary>
        protected virtual ChangeTracker Tracker { get; }

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(obj as TEntity);

        /// <inheritdoc />
        public bool Equals(TEntity other) => Comparer.Equals((TEntity)this, other);

        /// <inheritdoc />
        public override int GetHashCode() => Comparer.GetHashCode((TEntity)this);

        /// <summary>Returns true if left and right are equal.</summary>
        public static bool operator ==(Entity<TEntity> left, Entity<TEntity> right) => Comparer.Equals((TEntity)left, (TEntity)right);

        /// <summary>Returns false if left and right are equal.</summary>
        public static bool operator !=(Entity<TEntity> left, Entity<TEntity> right) => !(left == right);

        /// <summary>Gets a property (value).</summary>
        /// <param name="propertyName">
        /// The name of the property (should not be set, but retrieved by <see cref="CallerMemberNameAttribute"/>).
        /// </param>
        /// <typeparam name="T">
        /// The type of the property to set.
        /// </typeparam>
        protected T GetProperty<T>([CallerMemberName]string propertyName = null)
        {
            return (T)properties[propertyName];
        }

        /// <summary>Sets a property (value).</summary>
        /// <param name="value">
        /// The value to assign.
        /// </param>
        /// <param name="propertyName">
        /// The name of the property (should not be set, but retrieved by <see cref="CallerMemberNameAttribute"/>).
        /// </param>
        /// <typeparam name="T">
        /// The type of the property to set.
        /// </typeparam>
        /// <exception cref="InvalidModelException">
        /// If the new value violates the property constraints.
        /// </exception>
        protected void SetProperty<T>(T value, [CallerMemberName]string propertyName = null)
        {
            Tracker.Add(new PropertyChanged(properties, propertyName, value));
        }

        /// <summary>Guards the entity.</summary>
        /// <param name="guards">
        /// The validation messages that are the result of the guarding.
        /// </param>
        /// <returns>
        /// A <see cref="Result{TEntity}"/> that contains the entity or the
        /// error messages.
        /// </returns>
        protected Result<TEntity> Guards(params IValidationMessage[] guards)
        {
            Guard.NotNull(guards, nameof(guards));
            return Result.For((TEntity)this, guards);
        }

        /// <summary>Represents the entity as a DEBUG <see cref="string"/>.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{GetType().Name}, ID: {Id:B}";

        /// <summary>The comparer that deals with equals and hash codes.</summary>
        private static readonly EntityEqualityComparer<TEntity> Comparer = new EntityEqualityComparer<TEntity>();
    }
}
