using Qowaiv.DomainModel;
using Qowaiv.Financial.Domain.Commands;
using Qowaiv.Financial.Domain.Events;
using Qowaiv.Financial.Domain.Validators;
using Qowaiv.Validation.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Qowaiv.Financial.Domain
{
    public sealed partial class FinancialEntry : AggregateRoot<FinancialEntry, Guid>
    {
        public FinancialEntry() : this(Guid.NewGuid()) { }

        public FinancialEntry(Guid id) : base(id, new FinancialEntryValidator()) { }

        public DateTime CreatedUtc { get; private set; }

        public Report Report { get; private set; }

        public IReadOnlyCollection<EntryLine> Lines => enties;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<EntryLine> enties = new List<EntryLine>();

        public Result<FinancialEntry> AddLines(params FinancialEntryLine[] lines)
        {
            return ApplyEvents(lines.Select(line => new EntryLineAdded
            {
                GlAccount = line.GlAccount,
                Amount = line.Amount,
                Date = line.Date,
                Description = line.Description,
                AccountId = line.AccountId,
            }).ToArray());
        }

        public static Result<FinancialEntry> Create(Guid id, Report report, FinancialEntryLine[] lines)
        {
            var entry = new FinancialEntry(id);

            var events = new List<object>
            {
                new Created
                {
                    Report = report,
                    CreatedUtc = Clock.UtcNow(),
                }
            };
            events.AddRange(lines.Select(line => new EntryLineAdded
            {
                GlAccount = line.GlAccount,
                Amount = line.Amount,
                Date = line.Date,
                Description = line.Description,
                AccountId = line.AccountId,
            }));

            return entry.ApplyEvents(@events.ToArray());
        }

        internal void When(Created @event)
        {
            Report = @event.Report;
            enties.Clear();
        }

        internal void When(EntryLineAdded @event)
        {
            enties.Add(new EntryLine
            {
                GlAccount = @event.GlAccount,
                Date = @event.Date,
                Amount = @event.Amount,
                Description = @event.Description,
                AccountId = @event.AccountId,
            });
        }
    }
}
