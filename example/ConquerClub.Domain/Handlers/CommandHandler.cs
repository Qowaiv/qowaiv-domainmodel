using Qowaiv.Validation.Abstractions;

namespace ConquerClub.Domain.Handlers
{
    public interface CommandHandler<in TCommand>
    {
        Result Handle(TCommand command);
    }
}
