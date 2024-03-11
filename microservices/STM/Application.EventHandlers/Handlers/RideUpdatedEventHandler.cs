using Application.EventHandlers.Interfaces;
using Contracts;
using Domain.Aggregates.Ride.Events;
using Domain.Common.Interfaces;
using Domain.Common.Interfaces.Events;

namespace Application.EventHandlers.Handlers;

public class RideUpdatedEventHandler : IDomainEventHandler<RideTrackingUpdated>
{
    private readonly IMassTransitPublisher _eventPublisher;
    private readonly IEventContext _eventContext;
    private readonly IDatetimeProvider _datetimeProvider;

    public RideUpdatedEventHandler(IMassTransitPublisher eventPublisher, IEventContext eventContext, IDatetimeProvider datetimeProvider)
    {
        _eventPublisher = eventPublisher;
        _eventContext = eventContext;
        _datetimeProvider = datetimeProvider;
    }

    public async Task HandleAsync(RideTrackingUpdated domainEvent)
    {
        var applicationEvent = new ApplicationRideTrackingUpdated(
            domainEvent.Message,
            domainEvent.TrackingCompleted,
            domainEvent.Duration,
            domainEvent.delta,
            Guid.NewGuid(),
            _datetimeProvider.GetCurrentTime());

        await _eventContext.AddOrUpdateAsync(applicationEvent);

        //decoupling the domain event from the infrastructure
        await _eventPublisher.Publish(applicationEvent);
    }
}