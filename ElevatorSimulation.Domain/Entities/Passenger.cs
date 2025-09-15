using ElevatorSimulation.Domain.Interfaces;
using ElevatorSimulation.Domain.Types;

namespace ElevatorSimulation.Domain.Entities
{
    public class Passenger : IPassenger
    {
        public int Id { get; }
        public int OriginFloor { get; }
        public int DestinationFloor { get; }
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
