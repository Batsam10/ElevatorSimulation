namespace ElevatorSimulation
{
    public class ElevatorSimulation
    {
        private Building building;
        private bool isRunning;
        private readonly int _floors;
        private readonly int _elevators;
        private readonly int _capacity;

        public ElevatorSimulation(int floors, int elevators, int capacity)
        {
            building = new Building(floors, elevators, capacity);
            isRunning = true;
            _floors = floors;
            _elevators = elevators;
            _capacity = capacity;
        }

        public void Run()
        {
            Console.WriteLine("=== ELEVATOR SIMULATION STARTED ===");
            Console.WriteLine($"Building: {_floors} floors, {_elevators} elevators, capacity {_capacity} each");
            Console.WriteLine();

            // Add some initial passengers for demonstration
            building.RequestElevator(1, 5, 2);
            building.RequestElevator(3, 8, 1);
            building.RequestElevator(7, 2, 3);

            while (isRunning)
            {
                building.DisplayStatus();

                // Check for user input (non-blocking)
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    HandleUserInput(key);
                }

                // Update simulation
                building.Update();

                // Wait before next update
                Thread.Sleep(1000);
            }
        }

        private void HandleUserInput(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.R:
                    RequestElevatorFromUser();
                    break;
                case ConsoleKey.Q:
                    isRunning = false;
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

                building.RequestElevator(fromFloor, toFloor, passengers);
                Console.WriteLine("Elevator requested successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
