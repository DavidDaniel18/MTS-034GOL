using Application.Interfaces.Policies;
using Application.Usecases;
using MassTransit;
using Microsoft.Extensions.Logging;
using MqContracts;

namespace Controllers.Controllers;

public class TripComparatorMqController : IConsumer<CoordinateMessage>
{
    private readonly CompareTimes _compareTimes;
    private readonly IInfiniteRetryPolicy<TripComparatorMqController> _infiniteRetryPolicy;

    private readonly ILogger<TripComparatorMqController> _logger;

    public TripComparatorMqController(
        ILogger<TripComparatorMqController> logger,
        CompareTimes compareTimes,
        IInfiniteRetryPolicy<TripComparatorMqController> infiniteRetryPolicy)
    {
        _logger = logger;
        _compareTimes = compareTimes;
        _infiniteRetryPolicy = infiniteRetryPolicy;
    }

    public async Task Consume(ConsumeContext<CoordinateMessage> context)
    {
        string startingCoordinates = context.Message.StartingCoordinates, destinationCoordinates = context.Message.DestinationCoordinates;

        _logger.LogInformation($"Comparing trip duration from {startingCoordinates} to {destinationCoordinates}");

        await _infiniteRetryPolicy.ExecuteAsync(async () => await _compareTimes.BeginComparingBusAndCarTime(
            RemoveWhiteSpaces(startingCoordinates),
            RemoveWhiteSpaces(destinationCoordinates)));
        string RemoveWhiteSpaces(string s) => s.Replace(" ", "");
    }
}