namespace Application.EventHandlers.Interfaces;

public interface IEventPublisher
{
    public Task Publish<TEvent>(TEvent message) where TEvent : Event;
}