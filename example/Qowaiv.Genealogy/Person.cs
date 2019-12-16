using Qowaiv.DomainModel.EventSourcing;
using Qowaiv.Genealogy.Commands;
using Qowaiv.Genealogy.Events;
using Qowaiv.Genealogy.Mapping;
using Qowaiv.Validation.Abstractions;

namespace Qowaiv.Genealogy
{
    public class Person : AggregateRoot<Person>
    {
        private static readonly PersonValidator _validator = new PersonValidator();

        public Person() : base(_validator) { }

        public Gender Gender { get; private set; }

        public string PersonalName { get; private set; }

        public string FamilyName { get; private set; }

        public Date DateOfBirth { get; private set; }

        public Date? DateOfDeath { get; private set; }

        public EmailAddress Email { get; private set; }

        public Result<Person> Update(UpdatePerson cmd)
        {
            Guard.NotNull(cmd, nameof(cmd));

            return ApplyEvent(cmd.Map<PersonUpdated>());
        }

        internal void Apply(PersonUpdated e)
        {
            Gender = e.Gender;
            PersonalName = e.PersonalName;
            FamilyName = e.FamilyName;
            DateOfBirth = e.DateOfBirth;
            DateOfDeath = e.DateOfDeath;
            Email = e.Email;
        }

        public static Result<Person> Create(CreatePerson cmd)
        {
            Guard.NotNull(cmd, nameof(cmd));

            var person = new Person();
            var updated = cmd.Map<PersonUpdated>();
            return person.ApplyEvents(new PersonCreated(), updated);
        }
    }
}
