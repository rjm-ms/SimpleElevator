using SimpleElevator.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SimpleElevator.UnitTests
{
    public class ElevatorSingletonTests
    {
        [Fact]
        public void GetInstance_ShouldReturnSameInstance()
        {
            var elevator1 = ElevatorSingleton.GetInstance(ElevatorCar.One);
            var elevator2 = ElevatorSingleton.GetInstance(ElevatorCar.One);

            Assert.Same(elevator1, elevator2);
        }

        [Fact]
        public void AddPickupFloor_ShouldAddPickupFloor()
        {
            var elevator = ElevatorSingleton.GetInstance(ElevatorCar.One);
            var requestId = Guid.NewGuid();
            elevator.AddPickupFloor(requestId, 3, Direction.Up);

            Assert.Single(elevator.PickupFloors);
            Assert.Equal(3, elevator.PickupFloors.First().Floor);
        }

        [Fact]
        public void AddDestinationFloor_ShouldAddDestinationFloor()
        {
            var elevator = ElevatorSingleton.GetInstance(ElevatorCar.One);
            var requestId = Guid.NewGuid();
            elevator.AddDestinationFloor(requestId, 5, Direction.Up);

            Assert.Single(elevator.DestinationFloors);
            Assert.Equal(5, elevator.DestinationFloors.First().Floor);
        }

        [Fact]
        public async Task MoveUp_ShouldIncreaseCurrentFloor()
        {
            var elevator = ElevatorSingleton.GetInstance(ElevatorCar.One);
            elevator.MoveUp();

            Assert.Equal(2, elevator.CurrentFloor);
        }

        [Fact]
        public async Task MoveDown_ShouldDecreaseCurrentFloor()
        {
            var elevator = ElevatorSingleton.GetInstance(ElevatorCar.One);
            elevator.SetCurentFloor(5);
            elevator.MoveDown();

            Assert.Equal(4, elevator.CurrentFloor);
        }

        [Fact]
        public async Task Stop_ShouldClearPickupAndDestinationFloors()
        {
            var elevator = ElevatorSingleton.GetInstance(ElevatorCar.One);
            var requestId = Guid.NewGuid();
            elevator.AddPickupFloor(requestId, 3, Direction.Up);
            elevator.AddDestinationFloor(requestId, 5, Direction.Up);

            elevator.Stop(StopAction.Idle);

            Assert.Empty(elevator.PickupFloors);
            Assert.Empty(elevator.DestinationFloors);
        }
    }
}
