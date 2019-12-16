using FluentValidation;
using Qowaiv.DomainModel.EventSourcing;
using Qowaiv.Validation.Abstractions;
using Qowaiv.Validation.Fluent;

namespace Qowaiv.DomainModel.UnitTests.Models
{
    public sealed class SimpleEventSourcedRoot : EventSourcedAggregateRoot<SimpleEventSourcedRoot>
    {
        public SimpleEventSourcedRoot() : base(new SimpleEventSourcedRootValidator()) { }

        public bool Initialized
        {
            get => GetProperty<bool>();
            private set => SetProperty(value);
        }

        public string Name
        {
            get => GetProperty<string>();
            private set => SetProperty(value);
        }

        public bool IsWrong
        {
            get => GetProperty<bool>();
            internal set => SetProperty(value);
        }

        public Result<SimpleEventSourcedRoot> SetName(UpdateNameEvent command) => ApplyEvent(command);

        internal void Apply(UpdateNameEvent @event)
        {
            Name = @event.Name;
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

    public class UpdateNameEvent
    {
        public string Name { get; set; }
    }

    public class InvalidEvent { }


    public class SimpleInitEvent { }
}
