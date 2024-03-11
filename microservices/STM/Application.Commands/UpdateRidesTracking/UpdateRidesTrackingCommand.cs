using Application.Commands.Seedwork;

namespace Application.Commands.UpdateRidesTracking;

public record UpdateRidesTrackingCommand(DateTime Delta) : ICommand
{
    public string GetCommandName()
        => string.Empty; // Signifies to not logs it (called too often)
}