using ElevatorSimulation.Domain.Interfaces;
using ElevatorSimulation.Domain.Types;

namespace ElevatorSimulation.Domain.Entities
{
    public class Elevator : IElevator
    {
        public int Id { get; }
        public int CurrentFloor { get; private set; }
        public Direction Direction { get; private set; }
        public ElevatorState State { get; private set; }
        public int Capacity { get; }
        public List<IPassenger> Passengers { get; private set; }
        public List<int> DestinationFloors { get; private set; }

        IReadOnlyList<IPassenger> IElevator.Passengers => Passengers.AsReadOnly();
        IReadOnlyList<int> IElevator.DestinationFloors => DestinationFloors.AsReadOnly();

        public Elevator(int id, int capacity, int startingFloor = 1)
        {
            Id = id;
            Capacity = capacity;
            CurrentFloor = startingFloor;
            Direction = Direction.Stationary;
            State = ElevatorState.Stopped;
            Passengers = new List<IPassenger>();
            DestinationFloors = new List<int>();
        }

        public bool CanAddPassenger()
        {
            return Passengers.Count < Capacity;
        }

        public void AddPassenger(IPassenger passenger)
        {
            if (CanAddPassenger())
            {
                Passengers.Add(passenger);
                passenger.State = PassengerState.InElevator;
                
                if (!DestinationFloors.Contains(passenger.DestinationFloor))
                {
                    DestinationFloors.Add(passenger.DestinationFloor);
                    DestinationFloors.Sort();
                }
            }
        }

        public void RemovePassenger(IPassenger passenger)
        {
            Passengers.Remove(passenger);
            passenger.State = PassengerState.Arrived;
        }

        public void GoToFloor(int floor)
        {
            if (!DestinationFloors.Contains(floor))
            {
                DestinationFloors.Add(floor);
                DestinationFloors.Sort();
            }
        }

        public void Move()
        {
            if (DestinationFloors.Count == 0)
            {
                Direction = Direction.Stationary;
                State = ElevatorState.Stopped;
                return;
            }

            int nextFloor = GetNextDestination();
            
            if (nextFloor == CurrentFloor)
            {
                DestinationFloors.Remove(CurrentFloor);
                State = ElevatorState.DoorsOpen;
                Direction = Direction.Stationary;
            }
            else
            {
                State = ElevatorState.Moving;
                if (nextFloor > CurrentFloor)
                {
                    Direction = Direction.Up;
                    CurrentFloor++;
                }
                else
                {
                    Direction = Direction.Down;
                    CurrentFloor--;
                }
            }
        }

        private int GetNextDestination()
        {
            if (DestinationFloors.Count == 0)
                return CurrentFloor;

            if (Direction == Direction.Up || Direction == Direction.Stationary)
            {
                var upFloors = DestinationFloors.Where(f => f >= CurrentFloor).OrderBy(f => f);
                if (upFloors.Any())
                    return upFloors.First();
            }
            
            if (Direction == Direction.Down || Direction == Direction.Stationary)
            {
                var downFloors = DestinationFloors.Where(f => f <= CurrentFloor).OrderByDescending(f => f);
                if (downFloors.Any())
                    return downFloors.First();
            }

            return DestinationFloors.OrderBy(f => Math.Abs(f - CurrentFloor)).First();
        }

        public override string ToString()
        {
            string directionSymbol = Direction == Direction.Up ? "↑" :
                                   Direction == Direction.Down ? "↓" : "•";
            return $"Elevator {Id}: Floor {CurrentFloor} {directionSymbol} ({State}) - Passengers: {Passengers.Count}/{Capacity}";
        }
    }
}
