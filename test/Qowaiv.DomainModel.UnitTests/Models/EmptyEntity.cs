using Qowaiv.Validation.Abstractions;
using System;

namespace Qowaiv.DomainModel.UnitTests.Models
{
    public sealed class EmptyEntity : AggregateRoot<EmptyEntity>
    {
        public EmptyEntity(): this(Guid.NewGuid()) { }

        public EmptyEntity(Guid id) : base(id, Validator.Empty<EmptyEntity>()) { }
    }
}
