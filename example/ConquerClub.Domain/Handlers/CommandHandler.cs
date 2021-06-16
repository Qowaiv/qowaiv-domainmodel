using Qowaiv.Validation.Abstractions;

namespace ConquerClub.Domain.Handlers
{
    public interface CommandHandler<TCommand>
    {
        Result Handle(TCommand command);
    }
}
