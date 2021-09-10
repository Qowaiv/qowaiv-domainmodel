﻿using EventStorage.Abstractions;
using Qowaiv.Financial.Domain.Commands;
using Qowaiv.Financial.Shared;
using Qowaiv.Financial.Shared.Handling;
using Qowaiv.Validation.Abstractions;
using System.Threading.Tasks;
using FinancialEntryId = Qowaiv.Identifiers.Id<Qowaiv.Financial.Shared.ForFinancialEntry>;

namespace Qowaiv.Financial.Domain.Handlers
{
    public class CreateFinancialEntryHandler : ICommandHandler<CreateFinancialEntry>
    {
        private readonly IEventStore<FinancialEntryId> store;
        private readonly IEventPublisher publisher;

        public CreateFinancialEntryHandler(IEventStore<FinancialEntryId> store, IEventPublisher publisher)
        {
            this.store = Guard.NotNull(store, nameof(store));
            this.publisher = Guard.NotNull(publisher, nameof(publisher));
        }

        public async Task<Result> HandleAsync(CreateFinancialEntry command)
        {
            // TODO: validate with external resources if report is available
            // TOOD: validate linked Division (code)
            // TOOD: validate linked GL account (code)
            // TODO: validate linked account (ID)
            var report = new Report(command.ReportingYear, command.ReportingMonth);
            return await FinancialEntry
                .Create(command.Id, report, command.Lines)
                .ActAsync(e => SaveAsync(e));
        }

        private async Task<Result> SaveAsync(FinancialEntry entry)
        {
            await store.SaveAsync(entry.Buffer);
            foreach (var committed in entry.Buffer.Committed)
            {
                await publisher.PublishAsync(new DomainEvent(entry.Id, committed));
            }
            return Result.OK;
        }
    }
}
