using Qowaiv.Identifiers;

namespace ConquerClub.Domain.Commands
{
    public class Command
    {
        protected Command() { }
        public Id<ForGame> Game { get; set; }
        public int ExpectedVersion { get; set; }
    }
}
