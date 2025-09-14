using ElevatorSimulation.Application.Interfaces;
using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Interfaces;

namespace ElevatorSimulation.Application.Services
{
    public class ElevatorSimulationService : IElevatorSimulationService
    {
        private IBuilding _building;
        private bool _isRunning;

        public bool IsRunning => _isRunning;

        public ElevatorSimulationService(int floors, int elevators, int capacity)
        {
            _building = new Building(floors, elevators, capacity);//Maybe having a DI container would be better for scalability
            _isRunning = true;
        }

        public void StartSimulation()
        {
            Console.WriteLine("=== ELEVATOR SIMULATION STARTED ===");
            Console.WriteLine($"Building: {_building.Floors.Count} floors, {_building.Elevators.Count} elevators, capacity {_building.Elevators.First().Capacity} each");
            Console.WriteLine();

            // Add some initial passengers for demonstration
            RequestElevator(1, 5, 2);
            RequestElevator(3, 8, 1);
            RequestElevator(7, 2, 3);

            while (_isRunning)
            {
                DisplaySimulationStatus();

                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    HandleUserInput(key);
                }

                UpdateSimulation();

                Thread.Sleep(1000);
            }
        }

        public void RequestElevator(int fromFloor, int toFloor, int numberOfPassengers)
        {
            _building.RequestElevator(fromFloor, toFloor, numberOfPassengers);
        }

        public void HandleUserInput(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.R:
                    RequestElevatorFromUser();
                    break;
                case ConsoleKey.Q:
                    _isRunning = false;
                    break;
            }
        }

        private void RequestElevatorFromUser()
        {
            Console.WriteLine("\n=== REQUEST ELEVATOR ===");

            try
            {
                Console.Write("From floor (1-10): ");
                int fromFloor = int.Parse(Console.ReadLine());

                Console.Write("To floor (1-10): ");
                int toFloor = int.Parse(Console.ReadLine());

                Console.Write("Number of passengers (1-8): ");
                int passengers = int.Parse(Console.ReadLine());

                RequestElevator(fromFloor, toFloor, passengers);
                Console.WriteLine("Elevator requested successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public void UpdateSimulation()
        {
            _building.Update();
        }

        public void DisplaySimulationStatus()
        {
            _building.DisplayStatus();
        }
    }
}
