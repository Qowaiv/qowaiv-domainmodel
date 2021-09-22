using FluentAssertions;
using NUnit.Framework;
using Qowaiv.DomainModel;
using Qowaiv.Validation.Abstractions;
using System;

namespace Must_specs
{
    public class Requires
    {
        [Test]
        public void Subject_not_to_be_null()
        {
            Func<object> create = () => new Must<TestModel>(null);
            create.Should().Throw<ArgumentNullException>();
        }
    }

    public class Guards_when
    {
        [Test]
        public void Must_Be_condition_is_met()
        {
            var model = new TestModel();
            new Must<TestModel>(model).Be(condition: true, ValidationMessage.Error("This is wrong"))
                .Should().BeValid().WithoutMessages()
                .Value.Should().BeSameAs(model);
        }

        [Test]
        public void Must_NotBe_condition_is_not_met()
        {
            var model = new TestModel();
            new Must<TestModel>(model).NotBe(condition: false, "This is wrong")
                .Should().BeValid().WithoutMessages()
                .Value.Should().BeSameAs(model);
        }

        [Test]
        public void Must_Exist_resolves_entity()
        {
            var model = new TestModel();
            new Must<TestModel>(model).Exist(8, (m, id) => new object())
                .Should().BeValid().WithoutMessages()
                .Value.Should().BeSameAs(model);
        }
    }

    public class Yields_when
    {
        [Test]
        public void Must_Be_condition_is_not_met()
            => new Must<TestModel>(new TestModel()).Be(condition: false, ValidationMessage.Error("This is wrong"))
            .Should().BeInvalid().WithMessage(ValidationMessage.Error("This is wrong"));

        [Test]
        public void Must_NotBe_condition_is_met()
            => new Must<TestModel>(new TestModel()).NotBe(condition: true, "This is wrong")
            .Should().BeInvalid().WithMessage(ValidationMessage.Error("This is wrong"));

        [Test]
        public void Must_Exist_does_not_resolve_entity()
          => new Must<TestModel>(new TestModel()).Exist(666, (m, id) => (object)null)
            .Should().BeInvalid().WithMessage(ValidationMessage.Error("Entity with ID 666 could not be found."));
    }

    public class ToString_as_type
    {
        [Test]
        public void Displays_type_based_on_TSubject()
            => new Must<object>(new TestModel()).ToString().Should().Be("Must<System.Object>");
    }

    internal class TestModel { }
}
