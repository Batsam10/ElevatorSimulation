using ElevatorSimulation.Domain.Types;

namespace ElevatorSimulation.Domain.Entities
{
    public class Floor
    {
        public int FloorNumber { get; }
        public List<Passenger> WaitingPassengers { get; private set; }
        public bool UpButtonPressed { get; set; }
        public bool DownButtonPressed { get; set; }

        Floor(int floorNumber)
        {
            FloorNumber = floorNumber;
            WaitingPassengers = new List<Passenger>();
            UpButtonPressed = false;
            DownButtonPressed = false;
        }

        public static Floor CreateFloor(int floorNumber)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(floorNumber);
            return new Floor(floorNumber);
        }

        public void CallElevator(Direction direction)
        {
            if (direction == Direction.Up)
                UpButtonPressed = true;
            else if (direction == Direction.Down)
                DownButtonPressed = true;
        }

        public void AddPassenger(Passenger passenger)
        {
            WaitingPassengers.Add(passenger);
            // Automatically press the appropriate button
            if (passenger.DestinationFloor > FloorNumber)
                CallElevator(Direction.Up);
            else if (passenger.DestinationFloor < FloorNumber)
                CallElevator(Direction.Down);
        }

        public void RemovePassenger(Passenger passenger)
        {
            WaitingPassengers.Remove(passenger);
        }
    }
}