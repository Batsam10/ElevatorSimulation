using ElevatorSimulation.Application.Interfaces;
using ElevatorSimulation.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Drawing;

namespace ElevatorSimulation.Application.Services
{
    public class ElevatorSimulationService : IElevatorSimulationService
    {
        private Building _building;
        private bool _isRunning;
        private readonly ILogger<ElevatorSimulationService> _logger;

        public bool IsRunning => _isRunning;

        public ElevatorSimulationService(ILogger<ElevatorSimulationService> logger)
        {
            _isRunning = false;
            _logger = logger;
        }

        public void InitializeSimulation(int floors, int elevators, int capacity)
        {
            _building = Building.CreateBuilding(floors, elevators, capacity);
            _isRunning = true;
        }

        public void StartSimulation()
        {
            _logger.LogInformation("=== ELEVATOR SIMULATION STARTED ===");
            _logger.LogInformation($"Building: {_building.Floors.Count} floors, {_building.Elevators.Count} elevators, capacity {_building.Elevators.First().Capacity} each");
            _logger.LogInformation("");

            // Add some initial passengers for demonstration
            RequestElevator(1, 5, 2);
            RequestElevator(3, 8, 1);
            RequestElevator(7, 2, 3);

            while (_isRunning)
            {
                DisplaySimulationStatus();

                UpdateSimulation();

                // Wait for user input to advance simulation step
                _logger.LogInformation("Press [Enter] to advance, [R] to request elevator, [Q] to quit...");
                ConsoleKeyInfo key = Console.ReadKey(true);
                HandleUserInput(key);

            }
        }

        public void RequestElevator(int fromFloor, int toFloor, int numberOfPassengers)
        {
            var response = _building.RequestElevator(fromFloor, toFloor, numberOfPassengers);
            if (!response.IsSuccess)
            {
                _logger.LogError(response.ErrorMessage);
            }
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
            _logger.LogInformation("\n=== REQUEST ELEVATOR ===");

            try
            {
                _logger.LogInformation("From floor (1-10): ");
                int fromFloor = int.Parse(Console.ReadLine());

                _logger.LogInformation("To floor (1-10): ");
                int toFloor = int.Parse(Console.ReadLine());

                _logger.LogInformation("Number of passengers (1-8): ");
                int passengers = int.Parse(Console.ReadLine());

                RequestElevator(fromFloor, toFloor, passengers);
                _logger.LogInformation("Elevator requested successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error: {ex.Message}");
            }

            _logger.LogInformation("Press any key to continue...");
            Console.ReadKey();
        }

        public void UpdateSimulation()
        {
            var updateDetails = _building.Update();

            foreach ((var passenger, var elevator) in updateDetails.Arrived)
            {
                _logger.LogInformation($"Passenger {passenger.Id} arrived at floor {elevator.CurrentFloor}");

            }

            foreach ((var passenger, var elevator) in updateDetails.Boarded)
            {
                _logger.LogInformation($"Passenger {passenger.Id} boarded elevator {elevator.Id} at floor {elevator.CurrentFloor}");
            }
        }

        public void DisplaySimulationStatus()
        {
            Console.Clear();
            _logger.LogInformation("=== ELEVATOR SYSTEM STATUS ===");
            _logger.LogInformation($"Time: {DateTime.Now:HH:mm:ss}");
            _logger.LogInformation("");

            _logger.LogInformation("ELEVATORS:");
            foreach (var elevator in _building.Elevators)
            {
                _logger.LogInformation($"  {elevator}");
                if (elevator.DestinationFloors.Any())
                {
                    _logger.LogInformation($"    Destinations: [{string.Join(", ", elevator.DestinationFloors)}]");
                }
            }

            _logger.LogInformation("");

            _logger.LogInformation("FLOORS WITH WAITING PASSENGERS:");

            foreach (var floor in _building.Floors.Where(f => f.WaitingPassengers.Any()))
            {
                _logger.LogInformation($"  Floor {floor.FloorNumber}: {floor.WaitingPassengers.Count} waiting");
                foreach (var passenger in floor.WaitingPassengers)
                {
                    _logger.LogInformation($"    {passenger}");
                }
            }

            _logger.LogInformation("");
            _logger.LogInformation("Commands: [R]equest elevator, [Q]uit, [Enter] to continue simulation");
        }
    }
}