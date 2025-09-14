# Elevator Simulation Console Application

A comprehensive C# console application that simulates the movement of elevators within a large building, designed to optimize passenger 
transportation efficiently.


## Features

### Core Functionality
- **Real-Time Elevator Status**: Displays current floor, direction, movement state, and passenger count for each elevator
- **Interactive Elevator Control**: Users can call elevators to specific floors and specify passenger counts
- **Multiple Floors and Elevators Support**: Configurable building with multiple floors and elevators
- **Efficient Elevator Dispatching**: Smart algorithm that directs the nearest available elevator to respond to requests
- **Passenger Limit Handling**: Prevents elevator overloading and manages capacity constraints
- **Real-Time Operation**: Immediate responses to user interactions with accurate elevator movement simulation


## Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- Windows, macOS, or Linux operating system


### Usage

1. **Starting the Simulation**
   - The application starts with a 10-floor building containing 3 elevators (capacity: 8 passengers each)
   - Initial passengers are automatically created for demonstration

2. **Interactive Commands**
   - **[R]**: Request an elevator
     - Enter origin floor (1-10)
     - Enter destination floor (1-10)  
     - Enter number of passengers (1-8)
   - **[Q]**: Quit the simulation
   - **[Enter]**: Continue simulation without input

3. **Real-Time Display**
   - Elevator status showing current floor, direction, state, and passenger count
   - Destination floors for each elevator
   - Waiting passengers on each floor
   - Real-time updates every second

