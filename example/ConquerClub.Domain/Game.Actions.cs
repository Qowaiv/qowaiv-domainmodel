using ConquerClub.Domain.Commands;
using ConquerClub.Domain.Events;
using Qowaiv.DomainModel;
using Qowaiv.Validation.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using Troschuetz.Random;
using ContinentId = Qowaiv.Identifiers.Id<ConquerClub.Domain.ForContinent>;
using CountryId = Qowaiv.Identifiers.Id<ConquerClub.Domain.ForCountry>;
using GameId = Qowaiv.Identifiers.Id<ConquerClub.Domain.ForGame>;

namespace ConquerClub.Domain
{
    public sealed partial class Game : AggregateRoot<Game, GameId>
    {
        public static Result<Game> Start(Start start, IGenerator rnd)
        {
            var game = new Game(start.Game);

            var map = new MapInitialized(
                Continents: start.Continents.Select(c => new ContinentInitialized(c.Name, c.Bonus, c.Territories.ToArray())).ToArray(),
                Countries: start.Countries.Select(c => new CountryInitialized(c.Name, c.Borders.ToArray())).ToArray());

            var settings = new SettingsInitialized(start.Players, start.RoundLimit, false);

            var armies = new ArmiesInitialized(
                RndArmies(start.Players, start.Countries.Length, rnd).ToArray());

            return game.ApplyEvents(
                map,
                settings,
                armies)
            | (g => g.ApplyEvent(g.StartTurn(Player.P1)));
        }

        public Result<Game> Deploy(CountryId country, Army army) =>
            MustBeInPhase(GamePhase.Deploy)
            | (g => g.MustBeActivePlayer(army.Owner))
            | (g => g.MustExist(country))
            | (g => g.MustBeOwnedBy(Countries.ById(country), army.Owner))
            | (g => g.MustNotExceedArmyBuffer(army))
            | (g => g.ApplyEvent(new Deployed(country, army)));

        public Result<Game> Attack(
            CountryId attacker,
            CountryId defender,
            IGenerator rnd) =>

            MustBeInPhase(GamePhase.Attack)
            | (g => g.MustExist(attacker))
            | (g => g.MustExist(defender))
            | (g => g.MustBeOwnedBy(Countries.ById(attacker), ActivePlayer))
            | (g => g.MustNotBeOwnedBy(Countries.ById(defender), ActivePlayer))
            | (g => g.MustBeReachable(Countries.ById(attacker), Countries.ById(defender)))
            | (g => g.MustHaveArmiesToAttack(Countries.ById(attacker)))
            | (g => g.Attack(attacker, defender, Dice
                .Attack(
                    Countries.ById(attacker).Army,
                    Countries.ById(defender).Army,
                    rnd)));

        public Result<Game> AutoAttack(
            CountryId attacker,
            CountryId defender,
            IGenerator rnd) =>

            MustBeInPhase(GamePhase.Attack)
            | (g => g.MustExist(attacker))
            | (g => g.MustExist(defender))
            | (g => g.MustBeOwnedBy(Countries.ById(attacker), ActivePlayer))
            | (g => g.MustNotBeOwnedBy(Countries.ById(defender), ActivePlayer))
            | (g => g.MustBeReachable(Countries.ById(attacker), Countries.ById(defender)))
            | (g => g.MustHaveArmiesToAttack(Countries.ById(attacker)))
            | (g => g.Attack(attacker, defender, Dice
               .AutoAttack(
                   Countries.ById(attacker).Army,
                   Countries.ById(defender).Army,
                   rnd)));

        private Result<Game> Attack(
            CountryId attacker,
            CountryId defender,
            AttackResult result)
            => Apply(Events
                .If(result.IsSuccess)
                    .Then(() => new Conquered(attacker, defender))
                .Else(() => new Attacked(attacker, defender, result)));

        public Result<Game> Advance(Army to) =>
            MustBeInPhase(GamePhase.Advance)
            | (g => g.MustBeActivePlayer(to.Owner))
            | (g => g.MustBeOwnedBy(To, to.Owner))
            | (g => g.MustNotExceedArmyBuffer(to))
            | (g => g.ApplyEvent(new Advanced(to)));

        public Result<Game> Reinforce(CountryId from, CountryId to, Army army) =>
            MustBeInPhase(GamePhase.Reinforce)
            | (g => g.MustExist(from))
            | (g => g.MustExist(to))
            | (g => g.MustBeOwnedBy(Countries.ById(from), army.Owner))
            | (g => g.MustBeOwnedBy(Countries.ById(to), army.Owner))
            | (g => g.MustBeReachable(Countries.ById(from), Countries.ById(to)))
            | (g => g.ApplyEvent(new Reinforced(from, to, army)));

        public Result<Game> Resign() =>
            Apply(Events
                .Add(new Resigned(ActivePlayer))
                .If(Countries.ActivePlayers().Count() == 2)
                    .Then(() => new Finished())
                .Else(() => StartTurn(NextPlayer)));

        internal void When(MapInitialized @event)
        {
            Continents = @event.Continents
                .Select((c, id) => new Continent(ContinentId.Create(id), c.Name, c.Bonus))
                .ToArray();

            Countries = @event.Countries
                .Select((c, id) => new Country(CountryId.Create(id), c.Name))
                .ToArray();

            LinkNeighborCountries(@event.Countries);
            LinkContinentsToCountries(@event.Continents);
            LinkCountriesToContent();
        }

        internal void When(SettingsInitialized @event)
            => Settings = new Settings(@event.Players, @event.RoundLimit, @event.FogOfWar);

        internal void When(ArmiesInitialized @event)
        {
            foreach (var data in @event.Armies.Select((army, index) => new
            {
                Army = army,
                Id = CountryId.Create(index)
            }))
            {
                Countries.ById(data.Id).Army = data.Army;
            }
            Phase = GamePhase.Deploy;
        }

        internal void When(ArmyInitiated @event)
            => Countries.ById(@event.Country).Army = @event.Army;

        internal void When(TurnStarted @event)
        {
            ActivePlayer = @event.Deployments.Owner;
            ArmyBuffer = @event.Deployments;
            Phase = GamePhase.Deploy;
        }

        internal void When(Deployed @event)
        {
            Countries.ById(@event.Country).Army += @event.Army;
            ArmyBuffer -= @event.Army;
            Phase = ArmyBuffer.Size > 0 ? GamePhase.Deploy : GamePhase.Attack;
        }

        internal void When(Attacked @event)
        {
            From = Countries.ById(@event.Attacker);
            To = Countries.ById(@event.Defender);
            From.Army = @event.Result.Attacker;
            To.Army = @event.Result.Defender;
        }
        
        internal void When(Conquered @event)
        {
            From = Countries.ById(@event.Attacker);
            To = Countries.ById(@event.Defender);
            ArmyBuffer = From.Army - 2;
            From.Army = From.Army.Owner.Army(1);
            To.Army = From.Army.Owner.Army(1);
            Phase = GamePhase.Advance;
        }

        internal void When(Advanced @event)
        {
            From.Army += ArmyBuffer - @event.To;
            To.Army += @event.To;

            ArmyBuffer = Army.None;
            From = null;
            To = null;
            Phase = GamePhase.Attack;
        }

        internal void When(Reinforced @event)
        {
            Countries.ById(@event.From).Army -= @event.Army;
            Countries.ById(@event.To).Army += @event.Army;
        }

        internal void When(Resigned @event)
        {
            foreach (var region in Countries.Where(r => r.Owner == @event.Player))
            {
                region.Army = Player.Neutral.Army(region.Army.Size);
            }
        }

        internal void When(Finished @event) => Phase = GamePhase.Finished;

        private void LinkNeighborCountries(IEnumerable<CountryInitialized> countries)
        {
            foreach (var data in countries.Select((c, id) => new
            {
                Country = CountryId.Create(id),
                c.Borders,
            }))
            {
                var country = Countries.ById(data.Country);
                country.Borders = data.Borders.Select(id => Countries.ById(id)).ToArray();
            }
        }

        private void LinkContinentsToCountries(IEnumerable<ContinentInitialized> continents)
        {
            foreach (var data in continents.Select((c, id) => new { Continent = ContinentId.Create(id), Countries = c.Territories }))
            {
                var continent = Continents.ById(data.Continent);

                foreach (var id in data.Countries)
                {
                    var country = Countries.ById(id);
                    country.Continent = continent;
                }
            }
        }

        private void LinkCountriesToContent()
        {
            foreach (var continent in Continents)
            {
                continent.Countries = Countries.Where(c => c.Continent == continent).ToArray();
            }
        }

        private TurnStarted StartTurn(Player player)
        {
            var countries = Countries.Count(c => c.Owner == player);
            var deploy = (Math.Max(3, countries / 3));
            deploy += Continents.Where(c => c.Owner == player).Sum(c => c.Bonus);
            return new TurnStarted(Deployments: player.Army(deploy));
        }

        private static IEnumerable<Army> RndArmies(int players, int countries, IGenerator rnd)
        {
            var perCountry = countries / (players + (players == 2 ? 1 : 0));

            return Enumerable
                .Range(0, countries)
                .Select(index =>
                {
                    var id = 1 + (index / perCountry);
                    return id == 3 && players == 2 || id > players ? 0 : id;
                })
                .Select(id => id == 0 ? Player.Neutral : new Player((byte)id))
                .Select(player => player.Army(3))
                .OrderBy(army => rnd.Next());
        }
    }
}
