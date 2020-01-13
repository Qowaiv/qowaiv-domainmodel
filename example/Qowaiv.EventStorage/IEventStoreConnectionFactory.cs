using EventStore.ClientAPI;
using System.Threading.Tasks;

namespace Qowaiv.EventStorage
{
    public interface IEventStoreConnectionFactory
    {
        Task<IEventStoreConnection> CreateOpenConnectionAsync();
    }
}
