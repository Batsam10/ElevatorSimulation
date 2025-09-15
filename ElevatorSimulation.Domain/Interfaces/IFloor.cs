using ElevatorSimulation.Domain.Types;

namespace ElevatorSimulation.Domain.Interfaces
{
    public interface IFloor
    {
        int FloorNumber { get; }
        IReadOnlyList<IPassenger> WaitingPassengers { get; }
        bool UpButtonPressed { get; set; }
        bool DownButtonPressed { get; set; }

        void CallElevator(Direction direction);
        void AddPassenger(IPassenger passenger);
        void RemovePassenger(IPassenger passenger);
    }
}
