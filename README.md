![Qowaiv](https://github.com/Qowaiv/Qowaiv/blob/master/design/qowaiv-logo_linkedin_100x060.jpg)

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![Code of Conduct](https://img.shields.io/badge/%E2%9D%A4-code%20of%20conduct-blue.svg?style=flat)](https://github.com/Qowaiv/qowaiv-domainmodel/blob/master/CODE_OF_CONDUCT.md)

| version                                                                   | package                                                                |
|---------------------------------------------------------------------------|------------------------------------------------------------------------|
|![v](https://img.shields.io/badge/version-0.0.1-blue.svg?cacheSeconds=3600)|[Qowaiv.DomainModel](https://www.nuget.org/packages/Qowaiv.DomainModel/)|

# Qowaiv Domain Model
Qowaiv Domain Model is library containing the (abstract) building blocks to set
up a Domain-Driven application.

## Event Sourcing
The choice have been made to only support DDD via Event Sourcing. In short:
Event Sourcing describes the state of an aggregate (root) by the (domain) events
that occurred to it, and getting the current state of affairs can always be
achieved by replaying these events.

## Always Valid
Aggregate roots should always be valid; that is, within the boundaries of their
domain. There are multiple ways to achieve this, but within Qowaiv Domain Model
the choice have been made to this via implicitly triggered validators. When
a public method is called that could/should lead to a new state the event(s)
describing the change are only added to the event buffer/stream, if the new
state is valid according to the validator.

## Aggregate Root
An Aggregate Root is the root (duh), of every Domain with the DDD context. 
It should be provided with an [IValidator<TAggragate>](https://github.com/Qowaiv/qowaiv-validation)
implementation of choice.

Furthermore there is an option to have a base where identity and the handling of
the events that should be published (as they do not lead to an invalid state):
`AggregateRoot<TAggregate>`. It has an `protected abstract void AddEventsToBuffer(params object[] events)`
method that has to be overridden, to achieve the persistence of the events.

The other option is `AggregateRoot<TAggregate, TId>` that comes with an
`EventBuffer<TId>`.  Events that should be persisted are added to it, and it
can return both the committed as the uncommitted events it buffers.

## Example
A (simplified) real life example of a financial entry:
``` C#
public sealed class FinancialEntry : AggregateRoot<FinancialEntry, Guid>
{
    public FinancialEntry(Guid id) : base(id, new FinancialEntryValidator()) { }

     public IReadOnlyCollection<EntryLine> Lines => enties;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<EntryLine> enties = new List<EntryLine>();

    public Result<FinancialEntry> AddLines(params FinancialEntryLine[] lines)
    {
        return ApplyEvents(lines.Select(line => new EntryLineAdded
        {
            GlAccount = line.GlAccount,
            Amount = line.Amount,
            Date = line.Date,
            Description = line.Description,
            AccountId = line.AccountId,
        }).ToArray());
    }
	
    // The method that is triggered when an EntryLineAdded
    // is triggered by the ApplyEvent(s) method.
    internal void When(EntryLineAdded @event)
    {
        enties.Add(new EntryLine
        {
            GlAccount = @event.GlAccount,
            Date = @event.Date,
            Amount = @event.Amount,
            Description = @event.Description,
            AccountId = @event.AccountId,
        });
    }
}
```

And a validator might look like this:

``` C#
public class FinancialEntryValidator : FluentModelValidator<FinancialEntry>
{
    public FinancialEntryValidator()
    {
        RuleFor(entry => entry.Lines).NotEmpty().Custom(BeBalanced);
        RuleFor(entry => entry.Report).NotEmpty();
        RuleForEach(entry => entry.Lines).SetValidator(new EntryLineValidator());
    }

    private void BeBalanced(IReadOnlyCollection<EntryLine> lines, CustomContext context)
    {
        if (lines.Select(line => line.Amount.Currency).Distinct().Count() > 1)
        {
            context.AddFailure(nameof(FinancialEntry.Lines), "Multiple currencies.");
            // return as we can not sum the amounts.
            return;
        }
        var sum = lines.Select(line => line.Amount).Sum();
        if (sum.Currency.IsEmptyOrUnknown())
        {
            context.AddFailure(nameof(FinancialEntry.Lines), "Unknown currency.");
        }

        if (sum.Amount != Amount.Zero)
        {
            context.AddFailure(nameof(FinancialEntry.Lines), "The lines are note balanced.");
        }
    }
}
```

You could argue that some of the rules specified in the validator should be handled
as part of the *anti-corruption layer*, but that is another topic. The point is
that by defining those constraints, you can no longer add any entry line with
an other currency then the lines already added, nor have an financial entry that
is not balanced, both extremely important in this domain.

If you want to throw an exception, or deal with a `Result<TAggegate>` is up to
the developer.

## Event Collection
When applying changes to an aggregate, based on it current states you might
want to apply different events; sometimes even a different amount of a different
type. This is supported the following way:

``` C#
public Result<Game> Attack(Country attacker, Country defender, Result result)
=> Apply(Events
    .If(Result.IsSuccess)
        .Then(() => new Conquered
        {
            From = attacker,
            To = defender,
            Armies = result.Attacker,
        })
    .Else(() => new Attacked
    {
        Attacker = attacker,
        Defender = defender,
        Result = result,
    })
    .If(Result.IsSuccess && Countries(defender).Single())
        .Then(() => new PlayerEliminated 
        {
            Player = Coumtries(defender).Owner 
        }));

```
