using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Types;

namespace ElevatorSimulation.Tests
{
    public class ElevatorTests
    {
        [Fact]
        public void Constructor_ShouldInitializeElevatorCorrectly()
        {
            // Arrange & Act
            var elevator = new Elevator(1, 8, 5);

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
            var elevator = new Elevator(1, 8);

            // Assert
            Assert.Equal(1, elevator.CurrentFloor);
        }

        [Fact]
        public void CanAddPassenger_ShouldReturnTrue_WhenElevatorNotFull()
        {
            // Arrange
            var elevator = new Elevator(1, 2);
            var passenger = new Passenger(1, 1, 5);

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
            var elevator = new Elevator(1, 1);
            var passenger = new Passenger(1, 1, 5);

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
            var elevator = new Elevator(1, 8, 1);
            var passenger = new Passenger(1, 1, 5);

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
            var elevator = new Elevator(1, 1, 1);
            var passenger1 = new Passenger(1, 1, 5);
            var passenger2 = new Passenger(2, 1, 6);

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
            var elevator = new Elevator(1, 8, 1);
            var passenger = new Passenger(1, 1, 5);
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
            var elevator = new Elevator(1, 8, 1);

            // Act
            elevator.GoToFloor(5);

            // Assert
            Assert.Contains(5, elevator.DestinationFloors);
        }

        [Fact]
        public void GoToFloor_ShouldNotAddDuplicateDestination()
        {
            // Arrange
            var elevator = new Elevator(1, 8, 1);

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
            var elevator = new Elevator(1, 8, 5);

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
            var elevator = new Elevator(1, 8, 1);
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
            var elevator = new Elevator(1, 8, 5);
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
            var elevator = new Elevator(1, 8, 4);
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
    }
}
