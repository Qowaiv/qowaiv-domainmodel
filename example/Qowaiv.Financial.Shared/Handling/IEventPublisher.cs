using System.Threading.Tasks;

namespace Qowaiv.Financial.Shared.Handling
{
    /// <summary>Publishes <see cref="DomainEvent"/>.</summary>
    public interface IEventPublisher
    {
        /// <summary>Publishes a <see cref="DomainEvent"/>.</summary>
        Task PublishAsync(DomainEvent domainEvent);
    }
}
