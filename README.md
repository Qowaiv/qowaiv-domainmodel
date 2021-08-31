![Qowaiv](https://github.com/Qowaiv/Qowaiv/blob/master/design/qowaiv-logo_linkedin_100x060.jpg)

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![Code of Conduct](https://img.shields.io/badge/%E2%9D%A4-code%20of%20conduct-blue.svg?style=flat)](https://github.com/Qowaiv/qowaiv-domainmodel/blob/master/CODE_OF_CONDUCT.md)

| version                                                                      | package                                                                                    |
|------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------|
|![v](https://img.shields.io/badge/version-0.1.1-blue.svg?cacheSeconds=3600)   |[Qowaiv.DomainModel](https://www.nuget.org/packages/Qowaiv.DomainModel/)                    |
|![v](https://img.shields.io/badge/version-0.1.0-darkred.svg?cacheSeconds=3600)|[Qowaiv.Do,ainModel.TestTools](https://www.nuget.org/packages/Qowaiv.DomainModel.TestTools/)|

# Qowaiv Domain Model
Qowaiv Domain Model is a library containing the (abstract) building blocks to
set up a Domain-Driven application.

## Event Sourcing
Within Qowaiv Domain Model, the choice has been made to only support DDD via
Event Sourcing. In short: Event Sourcing describes the state of an aggregate
(root) by the (domain) events that occurred within the domain. Getting the
current state of an aggregate root can always be achieved by replaying these
events. 

## Always Valid
Aggregate roots should always be valid according to the boundaries of their
domain. There are multiple ways to achieve this, but within Qowaiv Domain Model
this is guaranteed via implicitly triggered validators.
When a public method is called that would lead to a new aggregate state, the
events describing the change are only added to the event buffer
associated with the aggregate root if the new state is valid according to the
rules specified in the validator.

## Aggregate Root
An Aggregate Root is the root of every Domain within a DDD context. When
implementing a new aggregate root there are several steps that have to be taken.

First, a new Aggregate Root should have a corresponding
[IValidator<TAggragate>](https://github.com/Qowaiv/qowaiv-validation)
implementation of choice. This implementation will safeguard any post conditions
on the aggregate root.

Secondly, the actual class representing the aggregate root should be created. This class
should inherit from one of two possible base classes:

- `AggregateRoot<TAggregate>`
- `AggregateRoot<TAggregate, TId>`

`AggregateRoot<TAggregate>` is a low level base class that provides a framework
for handling the application of events. It has no event store of its own. Implementations
of this class should override the `protected abstract void AddEventsToBuffer(params object[] events)`
method to achieve persistence of events.

The second option, `AggregateRoot<TAggregate, TId>`, inherits from
`AggregateRoot<TAggregate>`. It has built-in identity support and systems for
storing events in the integrated `EventBuffer<TId>`. Events that should be
persisted are added to this buffer, and it can return both the committed as
well as the uncommitted events it contains.

### Immutability
As immutability comes with tons of benefits in DDD scenarios, the
`AggregateRoot<TAggregate>` is designed to be immutable; that is,
if you apply all your changes via the `Apply`, and `ApplyEvents` methods
(as you should), it will create an updated copy that represents the new state,
leaving the initial instance unchanged.

## Example
A (simplified) real life example of a financial entry, using `AggregateRoot<TAggregate, TId>`:
``` C#
public sealed class FinancialEntry : AggregateRoot<FinancialEntry, Guid>
{
    // Note that FinancialEntryValidator is passed in for validation purposes.
    // See the implementation of this class below.
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
    // is processed by the ApplyEvents method called from AddLines above.
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

And the implementation of the validator for `FinancialEntry` might look like this:

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

You could argue that some of the rules specified in the validator should be
handled as part of the *anti-corruption layer*, but that is another topic. The
point is that by defining those constraints, you can no longer add any entry
line with any other currency than the lines already added, nor add any financial
entries that are not balanced. Both extremely important within this particular
domain.

Note that the decision to throw an exception, or deal with a
`Result<TAggegate>` is up to the developer.

## Immutable Collection
When applying changes to an aggregate, you might want to apply a different type
and number of events, based on the current state of the aggregate. This use-case
is supported in the following way:

``` C#
public Result<Game> Attack(Country attacker, Country defender, AttackResult result)
=> Apply(Events
    .If(result.IsSuccess)
        .Then(() => new Conquered
        {
            From = result.Attacker,
            To = result.Defender,
            Armies = result.Attacker,
        })
    .Else(() => new Attacked
    {
        Attacker = attacker,
        Defender = defender,
        Result = result,
    })
    .If(result.IsSuccess && Countries(defender).Single())
        .Then(() => new PlayerEliminated 
        {
            Player = Countries(defender).Owner 
        }));
```

## Event Buffer
The `EventBuffer<TId>`, as used by `AggregateRoot<TAggregate, TId>` is an
immutable collection with the following API:

``` C#
// Creation
var id = NewId();
var buffer = EventBuffer.Empty(id, version: 5); // version optional.
var stored = EventBuffer.FromStorage(id, version: 5, storedEvents, (e) => Convert(e));

// Extending, returning a new instance.
var updated = buffer.Add(events); // excepts arrays, enumerables or a single event.

// Export for storage
var export = buffer.SelectUncommitted((id, verion, e) => Export(id, version, e));

// After successful export
var updated = buffer.MarkAllAsCommitted();
```

