using System;

namespace Qowaiv.Financial.Domain.Events
{
    public record Created(Report Report, DateTime CreatedUtc);
}
