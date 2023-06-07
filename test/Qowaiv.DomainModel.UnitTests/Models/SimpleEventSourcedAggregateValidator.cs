using FluentValidation;
using Qowaiv.Validation.Fluent;

namespace Qowaiv.DomainModel.UnitTests.Models;

public class SimpleEventSourcedAggregateValidator : ModelValidator<SimpleEventSourcedAggregate>
{
    public SimpleEventSourcedAggregateValidator()
    {
        RuleFor(m => m.IsWrong).Must(prop => !prop).WithMessage("Should not be wrong.");
    }
}
