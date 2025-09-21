using ElevatorSimulation.Domain.Interfaces;
using ElevatorSimulation.Domain.Types;

namespace ElevatorSimulation.Domain.Entities
{
    public class Floor : IFloor
    {
        public int FloorNumber { get; }
        public List<IPassenger> WaitingPassengers { get; private set; }
        public bool UpButtonPressed { get; set; }
        public bool DownButtonPressed { get; set; }

        IReadOnlyList<IPassenger> IFloor.WaitingPassengers => WaitingPassengers.AsReadOnly();

        Floor(int floorNumber)
        {
            FloorNumber = floorNumber;
            WaitingPassengers = new List<IPassenger>();
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

        public void AddPassenger(IPassenger passenger)
        {
            WaitingPassengers.Add(passenger);
            // Automatically press the appropriate button
            if (passenger.DestinationFloor > FloorNumber)
                CallElevator(Direction.Up);
            else if (passenger.DestinationFloor < FloorNumber)
                CallElevator(Direction.Down);
        }

        public void RemovePassenger(IPassenger passenger)
        {
            WaitingPassengers.Remove(passenger);
        }
    }
}