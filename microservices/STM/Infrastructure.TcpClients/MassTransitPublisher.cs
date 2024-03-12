using Application.EventHandlers.Interfaces;
using MassTransit;
using Event = Application.EventHandlers.Event;

namespace Infrastructure.TcpClients;

public class MassTransitPublisher : IMassTransitPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task Publish<TEvent>(TEvent message) where TEvent : Event
    {
        await _publishEndpoint.Publish(message, x =>
        {
            x.SetRoutingKey("Stm.RideTrackingUpdated");
        });
    }
}