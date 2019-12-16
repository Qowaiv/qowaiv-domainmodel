using FluentValidation;
using Qowaiv.DomainModel.EventSourcing;
using Qowaiv.Validation.Abstractions;
using Qowaiv.Validation.Fluent;

namespace Qowaiv.DomainModel.UnitTests.Models
{
    public sealed class SimpleEventSourcedRoot : AggregateRoot<SimpleEventSourcedRoot>
    {
        public SimpleEventSourcedRoot() : base(new SimpleEventSourcedRootValidator()) { }

        public bool Initialized { get; private set; }

        public string Name { get; private set; }

        public Date DateOfBirth { get; private set; }

        public bool IsWrong { get; private set; }

        public Result<SimpleEventSourcedRoot> SetName(string name) => ApplyEvent(new NameUpdated { Name = name  });

        public Result<SimpleEventSourcedRoot> SetPerson(string name, Date dateOfBirth)
        {
            return ApplyEvents(
                new NameUpdated { Name = name },
                new DateOfBirthUpdated { DateOfBirth = dateOfBirth });
        }

        internal void Apply(NameUpdated @event)
        {
            Name = @event.Name;
        }

        internal void Apply(DateOfBirthUpdated @event)
        {
            DateOfBirth = @event.DateOfBirth;
        }

        internal void Apply(SimpleInitEvent @event)
        {
            Initialized = true;
        }

        internal void Apply(InvalidEvent @event)
        {
            IsWrong = true;
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
