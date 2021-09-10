using Qowaiv.DomainModel;
using Qowaiv.Financial.Domain.Commands;
using Qowaiv.Financial.Domain.Events;
using Qowaiv.Financial.Domain.Validators;
using Qowaiv.Validation.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FinancialEntryId = Qowaiv.Identifiers.Id<Qowaiv.Financial.Shared.ForFinancialEntry>;

namespace Qowaiv.Financial.Domain
{
    public sealed partial class FinancialEntry : AggregateRoot<FinancialEntry, FinancialEntryId>
    {
        public FinancialEntry() : this(FinancialEntryId.Next()) { }

        public FinancialEntry(FinancialEntryId id) : base(id, new FinancialEntryValidator()) { }

        public DateTime CreatedUtc { get; private set; }

        public Report Report { get; private set; }

        public IReadOnlyCollection<EntryLine> Lines => enties;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<EntryLine> enties = new List<EntryLine>();

        public Result<FinancialEntry> AddLines(params FinancialEntryLine[] lines)
            => Apply(Events.Add(lines.Select(line => new EntryLineAdded(
                line.GlAccount,
                line.Date,
                line.Amount,
                line.Description,
                line.AccountId))));

        public static Result<FinancialEntry> Create(FinancialEntryId id, Report report, FinancialEntryLine[] lines)
            => new FinancialEntry(id)
            .Apply(Events
                .Add(new Created(report, Clock.UtcNow()))
                .Add(lines.Select(line => new EntryLineAdded(
                    line.GlAccount,
                    line.Date,
                    line.Amount,
                    line.Description,
                    line.AccountId))));

        internal void When(Created @event)
        {
            Report = @event.Report;
            enties.Clear();
        }

        internal void When(EntryLineAdded @event)
            => enties.Add(new EntryLine(
                @event.Amount,
                @event.Date,
                @event.Description,
                @event.GlAccount,
                @event.AccountId));

    }
}
