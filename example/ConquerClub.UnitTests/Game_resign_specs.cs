using ConquerClub.Domain;
using ConquerClub.Domain.Commands;
using ConquerClub.Domain.Events;
using NUnit.Framework;
using Qowaiv.Validation.TestTools;
using static ConquerClub.UnitTests.Arrange;

namespace Game_specs
{
    public class Resign_result_in
    {
        [Test]
        public void active_player_being_replaced_by_neutral()
        {
            var command = new Resign
            {
                ExpectedVersion = 4,
            };

            var result = Handle(command, BeneluxWithoutArmies()
                .Add(new ArmiesInitialized
                {
                    Armies = new[]
                    {
                        Player.P1.Army(1),
                        Player.P2.Army(80),
                        Player.P3.Army(85),
                    }
                })
                .Add(new TurnStarted { Deployments = Player.P1.Army(3) }));

            ValidationMessageAssert.IsValid(result);
            Assert.AreEqual(GamePhase.Deploy, result.Value.Phase);
            Assert.AreEqual(Player.Neutral.Army(1), result.Value.Countries.ById(Netherlands).Army);
            Assert.AreEqual(Player.P2, result.Value.ActivePlayer);
            Assert.AreEqual(new[] { Player.P2, Player.P3 }, result.Value.ActivePlayers);
        }

        [Test]
        public void the_game_beinig_ended_if_one_player_servives()
        {
            var command = new Resign
            {
                ExpectedVersion = 4,
            };

            var result = Handle(command, Benelux());

            ValidationMessageAssert.IsValid(result);
            Assert.AreEqual(GamePhase.Finished, result.Value.Phase);
        }
    }
}
