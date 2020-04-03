using System.Threading.Tasks;

namespace Qowaiv.Financial.Shared.Handling
{
    /// <summary>Listener to an event.</summary>
    public interface IEventListener
    {
        /// <summary>Handles the <paramref name="domainEvent"/> asynchronously.</summary>
        Task HandleAsync(DomainEvent domainEvent);
    }
}
