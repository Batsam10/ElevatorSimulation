using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Types;

namespace ElevatorSimulation.Tests
{
    /// <summary>
    /// Unit tests for the Building class functionality within the DDD structure
    /// </summary>
    public class BuildingTests
    {
        [Fact]
        public void Constructor_ShouldInitializeBuildingCorrectly()
        {
            // Arrange & Act
            var building = Building.CreateBuilding(10, 3, 8);

            // Assert
            Assert.Equal(10, building.Floors.Count);
            Assert.Equal(3, building.Elevators.Count);
            
            // Check floors are numbered correctly
            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(i + 1, building.Floors[i].FloorNumber);
            }

            // Check elevators are initialized correctly
            for (int i = 0; i < 3; i++)
            {
                Assert.Equal(i + 1, building.Elevators[i].Id);
                Assert.Equal(8, building.Elevators[i].Capacity);
                Assert.Equal(1, building.Elevators[i].CurrentFloor);
            }
        }

        [Fact]
        public void RequestElevator_ShouldCreatePassengersAndDispatchElevator()
        {
            // Arrange
            var building = Building.CreateBuilding(10, 2, 8);

            // Act
            building.RequestElevator(3, 7, 2);

            // Assert
            var floor3 = building.Floors[2]; // Floor 3 (0-indexed)
            Assert.Equal(2, floor3.WaitingPassengers.Count);
            
            // Check passengers are created correctly
            foreach (var passenger in floor3.WaitingPassengers)
            {
                Assert.Equal(3, passenger.OriginFloor);
                Assert.Equal(7, passenger.DestinationFloor);
                Assert.Equal(PassengerState.Waiting, passenger.State);
            }

            // Check that at least one elevator has floor 3 as destination
            Assert.True(building.Elevators.Any(e => e.DestinationFloors.Contains(3)));
        }

        [Fact]
        public void RequestElevator_ShouldRejectInvalidFloorNumbers()
        {
            // Arrange
            var building = Building.CreateBuilding(5, 1, 8);

            // Act & Assert - Invalid origin floor
            building.RequestElevator(0, 3, 1);
            building.RequestElevator(6, 3, 1);
            building.RequestElevator(3, 0, 1);
            building.RequestElevator(3, 6, 1);

            // All floors should be empty since requests were invalid
            Assert.True(building.Floors.All(f => f.WaitingPassengers.Count == 0));
        }

        [Fact]
        public void RequestElevator_ShouldRejectSameOriginAndDestination()
        {
            // Arrange
            var building = Building.CreateBuilding(5, 1, 8);

            // Act
            building.RequestElevator(3, 3, 1);

            // Assert
            Assert.True(building.Floors.All(f => f.WaitingPassengers.Count == 0));
        }

        [Fact]
        public void Update_ShouldMoveElevatorsAndHandlePassengers()
        {
            // Arrange
            var building = Building.CreateBuilding(10, 1, 8);
            building.RequestElevator(1, 5, 1);

            // Act - Multiple updates to simulate elevator movement
            for (int i = 0; i < 10; i++)
            {
                building.Update();
            }

            // Assert
            var elevator = building.Elevators.First();
            var floor1 = building.Floors[0];
            
            // Either passenger should be picked up or elevator should be moving toward pickup
            Assert.True(floor1.WaitingPassengers.Count == 0 || 
                       elevator.DestinationFloors.Contains(1) ||
                       elevator.Passengers.Any());
        }

        [Theory]
        [InlineData(1, 5, Direction.Up)]
        [InlineData(5, 1, Direction.Down)]
        public void RequestElevator_ShouldSetCorrectDirection(int origin, int destination, Direction expectedDirection)
        {
            // Arrange
            var building = Building.CreateBuilding(10, 1, 8);

            // Act
            building.RequestElevator(origin, destination, 1);

            // Assert
            var passenger = building.Floors[origin - 1].WaitingPassengers.First();
            Assert.Equal(destination, passenger.DestinationFloor);
            
            // Check that floor button is pressed correctly
            var floor = building.Floors[origin - 1];
            if (expectedDirection == Direction.Up)
                Assert.True(floor.UpButtonPressed);
            else
                Assert.True(floor.DownButtonPressed);
        }

        [Fact]
        public void DispatchElevator_ShouldChooseClosestElevator()
        {
            // Arrange
            Building building = Building.CreateBuilding(10, 3, 8);
            
            // Position elevators at different floors using GoToFloor and Update
            building.Elevators[0].GoToFloor(1);
            building.Elevators[1].GoToFloor(5);
            building.Elevators[2].GoToFloor(10);

            // Simulate movement until elevators reach their initial positions
            for (int i = 0; i < 10; i++)
            {
                building.Update();
            }

            // Ensure elevators are at their intended starting floors for the test
            Assert.Equal(1, building.Elevators[0].CurrentFloor);
            Assert.Equal(5, building.Elevators[1].CurrentFloor);
            Assert.Equal(10, building.Elevators[2].CurrentFloor);

            // Act - Request elevator to floor 6
            building.RequestElevator(6, 8, 1);

            // Assert - Elevator 2 (at floor 5) should be closest to floor 6
            Assert.Contains(6, building.Elevators[1].DestinationFloors);
        }

        [Fact]
        public void Update_ShouldHandleMultipleElevatorsSimultaneously()
        {
            // Arrange
            var building = Building.CreateBuilding(10, 3, 8);
            
            // Create requests for different elevators
            building.RequestElevator(1, 5, 1);
            building.RequestElevator(3, 8, 1);
            building.RequestElevator(7, 2, 1);

            // Act
            for (int i = 0; i < 15; i++)
            {
                building.Update();
            }

            // Assert - At least one elevator should have destinations or be active
            Assert.True(building.Elevators.Any(e => 
                e.DestinationFloors.Any() || 
                e.Passengers.Any() || 
                e.Direction != Direction.Stationary));
        }

        [Fact]
        public void Building_ShouldHandleCapacityLimits()
        {
            // Arrange
            var building = Building.CreateBuilding(5, 1, 2); // Small capacity elevator

            // Act - Request more passengers than capacity
            building.RequestElevator(1, 3, 5);

            // Simulate pickup
            for (int i = 0; i < 10; i++)
            {
                building.Update();
            }

            // Assert - Some passengers should still be waiting
            var elevator = building.Elevators.First();
            var floor1 = building.Floors[0];
            
            Assert.True(elevator.Passengers.Count <= 2); // Capacity limit
            Assert.True(floor1.WaitingPassengers.Count + elevator.Passengers.Count <= 5); // Total passengers
        }
    }
}

