using Application.Interfaces;
using Contracts;
using MassTransit;
using MqContracts;

namespace Infrastructure.Clients;

public class MassTransitRabbitMqClient : IDataStreamWriteModel
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitRabbitMqClient(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task Produce(BusPositionUpdated busPositionUpdated, DateTime delta, Guid positionUpdatedId)
    {
        try
        {
            await _publishEndpoint.Publish(busPositionUpdated,
            x =>
            {   
                x.SetRoutingKey("trip_comparison.response");
            });

            await _publishEndpoint.Publish(new BusPositionsUpdateCompleted(positionUpdatedId, DateTime.UtcNow, delta),
                x =>
            {
                x.SetRoutingKey("stm.update.completed");
            });
        }
        catch
        {
            // ignored - no need to fight over ack - single message is not that important in our context
        }
    }
}