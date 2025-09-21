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

        Passenger(int id, int originFloor, int destinationFloor)
        {
            Id = id;
            OriginFloor = originFloor;
            DestinationFloor = destinationFloor;
            State = PassengerState.Waiting;
        }

        public static Passenger CreatePassenger(int id, int originFloor, int destinationFloor)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(originFloor);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(destinationFloor);
            return new Passenger(id, originFloor, destinationFloor);
        }   

        public override string ToString()
        {
            return $"Passenger {Id}: {OriginFloor} -> {DestinationFloor} ({State})";
        }
    }
}
