namespace ElevatorSimulation
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Console.WriteLine($"Create a building");
                Console.WriteLine($"Enter the number of floors");

                string? numberOfFloorsStr = Console.ReadLine();
                if (!int.TryParse(numberOfFloorsStr, out int numberOfFloors) || numberOfFloors < 1)
                {
                    Console.WriteLine("Invalid number of floors. Please enter a positive integer.");
                    return;
                }

                Console.WriteLine($"Enter the number of elevators");
                string? numberOfElevatorsStr = Console.ReadLine();
                if (!int.TryParse(numberOfElevatorsStr, out int numberOfElevators) || numberOfElevators < 1)
                {
                    Console.WriteLine("Invalid number of elevators. Please enter a positive integer.");
                    return;
                }

                Console.WriteLine($"Enter the capacity per elevator - SAME FOR ALL");
                if (!int.TryParse(Console.ReadLine(), out int capacityPerElevator) || capacityPerElevator < 1)
                {
                    Console.WriteLine("Invalid capacity. Please enter a positive integer.");
                    return;
                }

                var simulation = new ElevatorSimulation(numberOfFloors, numberOfElevators, capacityPerElevator);
                simulation.Run();
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