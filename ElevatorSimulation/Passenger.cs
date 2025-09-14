namespace ElevatorSimulation
{
    public class Passenger
    {
        public int Id { get; set; }
        public int OriginFloor { get; set; }
        public int DestinationFloor { get; set; }
        public PassengerState State { get; set; }

        public Passenger(int id, int originFloor, int destinationFloor)
        {
            Id = id;
            OriginFloor = originFloor;
            DestinationFloor = destinationFloor;
            State = PassengerState.Waiting;
        }

        public override string ToString()
        {
            return $"Passenger {Id}: {OriginFloor} -> {DestinationFloor} ({State})";
        }
    }
}
