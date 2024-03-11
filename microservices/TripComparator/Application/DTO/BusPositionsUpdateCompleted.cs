
// MassTransit URN type resolutions, namespaces must be equal between project for a shared type 
// ReSharper disable once CheckNamespace
namespace Contracts;

public class BusPositionsUpdateCompleted
{
    public BusPositionsUpdateCompleted(Guid id, DateTime created, DateTime delta)
    {
        Id = id;
        Created = created;
        Delta = delta;
    }

    public Guid Id { get; init; }

    public DateTime Created { get; init; }

    public DateTime Delta { get; set; }
}