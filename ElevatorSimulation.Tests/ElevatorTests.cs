using Xunit;
using System.Linq;
using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Interfaces;
using ElevatorSimulation.Domain.Types;

namespace ElevatorSimulation.TestsDDD
{
    /// <summary>
    /// Unit tests for the Elevator class functionality within the DDD structure
    /// </summary>
    public class ElevatorTests
    {
        [Fact]
        public void Constructor_ShouldInitializeElevatorCorrectly()
        {
            // Arrange & Act
            IElevator elevator = Elevator.CreateElevator(1, 8, 5);

            // Assert
            Assert.Equal(1, elevator.Id);
            Assert.Equal(8, elevator.Capacity);
            Assert.Equal(5, elevator.CurrentFloor);
            Assert.Equal(Direction.Stationary, elevator.Direction);
            Assert.Equal(ElevatorState.Stopped, elevator.State);
            Assert.Empty(elevator.Passengers);
            Assert.Empty(elevator.DestinationFloors);
        }

        [Fact]
        public void Constructor_ShouldUseDefaultStartingFloor()
        {
            // Arrange & Act
            IElevator elevator = Elevator.CreateElevator(1, 8);

            // Assert
            Assert.Equal(1, elevator.CurrentFloor);
        }

        [Fact]
        public void CanAddPassenger_ShouldReturnTrue_WhenElevatorNotFull()
        {
            // Arrange
            IElevator elevator = Elevator.CreateElevator(1, 2);
            IPassenger passenger = Passenger.CreatePassenger(1, 1, 5);

            // Act
            elevator.AddPassenger(passenger);
            bool canAdd = elevator.CanAddPassenger();

            // Assert
            Assert.True(canAdd);
        }

        [Fact]
        public void CanAddPassenger_ShouldReturnFalse_WhenElevatorFull()
        {
            // Arrange
            IElevator elevator = Elevator.CreateElevator(1, 1);
            IPassenger passenger = Passenger.CreatePassenger(1, 1, 5);

            // Act
            elevator.AddPassenger(passenger);
            bool canAdd = elevator.CanAddPassenger();

            // Assert
            Assert.False(canAdd);
        }

        [Fact]
        public void AddPassenger_ShouldAddPassengerAndDestination()
        {
            // Arrange
            IElevator elevator = Elevator.CreateElevator(1, 8, 1);
            IPassenger passenger = Passenger.CreatePassenger(1, 1, 5);

            // Act
            elevator.AddPassenger(passenger);

            // Assert
            Assert.Single(elevator.Passengers);
            Assert.Contains(passenger, elevator.Passengers);
            Assert.Equal(PassengerState.InElevator, passenger.State);
            Assert.Contains(5, elevator.DestinationFloors);
        }

        [Fact]
        public void AddPassenger_ShouldNotAddWhenFull()
        {
            // Arrange
            IElevator elevator = Elevator.CreateElevator(1, 1, 1);
            IPassenger passenger1 = Passenger.CreatePassenger(1, 1, 5);
            IPassenger passenger2 = Passenger.CreatePassenger(2, 1, 6);

            // Act
            elevator.AddPassenger(passenger1);
            elevator.AddPassenger(passenger2);

            // Assert
            Assert.Single(elevator.Passengers);
            Assert.Contains(passenger1, elevator.Passengers);
            Assert.DoesNotContain(passenger2, elevator.Passengers);
        }

        [Fact]
        public void RemovePassenger_ShouldRemovePassengerAndUpdateState()
        {
            // Arrange
            IElevator elevator = Elevator.CreateElevator(1, 8, 1);
            IPassenger passenger = Passenger.CreatePassenger(1, 1, 5);
            elevator.AddPassenger(passenger);

            // Act
            elevator.RemovePassenger(passenger);

            // Assert
            Assert.Empty(elevator.Passengers);
            Assert.Equal(PassengerState.Arrived, passenger.State);
        }

        [Fact]
        public void GoToFloor_ShouldAddDestinationFloor()
        {
            // Arrange
            IElevator elevator = Elevator.CreateElevator(1, 8, 1);

            // Act
            elevator.GoToFloor(5);

            // Assert
            Assert.Contains(5, elevator.DestinationFloors);
        }

        [Fact]
        public void GoToFloor_ShouldNotAddDuplicateDestination()
        {
            // Arrange
            IElevator elevator = Elevator.CreateElevator(1, 8, 1);

            // Act
            elevator.GoToFloor(5);
            elevator.GoToFloor(5);

            // Assert
            Assert.Single(elevator.DestinationFloors);
            Assert.Equal(5, elevator.DestinationFloors.First());
        }

        [Fact]
        public void Move_ShouldStayStationary_WhenNoDestinations()
        {
            // Arrange
            IElevator elevator = Elevator.CreateElevator(1, 8, 5);

            // Act
            elevator.Move();

            // Assert
            Assert.Equal(Direction.Stationary, elevator.Direction);
            Assert.Equal(ElevatorState.Stopped, elevator.State);
            Assert.Equal(5, elevator.CurrentFloor);
        }

        [Fact]
        public void Move_ShouldMoveUp_WhenDestinationAbove()
        {
            // Arrange
            IElevator elevator = Elevator.CreateElevator(1, 8, 1);
            elevator.GoToFloor(5);

            // Act
            elevator.Move();

            // Assert
            Assert.Equal(Direction.Up, elevator.Direction);
            Assert.Equal(ElevatorState.Moving, elevator.State);
            Assert.Equal(2, elevator.CurrentFloor);
        }

        [Fact]
        public void Move_ShouldMoveDown_WhenDestinationBelow()
        {
            // Arrange
            IElevator elevator = Elevator.CreateElevator(1, 8, 5);
            elevator.GoToFloor(1);

            // Act
            elevator.Move();

            // Assert
            Assert.Equal(Direction.Down, elevator.Direction);
            Assert.Equal(ElevatorState.Moving, elevator.State);
            Assert.Equal(4, elevator.CurrentFloor);
        }

        [Fact]
        public void Move_ShouldOpenDoors_WhenReachingDestination()
        {
            // Arrange
            IElevator elevator = Elevator.CreateElevator(1, 8, 4);
            elevator.GoToFloor(5);
            elevator.Move(); // Move to floor 5

            // Act
            elevator.Move(); // Should arrive at floor 5

            // Assert
            Assert.Equal(5, elevator.CurrentFloor);
            Assert.Equal(ElevatorState.DoorsOpen, elevator.State);
            Assert.Equal(Direction.Stationary, elevator.Direction);
            Assert.DoesNotContain(5, elevator.DestinationFloors);
        }

        [Fact]
        public void ToString_ShouldReturnCorrectFormat()
        {
            // Arrange
            IElevator elevator = Elevator.CreateElevator(1, 8, 5);
            // To test ToString, we need to simulate movement that changes state and direction
            elevator.GoToFloor(elevator.CurrentFloor + 1);
            elevator.Move(); // This will set Direction to Up and State to Moving

            // Act
            string result = elevator.ToString();

            // Assert
            Assert.Contains("Elevator 1", result);
            Assert.Contains("Floor 6", result);
            Assert.Contains("↑", result);
            Assert.Contains("Moving", result);
            Assert.Contains("0/8", result);
        }

        [Theory]
        [InlineData(Direction.Up, "↑")]
        [InlineData(Direction.Down, "↓")]
        [InlineData(Direction.Stationary, "•")]
        public void ToString_ShouldShowCorrectDirectionSymbol(Direction direction, string expectedSymbol)
        {
            // Arrange
            IElevator elevator = Elevator.CreateElevator(1, 8, 1);
            // To test ToString, we need to ensure the elevator is in a state that reflects the direction.
            // For this specific test, we'll create an elevator and make it move to induce the direction.
            // This is a bit more involved than directly setting, but adheres to the read-only property.
            if (direction == Direction.Up)
            {
                elevator.GoToFloor(elevator.CurrentFloor + 1);
                elevator.Move(); // Should set direction to Up
            }
            else if (direction == Direction.Down)
            {
                elevator.GoToFloor(elevator.CurrentFloor - 1);
                elevator.Move(); // Should set direction to Down
            }
            // If Stationary, it should already be stationary by default or after reaching a destination.

            // Act
            string result = elevator.ToString();

            // Assert
            Assert.Contains(expectedSymbol, result);
        }
    }
}

