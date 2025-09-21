using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Interfaces;
using ElevatorSimulation.Domain.Types;

namespace ElevatorSimulation.Tests
{
    public class PassengerTests
    {
        [Fact]
        public void Passenger_Constructor_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            IPassenger passenger = Passenger.CreatePassenger(1, 3, 7);

            // Assert
            Assert.Equal(1, passenger.Id);
            Assert.Equal(3, passenger.OriginFloor);
            Assert.Equal(7, passenger.DestinationFloor);
            Assert.Equal(PassengerState.Waiting, passenger.State);
        }

        [Fact]
        public void Passenger_ToString_ShouldReturnCorrectFormat()
        {
            // Arrange
            IPassenger passenger = Passenger.CreatePassenger(5, 2, 8);
            passenger.State = PassengerState.InElevator;

            // Act
            string result = passenger.ToString();

            // Assert
            Assert.Contains("Passenger 5", result);
            Assert.Contains("2 -> 8", result);
            Assert.Contains("InElevator", result);
        }

        [Theory]
        [InlineData(PassengerState.Waiting)]
        [InlineData(PassengerState.InElevator)]
        [InlineData(PassengerState.Arrived)]
        public void Passenger_State_ShouldBeSettable(PassengerState state)
        {
            // Arrange
            IPassenger passenger = Passenger.CreatePassenger(1, 1, 5);

            // Act
            passenger.State = state;

            // Assert
            Assert.Equal(state, passenger.State);
        }

    }
}
