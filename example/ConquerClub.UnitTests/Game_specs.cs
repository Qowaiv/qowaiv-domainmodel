using ConquerClub.Domain;
using ConquerClub.UnitTests;
using NUnit.Framework;
using Qowaiv.Validation.TestTools;
using System.Linq;

namespace Game_specs
{
    public class Start
    {
        [Test]
        public void With_two_players_includes_thrid_neutral_player()
        {
            var game = Game.Start(Arrange.Benelux(players: 2), Arrange.Rnd());

            ValidationMessageAssert.IsValid(game);

            CollectionAssert.AreEquivalent(new[]
            {
                Player.P1,
                Player.P2,
                Player.Neutral
            },
            game.Value.Countries.Select(c => c.Owner).Distinct());
        }
    }
}
