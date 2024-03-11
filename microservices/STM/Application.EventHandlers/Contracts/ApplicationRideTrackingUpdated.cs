using Application.EventHandlers;

// MassTransit URN type resolutions, namespaces must be equal between projects for a shared type 
// ReSharper disable once CheckNamespace
namespace Contracts;

public class ApplicationRideTrackingUpdated : Event
{
    public ApplicationRideTrackingUpdated(string message, bool trackingCompleted, double duration, DateTime delta, Guid id, DateTime created) : base(id, created)
    {
        Message = message;
        TrackingCompleted = trackingCompleted;
        Duration = duration;
        Delta = delta;
    }

    public string Message { get; set; }

    public bool TrackingCompleted { get; set; }

    public double Duration { get; set; }

    public DateTime Delta { get; set; }
}