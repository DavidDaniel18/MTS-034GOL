using Application.Commands.Seedwork;
using Application.Commands.UpdateRidesTracking;
using Application.EventHandlers.Interfaces;
using Contracts;
using MassTransit;

namespace Controllers.Events;

public sealed class UpdateBusPositionCompletedConsumer(ICommandDispatcher commandDispatcher, IEventContext eventContext) : IConsumer<BusPositionsUpdateCompleted>
{
    public async Task Consume(ConsumeContext<BusPositionsUpdateCompleted> context)
    {
        var lastUpdate = await eventContext.TryGetAsync<ApplicationRideTrackingUpdated>();

        if(lastUpdate!.Id.Equals(context.Message.Id) is false) return;

        var delta = 50 - (DateTime.UtcNow - context.Message.Delta).TotalMilliseconds;

        var delay = delta > 0 ? delta : 0;

        await Task.Delay(Convert.ToInt32(delay));

        var command = new UpdateRidesTrackingCommand(DateTime.UtcNow);

        await commandDispatcher.DispatchAsync(command, CancellationToken.None);
    }
}