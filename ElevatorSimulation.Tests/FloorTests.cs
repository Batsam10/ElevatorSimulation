using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Types;

namespace ElevatorSimulation.Tests
{
    public class FloorTests
    {

        [Fact]
        public void Floor_Constructor_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            var floor = new Floor(5);

            // Assert
            Assert.Equal(5, floor.FloorNumber);
            Assert.Empty(floor.WaitingPassengers);
            Assert.False(floor.UpButtonPressed);
            Assert.False(floor.DownButtonPressed);
        }

        [Fact]
        public void Floor_CallElevator_ShouldSetUpButton()
        {
            // Arrange
            var floor = new Floor(3);

            // Act
            floor.CallElevator(Direction.Up);

            // Assert
            Assert.True(floor.UpButtonPressed);
            Assert.False(floor.DownButtonPressed);
        }

        [Fact]
        public void Floor_CallElevator_ShouldSetDownButton()
        {
            // Arrange
            var floor = new Floor(3);

            // Act
            floor.CallElevator(Direction.Down);

            // Assert
            Assert.False(floor.UpButtonPressed);
            Assert.True(floor.DownButtonPressed);
        }

        [Fact]
        public void Floor_CallElevator_ShouldIgnoreStationary()
        {
            // Arrange
            var floor = new Floor(3);

            // Act
            floor.CallElevator(Direction.Stationary);

            // Assert
            Assert.False(floor.UpButtonPressed);
            Assert.False(floor.DownButtonPressed);
        }

        [Fact]
        public void Floor_AddPassenger_ShouldAddToWaitingList()
        {
            // Arrange
            var floor = new Floor(3);
            var passenger = new Passenger(1, 3, 7);

            // Act
            floor.AddPassenger(passenger);

            // Assert
            Assert.Single(floor.WaitingPassengers);
            Assert.Contains(passenger, floor.WaitingPassengers);
        }

        [Fact]
        public void Floor_AddPassenger_ShouldCallElevatorUp_WhenDestinationAbove()
        {
            // Arrange
            var floor = new Floor(3);
            var passenger = new Passenger(1, 3, 7); // Going up

            // Act
            floor.AddPassenger(passenger);

            // Assert
            Assert.True(floor.UpButtonPressed);
            Assert.False(floor.DownButtonPressed);
        }

        [Fact]
        public void Floor_AddPassenger_ShouldCallElevatorDown_WhenDestinationBelow()
        {
            // Arrange
            var floor = new Floor(5);
            var passenger = new Passenger(1, 5, 2); // Going down

            // Act
            floor.AddPassenger(passenger);

            // Assert
            Assert.False(floor.UpButtonPressed);
            Assert.True(floor.DownButtonPressed);
        }

        [Fact]
        public void Floor_AddPassenger_ShouldNotCallElevator_WhenSameFloor()
        {
            // Arrange
            var floor = new Floor(3);
            var passenger = new Passenger(1, 3, 3); // Same floor (shouldn't happen in practice)

            // Act
            floor.AddPassenger(passenger);

            // Assert
            Assert.False(floor.UpButtonPressed);
            Assert.False(floor.DownButtonPressed);
        }

        [Fact]
        public void Floor_RemovePassenger_ShouldRemoveFromWaitingList()
        {
            // Arrange
            var floor = new Floor(3);
            var passenger1 = new Passenger(1, 3, 7);
            var passenger2 = new Passenger(2, 3, 8);

            floor.AddPassenger(passenger1);
            floor.AddPassenger(passenger2);

            // Act
            floor.RemovePassenger(passenger1);

            // Assert
            Assert.Single(floor.WaitingPassengers);
            Assert.DoesNotContain(passenger1, floor.WaitingPassengers);
            Assert.Contains(passenger2, floor.WaitingPassengers);
        }

        [Fact]
        public void Floor_AddMultiplePassengers_ShouldMaintainCorrectCount()
        {
            // Arrange
            var floor = new Floor(1);

            // Act
            for (int i = 1; i <= 5; i++)
            {
                var passenger = new Passenger(i, 1, 10);
                floor.AddPassenger(passenger);
            }

            // Assert
            Assert.Equal(5, floor.WaitingPassengers.Count);
            Assert.True(floor.UpButtonPressed); // All going up
        }

        [Fact]
        public void Floor_MixedDirectionPassengers_ShouldSetBothButtons()
        {
            // Arrange
            var floor = new Floor(5);
            var passengerUp = new Passenger(1, 5, 8);
            var passengerDown = new Passenger(2, 5, 2);

            // Act
            floor.AddPassenger(passengerUp);
            floor.AddPassenger(passengerDown);

            // Assert
            Assert.True(floor.UpButtonPressed);
            Assert.True(floor.DownButtonPressed);
            Assert.Equal(2, floor.WaitingPassengers.Count);
        }

    }
}
