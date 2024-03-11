using Application.Commands.Seedwork;
using Application.Commands.TrackBus;
using Application.Commands.UpdateRidesTracking;
using Application.EventHandlers.Interfaces;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Controllers.Rest;

[ApiController]
[Route("[controller]/[action]")]
public class TrackController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IEventConsumer _eventConsumer;
    private readonly ILogger<TrackController> _logger;

    public TrackController(ILogger<TrackController> logger, ICommandDispatcher commandDispatcher, IEventConsumer eventConsumer)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
        _eventConsumer = eventConsumer;
    }

    [HttpPost]
    [ActionName(nameof(BeginTracking))]
    public async Task<AcceptedResult> BeginTracking([FromBody] TrackBusCommand trackBusCommand)
    {
        _logger.LogInformation("TrackBus endpoint reached");

        await _commandDispatcher.DispatchAsync(trackBusCommand, CancellationToken.None);

        var updateRideCommand = new UpdateRidesTrackingCommand(DateTime.UtcNow);

        await _commandDispatcher.DispatchAsync(updateRideCommand, CancellationToken.None);

        return Accepted();
    }

    /// <summary>
    /// This does not allow to discriminate which bus is being tracked, maybe it should be published as an event by message queue?...
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ActionName(nameof(GetTrackingUpdate))]
    public async Task<ActionResult<ApplicationRideTrackingUpdated>> GetTrackingUpdate()
    {
        const int timeoutInMs = 5000;

        try
        {
            var update = await _eventConsumer.ConsumeNext<ApplicationRideTrackingUpdated>(new CancellationTokenSource(timeoutInMs).Token);

            return Ok(update);
        }
        catch (OperationCanceledException)
        {
            return Problem("Timeout while waiting for tracking update");
        }
    }
}