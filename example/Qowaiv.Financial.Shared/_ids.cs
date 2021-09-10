using Qowaiv.Identifiers;
using System;

namespace Qowaiv.Financial.Shared
{
    public sealed class ForDivision : Int32IdBehavior
    {
        public override string ToString(object obj, string format, IFormatProvider formatProvider)
            => base.ToString(obj, string.IsNullOrEmpty(format) ? "00000" : format, formatProvider);
    }

    public sealed class ForGlAcount : StringIdBehavior { }


    public sealed class ForAccount : GuidBehavior { }
    public sealed class ForFinancialEntry : GuidBehavior { }
}
