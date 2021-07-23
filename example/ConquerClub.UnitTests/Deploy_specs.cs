using ConquerClub.Domain;
using ConquerClub.Domain.Events;
using NUnit.Framework;
using Qowaiv.Validation.Abstractions;
using Qowaiv.Validation.TestTools;
using static ConquerClub.UnitTests.Arrange;
using Commands = ConquerClub.Domain.Commands;

namespace Deploy_specs
{
    public class Must_be
    {
        [Test]
        public void in_deploy_phase()
        {
            var command = new Commands.Deploy
            {
                Army = Player.P1.Army(3),
                Country = Netherlands,

                Game = GameId,
                ExpectedVersion = 5,
            };

            var result = Handle(command, Benelux().Deploy());

            ValidationMessageAssert.WithErrors(result,
               ValidationMessage.Error("Action must be in the Deploy phase to be executed, not in the Attack phase."));
        }

        [Test]
        public void by_active_player()
        {
            var command = new Commands.Deploy
            {
                Army = Player.P2.Army(3),
                Country = Netherlands,

                Game = GameId,
                ExpectedVersion = 4,
            };

            var result = Handle(command, Benelux());

            ValidationMessageAssert.WithErrors(result,
               ValidationMessage.Error("Action can only be applied by the active P1, not by P2."));
        }

        [Test]
        public void to_existing_country()
        {
            var command = new Commands.Deploy
            {
                Army = Player.P1.Army(3),
                Country = Unknown,

                Game = GameId,
                ExpectedVersion = 4,
            };

            var result = Handle(command, Benelux());

            ValidationMessageAssert.WithErrors(result,
               ValidationMessage.Error("Country with id 666 does not exist."));
        }

        [Test]
        public void to_country_owned_by_player()
        {
            var command = new Commands.Deploy
            {
                Army = Player.P1.Army(3),
                Country = Belgium,

                Game = GameId,
                ExpectedVersion = 4,
            };

            var result = Handle(command, Benelux());

            ValidationMessageAssert.WithErrors(result,
               ValidationMessage.Error("Country Belgium must be owned by P1."));
        }

        [Test]
        public void with_army_that_not_exceeds_available()
        {
            var command = new Commands.Deploy
            {
                Army = Player.P1.Army(4),
                Country = Netherlands,

                Game = GameId,
                ExpectedVersion = 4,
            };

            var result = Handle(command, Benelux());

            ValidationMessageAssert.WithErrors(result,
               ValidationMessage.Error("Action not be executed as it requires more armies (P1.Army(4)) then available (P1.Army(3))."));
        }
    }
    
    public class Adds_armies_to_target
    { 
        [Test]
        public void and_stays_in_deploy_phase_when_deployment_left()
        {
            var command = new Commands.Deploy
            {
                Army = Player.P1.Army(2),
                Country = Netherlands,

                Game = GameId,
                ExpectedVersion = 4,
            };

            var result = Handle(command, Benelux());
            var game = ValidationMessageAssert.IsValid(result);

            Assert.AreEqual(Player.P1.Army(5), game.Countries.ById(Netherlands).Army);
            Assert.AreEqual(Player.P1.Army(1), game.ArmyBuffer);
            Assert.AreEqual(GamePhase.Deploy, game.Phase);
        }

        [Test]
        public void and_moves_to_attack_phase_when_out_deployments()
        {
            var command = new Commands.Deploy
            {
                Army = Player.P1.Army(3),
                Country = Netherlands,

                Game = GameId,
                ExpectedVersion = 4,
            };

            var result = Handle(command, Benelux());

            var game = ValidationMessageAssert.IsValid(result);

            Assert.AreEqual(Player.P1.Army(6), game.Countries.ById(Netherlands).Army);
            Assert.AreEqual(Army.None, game.ArmyBuffer);
            Assert.AreEqual(GamePhase.Attack, game.Phase);
        }
    }
}
