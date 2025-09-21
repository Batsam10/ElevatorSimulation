using ElevatorSimulation.Domain.Interfaces;
using ElevatorSimulation.Domain.Types;
using System.Drawing;

namespace ElevatorSimulation.Domain.Entities
{
    public class Building : IBuilding
    {
        public List<IFloor> Floors { get; private set; }
        public List<IElevator> Elevators { get; private set; }
        private int passengerIdCounter = 1;

        IReadOnlyList<IFloor> IBuilding.Floors => Floors.AsReadOnly();
        IReadOnlyList<IElevator> IBuilding.Elevators => Elevators.AsReadOnly();

        private Building(int numberOfFloors, int numberOfElevators, int elevatorCapacity)
        {
            Floors = new List<IFloor>();
            Elevators = new List<IElevator>();

            for (int i = 1; i <= numberOfFloors; i++)
            {
                Floors.Add(Floor.CreateFloor(i));
            }

            for (int i = 1; i <= numberOfElevators; i++)
            {
                Elevators.Add(Elevator.CreateElevator(i, elevatorCapacity));
            }
        }


        public static Building CreateBuilding(int numberOfFloors, int numberOfElevators, int elevatorCapacity)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(numberOfFloors);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(numberOfElevators);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(elevatorCapacity);

            return new Building(numberOfFloors, numberOfElevators, elevatorCapacity);
        }

        public void RequestElevator(int floor, int destinationFloor, int numberOfPassengers = 1)
        {
            //ArgumentOutOfRangeException.ThrowIfGreaterThan(floor, Floors.Count);
            //ArgumentOutOfRangeException.ThrowIfLessThan(floor, 1);
            //ArgumentOutOfRangeException.ThrowIfGreaterThan(destinationFloor, Floors.Count);
            //ArgumentOutOfRangeException.ThrowIfLessThan(destinationFloor, 1);
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
                var passenger = Passenger.CreatePassenger(passengerIdCounter++, floor, destinationFloor);
                Floors[floor - 1].AddPassenger(passenger);
            }

            Direction requestDirection = destinationFloor > floor ? Direction.Up : Direction.Down;
            DispatchElevator(floor, requestDirection);
        }

        ///<summary>
        /// Efficient Elevator Dispatching.
        /// Choose the elevator that is closest to the requested floor and is either stationary or moving in the direction of the request.
        ///</summary>
        private void DispatchElevator(int floor, Direction direction)
        {
            IElevator bestElevator = null;
            int minDistance = int.MaxValue;

            foreach (var elevator in Elevators)
            {
                if (!elevator.CanAddPassenger())
                {
                    continue;
                }

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
                }
            }
        }

        /// <summary>
        /// Handles passenger exchange (boarding and alighting) when the elevator doors are open.
        /// </summary>
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
                if (!elevator.CanAddPassenger())
                {
                    break;
                }
                elevator.AddPassenger(passenger);
                currentFloor.RemovePassenger(passenger);
                Console.WriteLine($"Passenger {passenger.Id} boarded elevator {elevator.Id} at floor {elevator.CurrentFloor}");
            }

            // After pickup, if there are still waiting passengers, dispatch another elevator
            if (currentFloor.WaitingPassengers.Any())
            {
                if (currentFloor.WaitingPassengers.Any(p => p.DestinationFloor > currentFloor.FloorNumber))
                {
                    DispatchElevator(currentFloor.FloorNumber, Direction.Up);
                }

                if (currentFloor.WaitingPassengers.Any(p => p.DestinationFloor < currentFloor.FloorNumber))
                {
                    DispatchElevator(currentFloor.FloorNumber, Direction.Down);
                }
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
