using ConquerClub.Domain.Validation;
using Qowaiv.Identifiers;
using Qowaiv.Validation.Abstractions;
using System.Linq;

namespace ConquerClub.Domain
{
    public partial class Game
    {
        private Result<Game> MustBeInPhase(GamePhase phase)
           => Must.BeTrue(this, Phase == phase, Messages.MustBeInPhase, phase, Phase);

        private Result<Game> MustExist(Id<ForCountry> country)
            => Must.BeTrue(this, Countries.ById(country) != null, Messages.CountryMustExist, country);

        private Result<Game> MustBeOwnedBy(Country country, Player owner)
            => Must.BeTrue(this, country.Owner == owner, Messages.MustBeOwnedBy, country.Name, owner);

        private Result<Game> MustBeActivePlayer(Player player)
            => Must.BeTrue(this, player == Active, Messages.MustBeActive, Active, player);

        private Result<Game> MustNotExeedArmyBuffer(Army army)
            => Must.BeTrue(this, army <= ArmyBuffer, Messages.MustNotExeedArmyBuffer, ArmyBuffer, army);

        private Result<Game> MustBeReachable(Country from, Country to)
            => Must.BeTrue(this, from.Borders.Contains(to), Messages.MustBeReachable, from.Name, to.Name);
    }
}
