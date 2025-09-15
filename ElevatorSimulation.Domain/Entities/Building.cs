using ElevatorSimulation.Domain.Interfaces;
using ElevatorSimulation.Domain.Types;

namespace ElevatorSimulation.Domain.Entities
{
    public class Building : IBuilding
    {
        public List<IFloor> Floors { get; private set; }
        public List<IElevator> Elevators { get; private set; }
        private int passengerIdCounter = 1;

        IReadOnlyList<IFloor> IBuilding.Floors => Floors.AsReadOnly();
        IReadOnlyList<IElevator> IBuilding.Elevators => Elevators.AsReadOnly();

        public Building(int numberOfFloors, int numberOfElevators, int elevatorCapacity)
        {
            Floors = new List<IFloor>();
            Elevators = new List<IElevator>();

            for (int i = 1; i <= numberOfFloors; i++)
            {
                Floors.Add(new Floor(i));
            }

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

            for (int i = 0; i < numberOfPassengers; i++)
            {
                var passenger = new Passenger(passengerIdCounter++, floor, destinationFloor);
                Floors[floor - 1].AddPassenger(passenger);
            }

            Direction requestDirection = destinationFloor > floor ? Direction.Up : Direction.Down;
            DispatchElevator(floor, requestDirection);
        }

        private void DispatchElevator(int floor, Direction direction)
        {
            IElevator bestElevator = null;
            int minDistance = int.MaxValue;

            foreach (var elevator in Elevators)
            {
                int distance = Math.Abs(elevator.CurrentFloor - floor);
                
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

            if (bestElevator == null)
            {
                bestElevator = Elevators.OrderBy(e => Math.Abs(e.CurrentFloor - floor)).First();
            }

            bestElevator.GoToFloor(floor);
        }

        public void Update()
        {
            foreach (var elevator in Elevators)
            {
                elevator.Move();

                if (elevator.State == ElevatorState.DoorsOpen)
                {
                    HandlePassengerExchange(elevator);
                    // After handling, close doors (state change handled internally by elevator)
                }
            }
        }

        private void HandlePassengerExchange(IElevator elevator)
        {
            var currentFloor = Floors[elevator.CurrentFloor - 1];

            var passengersToDropOff = elevator.Passengers.Where(p => p.DestinationFloor == elevator.CurrentFloor).ToList();
            foreach (var passenger in passengersToDropOff)
            {
                elevator.RemovePassenger(passenger);
                Console.WriteLine($"Passenger {passenger.Id} arrived at floor {elevator.CurrentFloor}");
            }

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
