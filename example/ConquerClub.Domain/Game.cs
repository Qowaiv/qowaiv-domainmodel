using ConquerClub.Domain.Events;
using Qowaiv.DomainModel;
using Qowaiv.Identifiers;
using Qowaiv.Validation.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConquerClub.Domain
{
    public sealed class Game : AggregateRoot<Game, Id<ForGame>>
    {
        public Game(Id<ForGame> aggregateId, IValidator<Game> validator) : base(aggregateId, validator)
        {
        }

        /// <summary>Gets the current round.</summary>
        public int Round { get; private set; }

        /// <summary>Gets the phase the round is in.</summary>
        public GamePhase Phase { get; set; }

        public Result<Game> Resign(Player player)
        {
            return ApplyEvent(new Resigned { Player = player });
        }

        internal void When(Resigned @event)
        {

        }
    }
}
