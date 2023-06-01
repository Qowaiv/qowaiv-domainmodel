using Qowaiv;

namespace ConquerClub.Domain;

[Serializable]
public sealed class ForGame : Qowaiv.Identifiers.StringIdBehavior
{
    public override object Next() => Uuid.NewUuid().ToString();
}

[Serializable]
public sealed class ForContinent : Qowaiv.Identifiers.Int32IdBehavior { }

[Serializable]
public sealed class ForCountry : Qowaiv.Identifiers.Int32IdBehavior { }
