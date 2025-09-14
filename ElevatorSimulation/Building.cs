namespace ElevatorSimulation
{
    public class Building
    {
        public List<Floor> Floors { get; set; } = new();
        public List<Elevator> Elevators { get; set; } = new();
        private int passengerIdCounter = 1;

        public Building(int numberOfFloors, int numberOfElevators, int elevatorCapacity)
        {
            // Initialize floors
            for (int i = 1; i <= numberOfFloors; i++)
            {
                Floors.Add(new Floor(i));
            }

            // Initialize elevators
            for (int i = 1; i <= numberOfElevators; i++)
            {
                Elevators.Add(new Elevator(i, elevatorCapacity));
            }
        }

        public void RequestElevator(int floor, int destinationFloor, int numberOfPassengers = 1)
        {
            if (floor < 1 || floor > Floors.Count || destinationFloor < 1 || destinationFloor > Floors.Count)
            {
                Console.WriteLine("Invalid floor number!");
                return;
            }

            if (floor == destinationFloor)
            {
                Console.WriteLine("Origin and destination floors cannot be the same!");
                return;
            }

            // Create passengers
            for (int i = 0; i < numberOfPassengers; i++)
            {
                var passenger = new Passenger(passengerIdCounter++, floor, destinationFloor);
                Floors[floor - 1].AddPassenger(passenger);
            }

            // Dispatch elevator
            Direction requestDirection = destinationFloor > floor ? Direction.Up : Direction.Down;
            DispatchElevator(floor, requestDirection);
        }

        private void DispatchElevator(int floor, Direction direction)
        {
            // Find the best elevator to dispatch
            Elevator bestElevator = null;
            int minDistance = int.MaxValue;

            foreach (var elevator in Elevators)
            {
                int distance = Math.Abs(elevator.CurrentFloor - floor);

                // Prefer elevators that are stationary or moving in the same direction
                bool isGoodCandidate = elevator.Direction == Direction.Stationary ||
                                      (elevator.Direction == direction &&
                                       ((direction == Direction.Up && elevator.CurrentFloor <= floor) ||
                                        (direction == Direction.Down && elevator.CurrentFloor >= floor)));

                if (isGoodCandidate && distance < minDistance)
                {
                    minDistance = distance;
                    bestElevator = elevator;
                }
            }

            // If no good candidate found, use the closest elevator
            if (bestElevator == null)
            {
                bestElevator = Elevators.OrderBy(e => Math.Abs(e.CurrentFloor - floor)).First();
            }

            bestElevator.GoToFloor(floor);
        }

        public void Update()
        {
            // Update all elevators
            foreach (var elevator in Elevators)
            {
                elevator.Move();

                // Handle passenger pickup and drop-off
                if (elevator.State == ElevatorState.DoorsOpen)
                {
                    HandlePassengerExchange(elevator);
                    elevator.State = ElevatorState.DoorsClosed;
                }
            }
        }

        private void HandlePassengerExchange(Elevator elevator)
        {
            var currentFloor = Floors[elevator.CurrentFloor - 1];

            // Drop off passengers
            var passengersToDropOff = elevator.Passengers.Where(p => p.DestinationFloor == elevator.CurrentFloor).ToList();
            foreach (var passenger in passengersToDropOff)
            {
                elevator.RemovePassenger(passenger);
                Console.WriteLine($"Passenger {passenger.Id} arrived at floor {elevator.CurrentFloor}");
            }

            // Pick up passengers
            var passengersToPickUp = currentFloor.WaitingPassengers.Where(p =>
                elevator.CanAddPassenger() &&
                ((p.DestinationFloor > elevator.CurrentFloor && elevator.Direction != Direction.Down) ||
                 (p.DestinationFloor < elevator.CurrentFloor && elevator.Direction != Direction.Up) ||
                 elevator.Direction == Direction.Stationary)).ToList();

            foreach (var passenger in passengersToPickUp)
            {
                elevator.AddPassenger(passenger);
                currentFloor.RemovePassenger(passenger);
                Console.WriteLine($"Passenger {passenger.Id} boarded elevator {elevator.Id} at floor {elevator.CurrentFloor}");
            }

            // Reset floor buttons if no more passengers waiting
            if (!currentFloor.WaitingPassengers.Any(p => p.DestinationFloor > elevator.CurrentFloor))
                currentFloor.UpButtonPressed = false;
            if (!currentFloor.WaitingPassengers.Any(p => p.DestinationFloor < elevator.CurrentFloor))
                currentFloor.DownButtonPressed = false;
        }

        public void DisplayStatus()
        {
            Console.Clear();
            Console.WriteLine("=== ELEVATOR SYSTEM STATUS ===");
            Console.WriteLine($"Time: {DateTime.Now:HH:mm:ss}");
            Console.WriteLine();

            // Display elevators
            Console.WriteLine("ELEVATORS:");
            foreach (var elevator in Elevators)
            {
                Console.WriteLine($"  {elevator}");
                if (elevator.DestinationFloors.Any())
                {
                    Console.WriteLine($"    Destinations: [{string.Join(", ", elevator.DestinationFloors)}]");
                }
            }

            Console.WriteLine();

            // Display floors with waiting passengers
            Console.WriteLine("FLOORS WITH WAITING PASSENGERS:");
            foreach (var floor in Floors.Where(f => f.WaitingPassengers.Any()))
            {
                Console.WriteLine($"  Floor {floor.FloorNumber}: {floor.WaitingPassengers.Count} waiting");
                foreach (var passenger in floor.WaitingPassengers)
                {
                    Console.WriteLine($"    {passenger}");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Commands: [R]equest elevator, [Q]uit, [Enter] to continue simulation");
        }
    }
}
