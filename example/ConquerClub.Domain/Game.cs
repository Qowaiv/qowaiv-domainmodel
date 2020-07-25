using ConquerClub.Domain.Commands;
using ConquerClub.Domain.Events;
using ConquerClub.Domain.Validation;
using Qowaiv.DomainModel;
using Qowaiv.Identifiers;
using Qowaiv.Validation.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using Troschuetz.Random;

namespace ConquerClub.Domain
{
    public sealed class Game : AggregateRoot<Game, Id<ForGame>>
    {
        public Game() : this(Id<ForGame>.Empty) { }
        public Game(Id<ForGame> id) : base(id, new GameValidator()) { }

        public Settings Settings { get; private set; }

        public IReadOnlyList<Continent> Continents { get; private set; }
        public IReadOnlyList<Country> Countries { get; private set; }

        /// <summary>Gets the current round.</summary>
        public int Round { get; private set; } = 1;

        /// <summary>Gets the phase the round is in.</summary>
        public GamePhase Phase { get; private set; }

        /// <summary>Gets or sets the active player.</summary>
        public Player Active { get; private  set; }

        /// <summary>Gets or sets the last from country.</summary>
        public Country From { get; private set; }

        /// <summary>Gets or sets the last to country.</summary>
        public Country To { get; private set; }

        /// <summary>Gets or sets the armies to <see cref="Commands.Deploy"/> and <see cref="Commands.Advance"/>.</summary>
        public Army ArmyBuffer { get; private set; }


        public Result<Game> Deploy(Id<ForCountry> country, Army army)
        {
            // in phase.
            // country should exist.
            // army owner should match active player.

            return ApplyEvent(new Deployed 
            {
                Country = country, 
                Army = army, 
            });
        }

        public Result<Game> Attack(
            Id<ForCountry> attacker,
            Id<ForCountry> defender,
            IGenerator rnd)
        {
            // in phase.
            // attacker has more then 1 army.
            // attacker is active player.
            // defender is not active player.
            // attacker country can reach defender country.

            var att = Countries.ById(attacker);
            var def = Countries.ById(defender);

            var result = Dice.Attack(att.Army - 1, def.Army, rnd);

            return ApplyEvent(new Attacked
            {
                Attacker = att.Id,
                Defender = def.Id,
                Result = result,
            });
        }

        public Result<Game> AutoAttack(
            Id<ForCountry> attacker,
            Id<ForCountry> defender,
            IGenerator rnd)
        {
            // in phase.
            // attacker has more then 1 army.
            // attacker is active player.
            // defender is not active player.
            // attacker country can reach defender country.

            var att = Countries.ById(attacker);
            var def = Countries.ById(defender);

            var result = Dice.AutoAttack(att.Army - 1, def.Army, rnd);

            return ApplyEvent(new Attacked
            {
                Attacker = att.Id,   
                Defender = def.Id,
                Result = result,
            });
        }

        public Result<Game> Advance(Army to)
        {
            // in phase.
            // to owner is active player.
            // to owner equals owner army buffer.
            // to <= army buffer.

            return ApplyEvent(new Advanced
            {
                To = to,
            });
        }

        public Result<Game> Reinforce(Id<ForCountry> from, Id<ForCountry> to, Army army)
        {
            // in phase.
            // counties should exist.
            // army owner should match active player.
            // both countries should have same owner as army.

            return ApplyEvent(new Reinforced
            {
                From = from,
                To = to,
                Army = army,
            });
        }

        public Result<Game> Resign(Player player)
        {
            return ApplyEvent(new Resigned { Player = player });
        }

        internal void When(MapInitialized @event)
        {
            Continents = @event.Continents
                .Select((c, id) => new Continent(Id<ForContinent>.Create(id), c.Name, c.Bonus))
                .ToArray();

            Countries = @event.Countries
                .Select((c, id) => new Country(Id<ForCountry>.Create(id), c.Name))
                .ToArray();

            LinkCountriesToCountries(@event.Countries);
            LinkContinentToCounties(@event.Continents);
            LinkCountriesToContent();
        }

        internal void When(SettingsInitialized @event)
        {
            Settings = new Settings(
                players: @event.Players,
                roundLimit: @event.RoundLimit,
                fogOfWar: @event.FogOfWar);
        }

        internal void When(ArmyInitiated @event)
        {
            Countries.ById(@event.Country).Army = @event.Army;
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

            if (@event.Result.IsSuccess)
            {
                ArmyBuffer = From.Army - 2;
                From.Army = From.Army.Owner.Army(1);
                To.Army = From.Army.Owner.Army(1);
                Phase = GamePhase.Advance;
            }
            else
            {
                From.Army = @event.Result.Attacker;
                To.Army = @event.Result.Defender;
            }
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

        private void LinkCountriesToCountries(MapInitialized.Country[] countries)
        {
            foreach (var data in countries.Select((c, id) => new
            {
                Country = Id<ForCountry>.Create(id),
                Borders = c.Borders.Select(t => Id<ForCountry>.Create(t)).ToArray(),
            }))
            {
                var country = Countries.ById(data.Country);
                country.Borders = data.Borders.Select(id => Countries.ById(id)).ToArray();
            }
        }
        private void LinkContinentToCounties(MapInitialized.Continent[] continents)
        {
            foreach (var data in continents.Select((c, id) => new
            {
                Continent = Id<ForContinent>.Create(id),
                Countries = c.Territories.Select(t => Id<ForCountry>.Create(t)).ToArray(),
            }))
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

        public static Result<Game> Start(Start start)
        {
            var game = new Game(start.Game);

            var map = new MapInitialized
            {
                Continents = start.Continents.Select(c =>
                    new MapInitialized.Continent
                    {
                        Name = c.Name,
                        Bonus = c.Bonus,
                        Territories = c.Territories.ToArray(),
                    }).ToArray(),
                Countries = start.Countries.Select(c =>
                    new MapInitialized.Country
                    {
                        Name = c.Name,
                        Borders = c.Borders.ToArray(),
                    }).ToArray()
            };
            var settings = new SettingsInitialized
            {
                Players = start.Players,
                RoundLimit = start.RoundLimit,
            };
            return game.ApplyEvents(map, settings);
        }
    }
}
