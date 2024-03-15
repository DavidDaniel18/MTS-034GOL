using Application.Commands.Seedwork;
using Application.CommandServices.Repositories;
using Application.Dtos;
using Application.EventHandlers.Interfaces;
using Application.Mapping.Interfaces;
using Domain.Aggregates.Ride.Events;
using Domain.Aggregates.Trip;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly RideServices _rideServices;
    private readonly IEventContext _eventContext;
    private readonly IMappingTo<TripDto, Trip> _tripMapper;
    private readonly IDomainEventHandler<RideTrackingUpdated> _domainEventHandler;

    public UpdateRidesTrackingHandler(
        IRideWriteRepository rideRepository,
        IBusWriteRepository busRepository,
        IUnitOfWork unitOfWork,
        RideServices rideServices,
        IEventContext eventContext,
        IMappingTo<TripDto, Trip> tripMapper,
        IDomainEventHandler<RideTrackingUpdated> domainEventHandler,
        ILogger<UpdateRidesTrackingHandler> logger)
    {
        _rideRepository = rideRepository;
        _busRepository = busRepository;
        _unitOfWork = unitOfWork;
        _rideServices = rideServices;
        _eventContext = eventContext;
        _tripMapper = tripMapper;
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

                    var tripDto = await _eventContext.TryGetAsync<TripDto>();

                    var trip = _tripMapper.MapFrom(tripDto!);

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