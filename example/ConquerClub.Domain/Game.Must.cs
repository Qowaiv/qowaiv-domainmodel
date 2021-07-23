using ConquerClub.Domain.Validation;
using FluentValidation.Internal;
using Qowaiv.Identifiers;
using Qowaiv.Validation.Abstractions;
using System.Linq;

namespace ConquerClub.Domain
{
    public partial class Game
    {
        private Result<Game> MustBeInPhase(GamePhase phase)
           => Must.Be(this, Phase == phase, Messages.MustBeInPhase, phase, Phase);

        private Result<Game> MustExist(Id<ForCountry> country)
            => Must.Be(this, Countries.ById(country) != null, Messages.CountryMustExist, country);

        private Result<Game> MustBeOwnedBy(Country country, Player owner)
            => Must.Be(this, country.Owner == owner, Messages.MustBeOwnedBy, country.Name, owner);

        private Result<Game> MustBeActivePlayer(Player player)
            => Must.Be(this, player == ActivePlayer, Messages.MustBeActive, ActivePlayer, player);

        private Result<Game> MustNotBeOwnedBy(Country country, Player owner)
            => Must.NotBe(this, country.Owner == owner, Messages.MustNotBeOwnedBy, country.Name, owner);

        private Result<Game> MustNotExeedArmyBuffer(Army army)
            => Must.Be(this, army <= ArmyBuffer, Messages.MustNotExeedArmyBuffer, ArmyBuffer, army);

        private Result<Game> MustBeReachable(Country from, Country to)
            => Must.Be(this, from.Borders.Contains(to), Messages.MustBeReachable, from.Name, to.Name);

        private Result<Game> MustHaveArmiesToAttack(Country attacker)
            => Must.Be(this, attacker.Army > 1, Messages.MustHaveArmiesToAttack, attacker.Name);

        private Result<Game> MusHaveCountry(Player player)
            => Must.Be(this, Countries.Any(c => c.Owner == player), Messages.MusHaveCountry, player);
    }
}
