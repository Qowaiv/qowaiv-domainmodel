using Qowaiv;

namespace ConquerClub.Domain
{
    public sealed class ForGame : Qowaiv.Identifiers.StringIdBehavior
    {
        public override object Next() => Uuid.NewUuid().ToString();
    }

    public sealed class ForContinent : Qowaiv.Identifiers.Int32IdBehavior { }
    public sealed class ForCountry : Qowaiv.Identifiers.Int32IdBehavior { }
}
