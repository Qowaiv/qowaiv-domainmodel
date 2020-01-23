![Qowaiv](https://github.com/Qowaiv/Qowaiv/blob/master/design/qowaiv-logo_linkedin_100x060.jpg)

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![Code of Conduct](https://img.shields.io/badge/%E2%9D%A4-code%20of%20conduct-blue.svg?style=flat)](https://github.com/Qowaiv/qowaiv-domainmodel/blob/master/CODE_OF_CONDUCT.md)

| version                                                                   | package                                                                                            |
|---------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------|
|![v](https://img.shields.io/badge/version-0.0.0-blue.svg?cacheSeconds=3600)|[Qowaiv.DomainModel](https://www.nuget.org/packages/Qowaiv.DomainModel/)                            |                         |
|![v](https://img.shields.io/badge/version-0.0.0-blue.svg?cacheSeconds=3600)|[Qowaiv.DomainModel](https://www.nuget.org/packages/Qowaiv.DomainModel/)|                      |

# Qowaiv Domain Model
Qowaiv Domain Model is library containing the (abstract) building blocks to set
up a Domain-Driven application. The starting-point of the modeled entities is
that the concept of _Always Valid_ is achieved by implicitly triggered
validation; if changes would lead to an invalid state, they are rolled back.

## Implicit Validation
To prevent an Aggregate Root from having an invalid state, a `ChangeTracker`
is shared with all its entities. The `SetProperty<T>(T value, string)` method
is used to set properties.

When multiple properties should be updated at once, The `TrackChanges()` method
keeps track of those changes for the scope of the method. `The Result<T>`
contains the errors if any, and the changes are rolled back.

``` C#
var aggregate = new MyAggregate();
Result<MyAggregate> result = aggregate.TrackChanges((m) =>
{
	m.Prop1 = invalidValue;
	m.Prop2 = alsoInvalid;
});
```

When an aggregate is updated without explicitly tracking changes, every update
triggers a validation, and on a failure follows a roll back, and an exception
that describes the validation error.

``` C#
var aggregate = new MyAggregate();
aggregate.Prop = invalidValue; // Throws
```

## Aggregate Root
An Aggregate Root is the root (duh), of every Domain with the DDD context. 
It has een imutable ID (Guid.Empty is not allowed), and should be provided 
with an [IValidator<TAggragate>](https://github.com/Qowaiv/qowaiv-validation)
implemetation of choice.

``` C#
public sealed class Family : AggregateRoot<Family>
{
    public Family(Guid id): base(id, new FamilyValidator()) 
    { 
	    // A build-in collection type that support tracking changes.
        Members = new ChildCollection<FamilyMember>(Tracker);
    }

    public ChildCollection<FamilyMember> Members { get; }

    // Adding a child entity returning a Result<Family> potentially 
    // containing (multiple) errors. This is the advised way.
    public Result<Family> AddFamilyMember(Date dateOfBirth, decimal length)
    {
        return TrackChanges((m) =>
        {
            m.Members.Add(
            new Person(m.Tracker) 
            {
                DateOfBirth = dateOfBirth,
                Length = length,
            });
        });
    }
	
	 // Adding a child entity that throws an InvalidOperationException.
    public void AddMember1(Date dateOfBirth, decimal length)
    {
        TrackChanges((m) =>
        {
            m.Members.Add(
            new FamilyMember(m.Tracker) 
            {
                DateOfBirth = dateOfBirth,
                Length = length,
            });
        }).ThrowIfInvalid();
    }
}

public sealed class FamilyMember : Entity<FamilyMember>
{
    // Only constructed via the Aggregate Root.
    internal FamilyMember(ChangeTracker tracker) : base(tracker) { }

    // Only set via the Aggregate Root.
    public string FullName
    {
        // Gets the value from the property collection.
        get => GetProperty<string>();
        // Registers the change to th change tracker.
        internal set => SetProperty(value);
    }

    public Date DateOfBirth
    {
        get => GetProperty<Date>();
        internal set => SetProperty(value);
    }
    public decimal Length
    {
        get => GetProperty<Date>();
        internal set => SetProperty(value);
    }
}
```

##

And a validator might look like this:

``` C#
public FamilyValidator : FluentModelValidator<Family>
{
    public FamilyValidator()
    {
        RuleFor(m => m.Members).NotEmpty();
        RuleForEach(m => m.Members).ChildRules(member => 
        {
            member.RuleFor(m => m.DateOfBirth)
                .NotEmpty();
            member.RuleFor(m => m.Length)
                .Must(l => l > 0.5)
                .WithMessage("Short people got no reason to live");
        });
    }
}
```

You could argue that some of the rules specified in the validator should be handled
as part of the *anti-corruption layer*, but that is another topic. The point is
that by defining those constraints, you can no longer add any item to `Family.Members`
that is short than 50 cms, or has `default` as `DateOfBirth`, nor could you remove
a member if `Family.Members` contains a single item.

If you want to throw an exception, or deal with a `Result<TAggegate>` is up to
the developer.

### Event Sourcing
Event Sourcing (often as part of Command and Query Responsibility Segregation aka CQRS)
is a concept in which changes on the domain are described (and communicated) as
messages/events. Qowaiv has support for such scenarios.

``` C#
public sealed class Family : EventSourcedAggregateRoot<Family>
{
    public Family(): base(new FamilyValidator()) 
    { 
        Members = new ChildCollection<Person>(Tracker);
    }

    public ChildCollection<Person> Members { get; }

    public Result<Family> AddMember(Date dateOfBirth, decimal length)
    {
        // Works on a name based convention
        return ApplyChange(new FamilyMemberAdded { DateOfBirth = dateOfBirth, Length = Length });
    }
    
    // Matches the name based convention.
    private void Apply(FamilyMemberAdded @event)
    {
        Members.Add(
        new Person(m.Tracker) 
        {
            DateOfBirth = @event.DateOfBirth,
            Length = @event.Length,
        });
    }
}
```

Interacting with an Event Source based aggregate root is commonly done via a
command handler. Whatever mechanism (and with which technology) you choice,
saving the new events and publishing them, is done here.

``` C#
public class CommandHandler
{
    public Result<Family> Handle(AddFamilyMember cmd)
    {
        var family = repository.ById(cmd.Id);
        var result = family.AddMember(cmd.DateOfBirth, cmd.Length);
        repository.Save(family);
        eventbus.Publish(family.EventStream.GetUncommitted());        
        family.EventStream.MarkAllAsCommitted();
        return result;
    }
}
```

The main idea is that if you should have an `Apply(EventyType @event)`
method that changes the state based on the data in the event. This one is called
via `ApplyChange(object @event)`. If applying the event to the aggregate 
lead to an invalid state, the changes are not applied, and the event is not
added to the event stream.

## Complex Value Objects
Complex Value Objects - opposed to [Single Value Objects (SVO's)](https://github.com/Qowaiv/Qowaiv),
those who can be represented by a single scalar - are represented by multiple (immutable)
scalars. Qowaiv provides a generic base class helping to achieve that.

``` C#
// Should be sealed; shared logic with other Value Object is discouraged.
public sealed class Address : ValueObject<Address>
{
    public Address(string street, HouseNumber number, PostalCode code, Country country)
    {
        Street = street;
        HouseNumber = number;
        PostalCode = code;
        Country = country;
    }

    public string Street { get; }
    public HouseNumber HouseNumber { get; }
    public PostalCode PostalCode { get; }
    public Country Country { get; }

    public override bool Equals(AddressValueObject other)
    {
        // If we're dealing with the same instance.
        if (AreSame(other)) { return true; }
		
		// Not null and all properties are equal.
        return NotNull(other)
            && Street == other.Street
            && HouseNumber == other.HouseNumber
            && PostalCode == other.PostalCode
            && Country == other.Country;
    }

    protected override int Hash()
    {
        return QowaivHash.HashObject(Street)
            ^ QowaivHash.Hash(HouseNumber, 3)
            ^ QowaivHash.Hash(PostalCode, 5)
            ^ QowaivHash.Hash(Country, 13);
    }   
}

```
