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
    public partial class FinancialEntry : AggregateRoot<FinancialEntry>
    {
        public FinancialEntry() : this(Guid.NewGuid()) { }

        public FinancialEntry(Guid id) : base(id, new FinancialEntryValidator())
        {
        }

        public DateTime CreatedUtc { get; private set; }

        public Report Report { get; private set; }

        public IReadOnlyCollection<EntryLine> Lines => enties;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<EntryLine> enties = new List<EntryLine>();


        public Result<FinancialEntry> AddLines(params FinancialEntryLine[] lines)
        {
            return ApplyEvent(new EntryLinesAdded 
            {
                Lines = lines.Select(line => new EntryLinesAdded.Line
                {
                    GlAccount = line.GlAccount,
                    Amount = line.Amount,
                    Date = line.Date,
                    Description = line.Description,
                    AccountId = line.AccountId,
                }).ToArray(),
            });
        }

        public static Result<FinancialEntry> Create(Guid id, Report report, FinancialEntryLine[] lines)
        {
            var entry = new FinancialEntry(id);

            var @event = new Created
            {
                Report = report,
                CreatedUtc = Clock.UtcNow(),
                Lines = lines.Select(line => new Created.Line
                {
                    GlAccount = line.GlAccount,
                    Amount = line.Amount,
                    Date = line.Date,
                    Description = line.Description,
                    AccountId = line.AccountId,
                }).ToArray(),
            };
            return entry.ApplyEvent(@event);
        }

        internal void Apply(Created @event)
        {
            Report = @event.Report;
            enties.Clear();
            enties.AddRange(@event.Lines.Select(line => new EntryLine
            {
                AccountId = line.AccountId,
                Amount = line.Amount,
                Date = line.Date,
                Description = line.Description,
                GlAccount= line.GlAccount,
            }));
        }

        internal void Apply(EntryLinesAdded @event)
        {
            foreach(var line in @event.Lines)
            {
                enties.Add(new EntryLine
                {
                    GlAccount = line.GlAccount,
                    Date = line.Date,
                    Amount = line.Amount,
                    Description = line.Description,
                    AccountId = line.AccountId,
                });
            }
        }
    }
}
