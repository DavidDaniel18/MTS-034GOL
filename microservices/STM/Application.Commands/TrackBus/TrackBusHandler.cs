using Application.Commands.Seedwork;
using Application.CommandServices.Repositories;
using Application.Dtos;
using Application.EventHandlers.Interfaces;
using Application.Mapping.Interfaces;
using Domain.Aggregates.Trip;
using Domain.Services.Aggregates;
using Microsoft.Extensions.Logging;

namespace Application.Commands.TrackBus;

public class TrackBusHandler : ICommandHandler<TrackBusCommand>
{
    private readonly IBusWriteRepository _busRepository;
    private readonly RideServices _domainRideServices;
    private readonly IEventContext _eventContext;
    private readonly IMappingTo<Trip, TripDto> _tripMapper;
    private readonly ILogger<TrackBusHandler> _logger;
    private readonly IRideWriteRepository _rideRepository;
    private readonly ITripWriteRepository _tripRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TrackBusHandler(
        ITripWriteRepository tripRepository,
        IRideWriteRepository rideRepository,
        IBusWriteRepository busRepository,
        IUnitOfWork unitOfWork,
        RideServices domainRideServices,
        IEventContext eventContext,
        IMappingTo<Trip, TripDto> tripMapper,
        ILogger<TrackBusHandler> logger)
    {
        _tripRepository = tripRepository;
        _rideRepository = rideRepository;
        _busRepository = busRepository;
        _unitOfWork = unitOfWork;
        _domainRideServices = domainRideServices;
        _eventContext = eventContext;
        _tripMapper = tripMapper;
        _logger = logger;
    }

    public async Task Handle(TrackBusCommand command, CancellationToken cancellation)
    {
        try
        {
            var bus = await _busRepository.GetAsync(command.BusId);

            var trip = await _tripRepository.GetAsync(bus.TripId);

            var ride = _domainRideServices.CreateRide(
                bus,
                trip,
                command.ScheduledDepartureId,
                command.ScheduledDestinationId);

            await _rideRepository.AddOrUpdateAsync(ride);

            var tripDto = _tripMapper.MapFrom(trip);

            await _eventContext.AddOrUpdateAsync(tripDto);

            await _unitOfWork.SaveChangesAsync();
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(
                $"An unknown error occurred while tracking bus with ID {command.BusId}. Exception: {e.Message}");

            throw;
        }
    }
}