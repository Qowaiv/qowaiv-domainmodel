namespace Commands.CommandProcessor_specs;

interface CommandHandler<TCommand> { Task<Result<string>> Handle(TCommand command); }
interface CancelableCommandHandler<TCommand> { Task<Result<string>> Handle(TCommand command, CancellationToken token); }
interface SyncCommandHandler<TCommand> { string Handle(TCommand command); }

public class Cancelation_token
{
    [Test]
    public async Task passed_through_when_applicable()
        => (await new CancelableCommandProcessor(new AsyncCancelableCommandHandler())
        .Send(new EmptyCommand(), token: default))
        .Should().BeValid()
        .Value.Should().Be("AsyncCancelableCommandHandler.Handle(token)");

    [Test]
    public async Task ignored_when_not_in_contract()
        => (await new AsyncCommandProcessor(new AsyncCommandHandler())
        .Send(new EmptyCommand(), token: default))
        .Should().BeValid()
        .Value.Should().Be("AsyncCommandHandler.Handle()");

    [Test]
    public async Task added_when_method_with_token_is_available()
        => (await new CancelableCommandProcessor(new AsyncCancelableCommandHandler())
        .Send(new EmptyCommand()))
        .Should().BeValid()
        .Value.Should().Be("AsyncCancelableCommandHandler.Handle(token)");

    [Test]
    public async Task not_added_when_not_available()
        => (await new AsyncCommandProcessor(new AsyncCommandHandler())
        .Send(new EmptyCommand()))
        .Should().BeValid()
        .Value.Should().Be("AsyncCommandHandler.Handle()");
}

public class Throws_when
{
    [Test]
    public void hander_could_not_be_resolved()
    {
        Action send = () => new InvalidReturnTypeProcessor(null!).Send(new EmptyCommand());
        send.Should().Throw<UnresolvedCommandHandler>().WithMessage(
            "The command handler Commands.CommandProcessor_specs.SyncCommandHandler`1[Commands.CommandProcessor_specs.EmptyCommand] could not be resolved.");
    }

    [Test]
    public void method_could_not_be_resolved()
    {
        Action send = () => new InvalidReturnTypeProcessor(new AsyncCommandHandler()).Send(new EmptyCommand());

        send.Should().Throw<UnresolvedCommandHandler>().WithMessage(
            "The command handler method System.Int32 Commands.CommandProcessor_specs.SyncCommandHandler`1[Commands.CommandProcessor_specs.EmptyCommand].Handle(Commands.CommandProcessor_specs.EmptyCommand, token?) could not be resolved.");
    }
}

public class Caches
{
    [Test]
    public async Task supported_command_types()
    {
        var processor = new AsyncCommandProcessor(new AsyncCommandHandler());
        processor.CommandTypes.Should().BeEmpty(because: "not called yet.");
        (await processor.Send(new EmptyCommand())).Should().BeValid();
        processor.CommandTypes.Should().BeEquivalentTo(new[] { typeof(EmptyCommand) }, because: "empty command is supported.");
    }
}

class AsyncCommandProcessor : CommandProcessor<Task<Result<string>>>
{
    private readonly object handler;
    public AsyncCommandProcessor(object handler) => this.handler = handler;
    protected override Type GenericHandlerType => typeof(CommandHandler<>);
    protected override string HandlerMethod => nameof(CommandHandler<object>.Handle);
    protected override object GetHandler(Type handlerType) => handler;
}

class CancelableCommandProcessor : CommandProcessor<Task<Result<string>>>
{
    private readonly object handler;
    public CancelableCommandProcessor(object handler) => this.handler = handler;
    protected override Type GenericHandlerType => typeof(CancelableCommandHandler<>);
    protected override string HandlerMethod => nameof(CancelableCommandHandler<object>.Handle);
    protected override object GetHandler(Type handlerType) => handler;
}
class SyncCommandProcessor : CommandProcessor<string>
{
    private readonly object handler;
    public SyncCommandProcessor(object handler) => this.handler = handler;
    protected override Type GenericHandlerType => typeof(SyncCommandHandler<>);
    protected override string HandlerMethod => nameof(SyncCommandHandler<object>.Handle);
    protected override object GetHandler(Type handlerType) => handler;
}

class InvalidReturnTypeProcessor : CommandProcessor<int>
{
    private readonly object handler;
    public InvalidReturnTypeProcessor(object handler) => this.handler = handler;
    protected override Type GenericHandlerType => typeof(SyncCommandHandler<>);
    protected override string HandlerMethod => nameof(SyncCommandHandler<object>.Handle);
    protected override object GetHandler(Type handlerType) => handler;
}

[EmptyTestClass]
record EmptyCommand();

class AsyncCommandHandler : CommandHandler<EmptyCommand>
{
    public Task<Result<string>> Handle(EmptyCommand command) => Result.For("AsyncCommandHandler.Handle()").AsTask();
}

class AsyncCancelableCommandHandler : CancelableCommandHandler<EmptyCommand>
{
    public Task<Result<string>> Handle(EmptyCommand command) => throw new NotSupportedException("Use overload with token");
    public Task<Result<string>> Handle(EmptyCommand command, CancellationToken token) => Result.For("AsyncCancelableCommandHandler.Handle(token)").AsTask();
}
