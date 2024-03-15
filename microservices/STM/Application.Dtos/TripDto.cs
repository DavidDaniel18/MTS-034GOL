
namespace Application.Dtos;

public sealed class TripDto
{
    public string Id { get; set; }

    public List<ScheduledStopDto> ScheduledStops { get; set; }
}