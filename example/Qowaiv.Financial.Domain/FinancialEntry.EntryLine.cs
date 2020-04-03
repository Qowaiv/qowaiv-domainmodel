using System;

namespace Qowaiv.Financial.Domain
{
    public partial class FinancialEntry
    {
        public sealed class EntryLine : IEquatable<EntryLine>
        {
            public Money Amount { get; internal set; }
            public Date Date { get; internal set; }
            public string Description { get; internal set; }
            public GlAccountCode GlAccount { get; internal set; }
            public Guid? AccountId { get; internal set; }

            public override bool Equals(object obj) => obj is EntryLine other && Equals(other);

            public bool Equals(EntryLine other)
            {
                return other != null
                    && Amount == other.Amount
                    && Date == other.Date
                    && Description == other.Description
                    && GlAccount == other.GlAccount
                    && AccountId == other.AccountId;
            }

            public override int GetHashCode()
            {
                return GetType().GetHashCode()
                    ^ Amount.GetHashCode()
                    ^ Date.GetHashCode()
                    ^ (Description ?? "").GetHashCode()
                    ^ GlAccount.GetHashCode()
                    ^ AccountId.GetHashCode();
            }

            public override string ToString() => $"Amount: {Amount}, Date: {Date}, GL: {GlAccount}, Account: {AccountId}, Desc: {Description}";
        }
    }
}

