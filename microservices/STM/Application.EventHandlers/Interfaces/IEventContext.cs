namespace Application.EventHandlers.Interfaces;

public interface IEventContext
{
    Task<T?> TryGetAsync<T>();

    Task AddOrUpdateAsync<T>(T @event);
}