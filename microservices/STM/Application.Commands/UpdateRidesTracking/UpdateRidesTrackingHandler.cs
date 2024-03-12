using Application.Commands.Seedwork;
using Application.CommandServices.Repositories;
using Domain.Aggregates.Ride.Events;
using Domain.Common.Exceptions;
using Domain.Common.Interfaces.Events;
using Domain.Services.Aggregates;
using Microsoft.Extensions.Logging;

namespace Application.Commands.UpdateRidesTracking;

public class UpdateRidesTrackingHandler : ICommandHandler<UpdateRidesTrackingCommand>
{
    private readonly IBusWriteRepository _busRepository;
    private readonly ILogger<UpdateRidesTrackingHandler> _logger;
    private readonly IRideWriteRepository _rideRepository;
    private readonly ITripWriteRepository _tripRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RideServices _rideServices;
    private readonly IDomainEventHandler<RideTrackingUpdated> _domainEventHandler;

    public UpdateRidesTrackingHandler(
        IRideWriteRepository rideRepository,
        IBusWriteRepository busRepository,
        ITripWriteRepository tripRepository,
        IUnitOfWork unitOfWork,
        RideServices rideServices,
        IDomainEventHandler<RideTrackingUpdated> domainEventHandler,
        ILogger<UpdateRidesTrackingHandler> logger)
    {
        _rideRepository = rideRepository;
        _busRepository = busRepository;
        _tripRepository = tripRepository;
        _unitOfWork = unitOfWork;
        _rideServices = rideServices;
        _domainEventHandler = domainEventHandler;
        _logger = logger;
    }

    public async Task Handle(UpdateRidesTrackingCommand command, CancellationToken cancellation)
    {
        try
        {
            var rides = await _rideRepository.GetAllAsync();

            foreach (var ride in rides)
            {
                try
                {
                    var bus = await _busRepository.GetAsync(ride.BusId);

                    var trip = await _tripRepository.GetAsync(bus.TripId);

                    var update = _rideServices.UpdateRide(ride, bus, trip, command.Delta);

                    // yes this is horrible but it's a proof of concept im sorry
                    await _domainEventHandler.HandleAsync(update);
                }
                catch (Exception e) when(e is IndexOutsideOfTripException or KeyNotFoundException)
                {
                    _logger.LogError(e, $"Error while updating ride with ID {ride.Id}");

                    _rideServices.CompleteTracking(ride);
                }
                finally
                {
                    if (ride.TrackingComplete)
                    {
                        _logger.LogInformation($"Tracking completed for ride with ID {ride.Id}");

                        _rideRepository.Remove(ride);
                    }
                }
               
            }

            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while updating rides");
        }
    }
}