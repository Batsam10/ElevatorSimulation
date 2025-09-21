using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Interfaces;
using ElevatorSimulation.Domain.Types;

namespace ElevatorSimulation.Tests
{
    public class FloorTests
    {
        [Fact]
        public void Floor_Constructor_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            IFloor floor = Floor.CreateFloor(5);

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
            IFloor floor = Floor.CreateFloor(3);

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
            IFloor floor = Floor.CreateFloor(3);

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
            IFloor floor = Floor.CreateFloor(3);

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
            IFloor floor = Floor.CreateFloor(3);
            IPassenger passenger = Passenger.CreatePassenger(1, 3, 7);

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
            IFloor floor = Floor.CreateFloor(3);
            IPassenger passenger = Passenger.CreatePassenger(1, 3, 7); // Going up

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
            IFloor floor = Floor.CreateFloor(5);
            IPassenger passenger = Passenger.CreatePassenger(1, 5, 2); // Going down

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
            IFloor floor = Floor.CreateFloor(3);
            IPassenger passenger = Passenger.CreatePassenger(1, 3, 3); // Same floor (shouldn't happen in practice)

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
            IFloor floor = Floor.CreateFloor(3);
            IPassenger passenger1 = Passenger.CreatePassenger(1, 3, 7);
            IPassenger passenger2 = Passenger.CreatePassenger(2, 3, 8);

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
            IFloor floor = Floor.CreateFloor(1);

            // Act
            for (int i = 1; i <= 5; i++)
            {
                IPassenger passenger = Passenger.CreatePassenger(i, 1, 10);
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
            IFloor floor = Floor.CreateFloor(5);
            IPassenger passengerUp = Passenger.CreatePassenger(1, 5, 8);
            IPassenger passengerDown = Passenger.CreatePassenger(2, 5, 2);

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
