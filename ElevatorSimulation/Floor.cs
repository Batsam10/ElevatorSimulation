namespace ElevatorSimulation
{
    public class Floor
    {
        public int FloorNumber { get; set; }
        public List<Passenger> WaitingPassengers { get; set; }
        public bool UpButtonPressed { get; set; }
        public bool DownButtonPressed { get; set; }

        public Floor(int floorNumber)
        {
            FloorNumber = floorNumber;
            WaitingPassengers = new List<Passenger>();
            UpButtonPressed = false;
            DownButtonPressed = false;
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
