namespace Infrastructure.EventDispatcher
{
    using System.Threading.Tasks;

    public interface IEventDispatcher
    {
        Task DispatchAsync<TEvent>(TEvent @event)
            where TEvent : IEvent;
    }
}
