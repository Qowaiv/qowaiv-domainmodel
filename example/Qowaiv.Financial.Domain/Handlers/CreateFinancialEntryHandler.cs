using Qowaiv.Financial.Domain.Commands;
using Qowaiv.Validation.Abstractions;
using System;
using System.Threading.Tasks;

namespace Qowaiv.Financial.Domain.Handlers
{
    public class CreateFinancialEntryHandler
    {
        public async Task<Result> HandleAsync(CreateFinancialEntry command)
        {
            // TODO: validate with external resources if report is available
            // TOOD: validate linked GL account (code)
            // TODO: validate linked account (ID)
            var report = new Report(command.ReportingYear, command.ReportingMonth);
            return await FinancialEntry.Create(command.Id, report, command.Lines)
                .ActAsync(e => SaveAsync(e));
        }

        private Task<Result> SaveAsync(FinancialEntry entry)
        {
            // todo: save entry.Stream;
            throw new NotImplementedException();
        }
    }
}
