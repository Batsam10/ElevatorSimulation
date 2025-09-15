namespace ElevatorSimulation.Domain.Interfaces
{
    public interface IBuilding
    {
        IReadOnlyList<IFloor> Floors { get; }
        IReadOnlyList<IElevator> Elevators { get; }

        void RequestElevator(int floor, int destinationFloor, int numberOfPassengers = 1);
        void Update();
        void DisplayStatus();
    }
}
