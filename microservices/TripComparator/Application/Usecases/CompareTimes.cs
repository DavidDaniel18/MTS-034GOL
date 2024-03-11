using Application.Interfaces;
using Contracts;
using MqContracts;

namespace Application.Usecases
{
    public class CompareTimes
    {
        private readonly IRouteTimeProvider _routeTimeProvider;

        private readonly IBusInfoProvider _iBusInfoProvider;

        private readonly IDataStreamWriteModel _dataStreamWriteModel;


        public CompareTimes(IRouteTimeProvider routeTimeProvider, IBusInfoProvider iBusInfoProvider, IDataStreamWriteModel dataStreamWriteModel)
        {
            _routeTimeProvider = routeTimeProvider;
            _iBusInfoProvider = iBusInfoProvider;
            _dataStreamWriteModel = dataStreamWriteModel;
        }

        public async Task BeginComparingBusAndCarTime(string startingCoordinates, string destinationCoordinates)
        {
            await Task.WhenAll(
                _routeTimeProvider.GetTravelTimeInSeconds(startingCoordinates, destinationCoordinates),
                _iBusInfoProvider.GetBestBus(startingCoordinates, destinationCoordinates)
                    .ContinueWith(task => _iBusInfoProvider.BeginTracking(task.Result)));
        }

        public async Task WriteUpdate(ApplicationRideTrackingUpdated positionUpdated)
        {
            positionUpdated.Message += $"\nCar: {_routeTimeProvider.GetSavedTravelTimeInSeconds()} seconds";

            await _dataStreamWriteModel.Produce(new BusPositionUpdated()
            {
                Message = positionUpdated.Message,
                Seconds = positionUpdated.Duration,
            }, positionUpdated.Delta,
                positionUpdated.Id);
        }
    }
}
