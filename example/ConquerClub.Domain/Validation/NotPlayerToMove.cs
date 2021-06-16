using Qowaiv.Validation.Abstractions;
using System;

namespace ConquerClub.Domain.Validation
{
    public class NotPlayerToMove : Exception, IValidationMessage
    {
        public NotPlayerToMove(Player player) 
            : base($"Player {player} is not the player to make a move.") { }

        public ValidationSeverity Severity => ValidationSeverity.Error;

        public string PropertyName => null;
    }
}
