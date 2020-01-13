using NUnit.Framework;
using Qowaiv.DomainModel.TestTools.EventSourcing;
using Qowaiv.Financial.Domain;
using Qowaiv.Financial.Domain.Commands;
using Qowaiv.Financial.Domain.Events;
using System;
using System.Text;

namespace Qowaiv.Financial.UnitTests.Domain
{
    public class FinancialEntryTest
    {
        [Test]
        public void Create_WithBalancedLines_PublishesOneEvent()
        {
            var entry = FinancialEntry.Create(
                id: Id(nameof(Create_WithBalancedLines_PublishesOneEvent)),
                report: new Report(2020, Month.January),
                lines: new FinancialEntryLine[]
                {
                    new FinancialEntryLine
                    {
                        Amount = +123.45 + Currency.EUR,
                        Date = new Date(2020, 01, 02),
                        GlAccount = new GlAccountCode("0070"),
                        Description = "New Year Reception",
                    },
                    new FinancialEntryLine
                    {
                        Amount = -123.45 + Currency.EUR,
                        Date = new Date(2020, 01, 02),
                        GlAccount = new GlAccountCode("0180"),
                    },
                }).Value;

            AggregateRootAssert.HasUncommittedEvents(entry,
                new Created
                {
                    CreatedUtc = new DateTime(2020, 01, 03, 12, 15, 37),
                    Lines = new[]
                    {
                        new Created.Line
                        {
                            Amount = +123.45 + Currency.EUR,
                            Date = new Date(2020, 01, 02),
                            GlAccount = new GlAccountCode("0070"),
                            Description = "New Year Reception",
                        },
                        new Created.Line
                        {
                            Amount = -123.45 + Currency.EUR,
                            Date = new Date(2020, 01, 02),
                            GlAccount = new GlAccountCode("0180"),
                        },
                    }
                });
        }

        private static Guid Id(string str) => Uuid.GenerateWithSHA1(Encoding.UTF8.GetBytes(str));
    }
}
