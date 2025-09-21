using ElevatorSimulation.Application.Interfaces;
using ElevatorSimulation.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ElevatorSimulation.Presentation
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var host = Host.CreateDefaultBuilder(args)
                     .ConfigureServices(services =>
                     {
                         services.AddScoped<IElevatorSimulationService, ElevatorSimulationService>();

                     })
                     .UseSerilog((hostingContext, services, loggerConfiguration) =>
                     {
                         loggerConfiguration
                             .MinimumLevel.Information()
                             .WriteTo.Console()
                             .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day);
                     })
                     .Build();

                var simulationService = host.Services.GetRequiredService<IElevatorSimulationService>();

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
