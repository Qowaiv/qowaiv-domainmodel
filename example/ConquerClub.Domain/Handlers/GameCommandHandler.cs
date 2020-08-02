using ConquerClub.Domain.Commands;
using Qowaiv.DomainModel.Validation;
using Qowaiv.Identifiers;
using Qowaiv.Validation.Abstractions;
using System;
using Troschuetz.Random;

namespace ConquerClub.Domain.Handlers
{
    public class GameCommandHandler :
        CommandHandler<Start>,
        CommandHandler<Deploy>,
        CommandHandler<AutoAttack>,
        CommandHandler<Attack>,
        CommandHandler<Advance>,
        CommandHandler<Reinforce>,
        CommandHandler<Resign>
    {
        public GameCommandHandler(IGenerator rnd, Func<Id<ForGame>, Result<Game>> load)
        {
            Rnd = rnd;
            Load = load;
        }

        protected IGenerator Rnd { get; }
        public Func<Id<ForGame>, Result<Game>> Load { get; }

        public Result Handle(Start command)=> Game.Start(command, Rnd);

        public Result Handle(Deploy command)
            => Load(command.Game)
                | (g => OptimisticLocking(g, command))
                | (g => g.Deploy(
                    command.Country,
                    command.Army));

        public Result Handle(AutoAttack command)
            => Load(command.Game)
                | (g => OptimisticLocking(g, command))
                | (g => g.AutoAttack(
                    command.Attacker,
                    command.Defender,
                    Rnd));
 
        public Result Handle(Attack command)
            => Load(command.Game)
                | (g => g.Attack(
                    command.Attacker,
                    command.Defender,
                    Rnd));

        public Result Handle(Advance command)
            => Load(command.Game)
                | (g => OptimisticLocking(g, command))
                | (g => g.Advance(command.To));

        public Result Handle(Reinforce command)
            => Load(command.Game)
                | (g => OptimisticLocking(g, command))
                | (g => g.Reinforce(
                    command.From,
                    command.To,
                    command.Army));

        public Result Handle(Resign command)
            => Load(command.Game)
                | (g => OptimisticLocking(g, command))
                | (g => g.Resign(command.Player));

        private static Result OptimisticLocking(Game dossier, Command command) =>
            dossier.Version == command.ExpectedVersion
            ? Result.OK
            : Result.WithMessages(ConcurrencyIssue.VersionMismatch(command.ExpectedVersion, dossier.Version));
    }
}
