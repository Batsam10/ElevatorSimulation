using ElevatorSimulation.Domain.Types;

namespace ElevatorSimulation.Domain.Entities
{
    public class Building
    {
        public List<Floor> Floors { get; private set; }
        public List<Elevator> Elevators { get; private set; }
        private int passengerIdCounter = 1;

        private Building(int numberOfFloors, int numberOfElevators, int elevatorCapacity)
        {
            Floors = new List<Floor>();
            Elevators = new List<Elevator>();

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

        public (bool IsSuccess, string ErrorMessage) RequestElevator(int floor, int destinationFloor, int numberOfPassengers = 1)
        {
            if (floor < 1 || floor > Floors.Count || destinationFloor < 1 || destinationFloor > Floors.Count)
            {

                return (false, "Invalid floor number!");
            }

            if (floor == destinationFloor)
            {
                return (false, "Origin and destination floors cannot be the same!");
            }

            for (int i = 0; i < numberOfPassengers; i++)
            {
                var passenger = Passenger.CreatePassenger(passengerIdCounter++, floor, destinationFloor);
                Floors[floor - 1].AddPassenger(passenger);
            }

            Direction requestDirection = destinationFloor > floor ? Direction.Up : Direction.Down;
            DispatchElevator(floor, requestDirection);
            return (true, string.Empty);
        }

        ///<summary>
        /// Efficient Elevator Dispatching.
        /// Choose the elevator that is closest to the requested floor and is either stationary or moving in the direction of the request.
        ///</summary>
        private void DispatchElevator(int floor, Direction direction)
        {
            Elevator bestElevator = null;
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

        public (List<(Passenger, Elevator)> Arrived, List<(Passenger, Elevator)> Boarded) Update()
        {
            var arrived = new List<(Passenger, Elevator)>();
            var boarded = new List<(Passenger, Elevator)>();

            foreach (var elevator in Elevators)
            {
                elevator.Move();

                if (elevator.State == ElevatorState.DoorsOpen)
                {
                    var details = HandlePassengerExchange(elevator);
                    arrived.AddRange(details.Arrived);
                    boarded.AddRange(details.Boarded);
                }
            }

            return (arrived, boarded);
        }

        /// <summary>
        /// Handles passenger exchange (boarding and alighting) when the elevator doors are open.
        /// </summary>
        private (List<(Passenger, Elevator)> Arrived, List<(Passenger, Elevator)> Boarded) HandlePassengerExchange(Elevator elevator)
        {
            var currentFloor = Floors[elevator.CurrentFloor - 1];
            var arrived = new List<(Passenger, Elevator)>();
            var boarded = new List<(Passenger, Elevator)>();

            var passengersToDropOff = elevator.Passengers.Where(p => p.DestinationFloor == elevator.CurrentFloor).ToList();
            foreach (var passenger in passengersToDropOff)
            {
                elevator.RemovePassenger(passenger);
                arrived.Add((passenger, elevator));
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
                boarded.Add((passenger, elevator));
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

            return (arrived, boarded);
        }
    }
}
