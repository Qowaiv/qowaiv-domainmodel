using FluentAssertions;
using NUnit.Framework;
using Qowaiv.DomainModel.Commands;
using Qowaiv.Validation.Abstractions;
using System;
using System.Threading;

namespace Commands.CommandProcessor_on_cancellation_specs
{
    public class Sends
    {
        [Test]
        public void a_command_with_a_non_cancellable_resulttype__and_a_non_cancellable_cancellationtoken()
        {
            var processor = new NumberProcessor(new NumberHandler());
            processor.Send(new One(), token: default).Value.Should().Be(1);
            
        }
    }

    public class Throws
    {
        [Test]
        public void for_non_cancellable_resulttypes_if_cancellationtoken_has_been_provided()
        {
            Action send = () => new NumberProcessor(new NumberHandler()).Send(new One(), token: new CancellationTokenSource().Token);
            send.Should().Throw<InvalidOperationException>()
                .WithMessage("A CancellationToken was provided, but the result is not awaitable (Task<>) and thus not cancellable");
        }
    }

    class NumberProcessor : CommandProcessor<Result<int>>
    {
        private readonly object handler;
        public NumberProcessor(object handler) => this.handler = handler;
        protected override Type GenericHandlerType => typeof(CommandHandler<>);
        protected override string HandlerMethod => nameof(CommandHandler<object>.Handle);
        protected override object GetHandler(Type handlerType) => handler;
    }

    interface CommandHandler<TCommand>
    {
        Result<int> Handle(TCommand command, CancellationToken token);
    }

    record One();

    record Two();

    class NumberHandler :
        CommandHandler<One>,
        CommandHandler<Two>
    {
        public Result<int> Handle(One command, CancellationToken token) => Result.For(1);
        public Result<int> Handle(Two command, CancellationToken token) => Result.For(2);
    }
}
