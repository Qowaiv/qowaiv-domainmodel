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

        public Result Handle(Start command)
        {
            return Game.Start(command, Rnd);
        }

        public Result Handle(Deploy command)
        {
            return Load(command.Game)
                .Act(game => OptimisticLocking(game, command))
                .Act(game => game.Deploy(
                    command.Country,
                    command.Army));
        }

        public Result Handle(AutoAttack command)
        {
            return Load(command.Game)
                .Act(game => OptimisticLocking(game, command))
                .Act(game => game.AutoAttack(
                    command.Attacker,
                    command.Defender,
                    Rnd));
        }
 
        public Result Handle(Attack command)
        {
            return Load(command.Game)
                .Act(game => game.Attack(
                    command.Attacker,
                    command.Defender,
                    Rnd));
        }

        public Result Handle(Advance command)
        {
            return Load(command.Game)
                .Act(game => OptimisticLocking(game, command))
                .Act(game => game.Advance(command.To));
        }

        public Result Handle(Reinforce command)
        {
            return Load(command.Game)
                .Act(game => OptimisticLocking(game, command))
                .Act(game => game.Reinforce(
                    command.From,
                    command.To,
                    command.Army));
        }

        public Result Handle(Resign command)
        {
            return Load(command.Game)
                .Act(game => OptimisticLocking(game, command))
                .Act(game => game.Resign(command.Player));
        }


        private static Result OptimisticLocking(Game dossier, Command command)
        {
            if (dossier.Version != command.ExpectedVersion)
            {
                return Result.WithMessages(ConcurrencyIssue.VersionMismatch(command.ExpectedVersion, dossier.Version));
            }
            return Result.OK;
        }

    }
}
