using FluentAssertions;
using NUnit.Framework;
using Qowaiv.DomainModel.Commands;
using Qowaiv.Validation.Abstractions;
using System;
using System.Threading.Tasks;

namespace Commands.CommandProcessor_specs
{
    public class Sends
    {
        [Test]
        public async Task a_command()
        {
            var processor = new NumberProcessor(new NumberHandler());
            var response = await processor.Send(new One());
            response.Value.Should().Be(1);
        }

        [Test]
        public async Task a_second_command_using_precompiled_func()
        {
            var processor = new NumberProcessor(new NumberHandler());
            var first = await processor.Send(new Two());
            var second = await processor.Send(new Two());
            first.Value.Should().Be(2);
            second.Value.Should().Be(2);
        }
    }

    public class Throws
    {
        [Test]
        public void for_null_commands()
        {
            Action send = () => new NumberProcessor(new NumberHandler()).Send(null);
            send.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void for_not_registered_commands()
        {
            Action send = () => new NumberProcessor(null).Send(new One());
            send.Should().Throw<UnresolvedCommandHandler>()
                .WithMessage("The command hander Commands.CommandProcessor_specs.CommandHandler`1[Commands.CommandProcessor_specs.One] could not be resolved.");
        }
    }

    class NumberProcessor : CommandProcessor<Task<Result<int>>>
    {
        private readonly object handler;
        public NumberProcessor(object handler) => this.handler = handler;
        protected override Type GenericHandlerType => typeof(CommandHandler<>);
        protected override string HandlerMethod => nameof(CommandHandler<object>.Handle);
        protected override object GetHandler(Type handlerType) => handler;
    }

    interface CommandHandler<TCommand>
    {
        Task<Result<int>> Handle(TCommand command);
    }
    record One();
    record Two();
    class NumberHandler :
        CommandHandler<One>,
        CommandHandler<Two>
    {
        public Task<Result<int>> Handle(One command) => Result.For(1).AsTask();
        public Task<Result<int>> Handle(Two command) => Result.For(2).AsTask();
    }
}
