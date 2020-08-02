using FluentValidation;
using Qowaiv.Validation.Abstractions;
using Qowaiv.Validation.Fluent;
using System;

namespace Qowaiv.DomainModel.UnitTests.Models
{
    public sealed class SimpleEventSourcedRoot : AggregateRoot<SimpleEventSourcedRoot, Guid>
    {
        public SimpleEventSourcedRoot() : base(Guid.NewGuid(), new SimpleEventSourcedRootValidator()) { }

        public bool Initialized { get; private set; }

        public string Name { get; private set; }

        public Date DateOfBirth { get; private set; }

        public bool IsWrong { get; private set; }

        public Result<SimpleEventSourcedRoot> SetName(string name) => ApplyEvent(new NameUpdated { Name = name });

        public Result<SimpleEventSourcedRoot> SetPerson(string name, Date dateOfBirth)
        {
            return ApplyEvents(
                new NameUpdated { Name = name },
                new DateOfBirthUpdated { DateOfBirth = dateOfBirth });
        }

        internal void When(NameUpdated @event)
        {
            Name = @event.Name;
        }

        internal void When(DateOfBirthUpdated @event)
        {
            DateOfBirth = @event.DateOfBirth;
        }

        internal void When(SimpleInitEvent @event)
        {
            Initialized = @event != null;
        }

        internal void When(InvalidEvent @event)
        {
            IsWrong = @event != null;
        }
    }

    public class SimpleEventSourcedRootValidator : FluentModelValidator<SimpleEventSourcedRoot>
    {
        public SimpleEventSourcedRootValidator()
        {
            RuleFor(m => m.IsWrong).Must(prop => !prop).WithMessage("Should not be wrong.");
        }
    }

    public class NameUpdated
    {
        public string Name { get; set; }
    }

    public class DateOfBirthUpdated
    {
        public Date DateOfBirth { get; set; }
    }

    public class InvalidEvent { }


    public class SimpleInitEvent { }
}
