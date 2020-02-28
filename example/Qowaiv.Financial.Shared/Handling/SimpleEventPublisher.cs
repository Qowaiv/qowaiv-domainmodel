using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qowaiv.Financial.Shared.Handling
{
    /// <summary>Simple implementation of a <see cref="IEventPublisher"/>.</summary>
    /// <remarks>
    /// This is a (too) simple implementation on purose.
    /// 
    /// Dealing with messaging a queueing is way beyond the goal/purpose of this
    /// example.
    /// </remarks>
    public class SimpleEventPublisher : IEventPublisher
    {
        /// <summary>Creates a new instance of the <see cref="SimpleEventPublisher"/> class.</summary>
        public SimpleEventPublisher(IServiceProvider provider)
        {
            this.provider = Guard.NotNull(provider, nameof(provider));
        }

        /// <summary>Registers the listener.</summary>
        /// <typeparam name="TListener">
        /// The type of the listener to register.
        /// </typeparam>
        public SimpleEventPublisher Register<TListener>()
        {
            listenerTypes.Add(typeof(TListener));
            return this;
        }

        /// <inheritdoc />
        public async Task PublishAsync(DomainEvent domainEvent)
        {
            Guard.NotNull(domainEvent, nameof(domainEvent));
            var listeners = GetListeners();
            await Task.WhenAll(listeners.Select(listener => listener.HandleAsync(domainEvent)));
        }

        private IEnumerable<IEventListener> GetListeners()
        {
            foreach (var listenerType in listenerTypes)
            {
                var service = (IEventListener)provider.GetService(listenerType);
                if (service is null)
                {
                    throw new InvalidOperationException($"Could not resolve the event listener of the type {listenerType}.");
                }
                yield return service;
            }
        }

        private readonly HashSet<Type> listenerTypes = new HashSet<Type>();
        private readonly IServiceProvider provider;
    }
}
