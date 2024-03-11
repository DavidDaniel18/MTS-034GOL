using MqContracts;

namespace Application.Interfaces;

public interface IDataStreamWriteModel
{
    Task Produce(BusPositionUpdated busPositionUpdated, DateTime delta, Guid positionUpdatedId);
}