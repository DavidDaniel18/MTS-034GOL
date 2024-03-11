using Application.Interfaces.Policies;
using Application.Usecases;
using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Controllers.Controllers;

public class RideTrackingUpdatedMqController : IConsumer<ApplicationRideTrackingUpdated>
{
    private readonly CompareTimes _compareTimes;

    public RideTrackingUpdatedMqController(CompareTimes compareTimes)
    {
        _compareTimes = compareTimes;
    }

    public async Task Consume(ConsumeContext<ApplicationRideTrackingUpdated> context)
    {
        await _compareTimes.WriteUpdate(context.Message);
    }
}