namespace ElevatorSimulation.Application.Interfaces
{
    public interface IElevatorSimulationService
    {
        void InitializeSimulation(int floors, int elevators, int capacity);
        void StartSimulation();
        void RequestElevator(int fromFloor, int toFloor, int numberOfPassengers);
        void HandleUserInput(ConsoleKeyInfo key);
        void UpdateSimulation();
        void DisplaySimulationStatus();
        bool IsRunning { get; }
    }
}

