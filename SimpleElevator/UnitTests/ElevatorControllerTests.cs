using SimpleElevator.Helpers;
using Xunit;

namespace SimpleElevator.UnitTests
{
    public class ElevatorControllerTests
    {
        [Fact]
        public async Task AssignElevator_ShouldAssignNearestMovingElevator()
        {
            // Arrange
            var controller = new ElevatorController();
            controller.InitializeElevators();
            var elevator1 = controller.Elevators[0];
            var elevator2 = controller.Elevators[1];
            elevator1.SetCurentFloor(2);
            elevator1.Direction = Direction.Up;
            elevator1.IsMoving = true;
            elevator2.SetCurentFloor(5);
            elevator2.Direction = Direction.Down;
            elevator2.IsMoving = true;

            // Act
            var assignedElevator = controller.AssignElevator(3, Direction.Up);

            // Assert
            Assert.Equal(elevator1, assignedElevator);
        }

        [Fact]
        public async Task AssignElevator_ShouldAssignNearestIdleElevator_WhenNoMovingElevatorInRequestedDirection()
        {
            // Arrange
            var controller = new ElevatorController();
            controller.InitializeElevators();
            var elevator1 = controller.Elevators[0];
            var elevator2 = controller.Elevators[1];
            elevator1.SetCurentFloor(2);
            elevator1.IsMoving = false;
            elevator2.SetCurentFloor(5);
            elevator2.IsMoving = false;

            // Act
            var assignedElevator = controller.AssignElevator(3, Direction.Up);

            // Assert
            Assert.Equal(elevator1, assignedElevator);
        }

        [Fact]
        public async Task AssignElevator_ShouldReturnNull_WhenNoElevatorsAvailable()
        {
            // Arrange
            var controller = new ElevatorController();

            // Act
            var assignedElevator = controller.AssignElevator(3, Direction.Up);

            // Assert
            Assert.Null(assignedElevator);
        }

    }
}
