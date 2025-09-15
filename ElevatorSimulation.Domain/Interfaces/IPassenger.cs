using ElevatorSimulation.Domain.Types;

namespace ElevatorSimulation.Domain.Interfaces
{
    public interface IPassenger
    {
        int Id { get; }
        int OriginFloor { get; }
        int DestinationFloor { get; }
        PassengerState State { get; set; }
    }
}
