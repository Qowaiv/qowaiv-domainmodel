﻿using ConquerClub.Domain;
using ConquerClub.Domain.Commands;
using ConquerClub.Domain.Events;
using NUnit.Framework;
using Qowaiv.DomainModel;
using Qowaiv.Validation.TestTools;
using static ConquerClub.UnitTests.Arrange;
using Id = Qowaiv.Identifiers.Id<ConquerClub.Domain.ForGame>;

namespace Game_specs
{
    public class Resign_result_in
    {
        [Test]
        public void active_player_being_replaced_by_neutral_x()
        {
            var buffer = BeneluxWithoutArmies()
                .Add(new ArmiesInitialized
                {
                    Armies = new[]
                    {
                        Player.P1.Army(1),
                        Player.P2.Army(80),
                        Player.P3.Army(85),
                    }
                })
                .Add(new TurnStarted(Player.P1.Army(3)));

            var org = AggregateRoot.FromStorage<Game, Id>(buffer);
            var result = org.Resign();

            ValidationMessageAssert.IsValid(result);
            Assert.AreEqual(GamePhase.Deploy, result.Value.Phase);
            Assert.AreEqual(Player.Neutral.Army(1), result.Value.Countries.ById(Netherlands).Army);
            Assert.AreEqual(Player.P2, result.Value.ActivePlayer);
            Assert.AreEqual(new[] { Player.P2, Player.P3 }, result.Value.ActivePlayers);
        }


        [Test]
        public void active_player_being_replaced_by_neutral()
        {
            var command = new Resign(GameId, 4);
            var buffer = BeneluxWithoutArmies()
                .Add(new ArmiesInitialized
                {
                    Armies = new[]
                    {
                        Player.P1.Army(1),
                        Player.P2.Army(80),
                        Player.P3.Army(85),
                    }
                })
                .Add(new TurnStarted(Player.P1.Army(3)));
            var result = Handle(command, buffer);

            ValidationMessageAssert.IsValid(result);
            Assert.AreEqual(GamePhase.Deploy, result.Value.Phase);
            Assert.AreEqual(Player.Neutral.Army(1), result.Value.Countries.ById(Netherlands).Army);
            Assert.AreEqual(Player.P2, result.Value.ActivePlayer);
            Assert.AreEqual(new[] { Player.P2, Player.P3 }, result.Value.ActivePlayers);
        }

        [Test]
        public void the_game_beinig_ended_if_one_player_survives()
        {
            var command = new Resign(GameId, 4);

            var result = Handle(command, Benelux());

            ValidationMessageAssert.IsValid(result);
            Assert.AreEqual(GamePhase.Finished, result.Value.Phase);
        }
    }
}
