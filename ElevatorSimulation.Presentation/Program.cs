using System;
using ElevatorSimulation.Application.Interfaces;
using ElevatorSimulation.Application.Services;

namespace ElevatorSimulation.Presentation
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Setup Dependency Injection (simple for console app)
                IElevatorSimulationService simulationService = new ElevatorSimulationService();
                
                // Initialize and run simulation
                // Building with 10 floors, 3 elevators, capacity of 8 passengers each
                simulationService.InitializeSimulation(10, 3, 8);
                simulationService.StartSimulation();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
