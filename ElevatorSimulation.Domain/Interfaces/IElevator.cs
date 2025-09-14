using ElevatorSimulation.Domain.Types;

namespace ElevatorSimulation.Domain.Interfaces
{
    public interface IElevator
    {
        int Id { get; }
        int CurrentFloor { get; }
        Direction Direction { get; }
        ElevatorState State { get; }
        int Capacity { get; }
        IReadOnlyList<IPassenger> Passengers { get; }
        IReadOnlyList<int> DestinationFloors { get; }

        bool CanAddPassenger();
        void AddPassenger(IPassenger passenger);
        void RemovePassenger(IPassenger passenger);
        void GoToFloor(int floor);
        void Move();
    }
}
