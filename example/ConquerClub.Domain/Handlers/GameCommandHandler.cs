using ConquerClub.Domain.Commands;
using ConquerClub.Domain.Validation;
using Qowaiv.Identifiers;
using Qowaiv.Validation.Abstractions;
using Qowaiv.Validation.Messages;
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
        public GameCommandHandler(
            IGenerator rnd,
            Func<Id<ForGame>, Result<Game>> load,
            Func<Game, Result> save)
        {
            Rnd = rnd;
            Load = load;
            Save = save;
        }

        protected IGenerator Rnd { get; }
        public Func<Id<ForGame>, Result<Game>> Load { get; }
        public Func<Game, Result> Save { get; }

        public Result Handle(Start command)
            => Game.Start(command, Rnd)
                | (g => Save(g));

        public Result Handle(Deploy command)
             => ExcuteAndSave(command, g => g.Deploy(
                command.Country,
                command.Army));

        public Result Handle(AutoAttack command)
            => ExcuteAndSave(command, g => g.AutoAttack(
                command.Attacker,
                command.Defender,
                Rnd));

        public Result Handle(Attack command)
            => ExcuteAndSave(command, g => g.Attack(
                command.Attacker,
                command.Defender,
                Rnd));

        public Result Handle(Advance command)
            => ExcuteAndSave(command, g => g.Advance(command.To));

        public Result Handle(Reinforce command)
            => ExcuteAndSave(command, g => g.Reinforce(
                command.From,
                command.To,
                command.Army));

        public Result Handle(Resign command)
            => ExcuteAndSave(command, g => g.Resign());

        private Result ExcuteAndSave<TCommand>(TCommand command, Func<Game, Result<Game>> act) where TCommand : Command
            => Load(command.Game)
            | (g => OptimisticLocking(g, command))
            | (g => act(g))
            | (g => Save(g));

        private static Result OptimisticLocking(Game dossier, Command command) =>
            dossier.Version == command.ExpectedVersion
            ? Result.OK
            : Result.WithMessages(ConcurrencyIssue.VersionMismatch(command.ExpectedVersion, dossier.Version));
    }
}
