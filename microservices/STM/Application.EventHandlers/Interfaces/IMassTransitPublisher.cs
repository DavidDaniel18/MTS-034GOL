namespace Application.EventHandlers.Interfaces;

public interface IMassTransitPublisher
{
    Task Publish<TEvent>(TEvent message) where TEvent : Event;
}