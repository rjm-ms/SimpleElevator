using SimpleElevator.Helpers;
using Xunit;

namespace SimpleElevator.UnitTests
{
    [Collection("ElevatorControllerTestCollection")]
    public class ElevatorControllerTests
    {
        public static IEnumerable<object[]> GetMovingElevatorTestData()
        {
            var controller = new ElevatorController();
            controller.InitializeElevators(4);
            var car1 = controller.Elevators.Where(x => x.ElevatorCar == ElevatorCar.One).First();
            var car2 = controller.Elevators.Where(x => x.ElevatorCar == ElevatorCar.Two).First();
            var car3 = controller.Elevators.Where(x => x.ElevatorCar == ElevatorCar.Three).First();
            var car4 = controller.Elevators.Where(x => x.ElevatorCar == ElevatorCar.Four).First();

            car1.SetCurentFloor(1);
            car1.Direction = Direction.Up;
            car1.IsMoving = true;

            car2.SetCurentFloor(10);
            car2.Direction = Direction.Down;
            car2.IsMoving = true;

            car3.SetCurentFloor(5);
            car3.Direction = Direction.Up;
            car3.IsMoving = true;

            car4.SetCurentFloor(4);
            car4.Direction = Direction.Down;
            car4.IsMoving = true;

            return new List<object[]>
            {
                new object[] { controller, 2, Direction.Up, car1.ElevatorCar.GetName() },
                new object[] { controller, 3, Direction.Up, car1.ElevatorCar.GetName() },
                new object[] { controller, 4, Direction.Up, car1.ElevatorCar.GetName() },
                new object[] { controller, 5, Direction.Up, car1.ElevatorCar.GetName() },
                new object[] { controller, 6, Direction.Up, car3.ElevatorCar.GetName() },
                new object[] { controller, 7, Direction.Up, car3.ElevatorCar.GetName() },
                new object[] { controller, 8, Direction.Up, car3.ElevatorCar.GetName() },
                new object[] { controller, 9, Direction.Up, car3.ElevatorCar.GetName() },
                new object[] { controller, 9, Direction.Down, car2.ElevatorCar.GetName() },
                new object[] { controller, 8, Direction.Down, car2.ElevatorCar.GetName() },
                new object[] { controller, 7, Direction.Down, car2.ElevatorCar.GetName() },
                new object[] { controller, 6, Direction.Down, car2.ElevatorCar.GetName() },
                new object[] { controller, 5, Direction.Down, car2.ElevatorCar.GetName() },
                new object[] { controller, 4, Direction.Down, car2.ElevatorCar.GetName() },
                new object[] { controller, 3, Direction.Down, car4.ElevatorCar.GetName() },
                new object[] { controller, 2, Direction.Down, car4.ElevatorCar.GetName() },
            };
        }

        public static IEnumerable<object[]> GetIdleElevatorTestData()
        {
            var controller = new ElevatorController();
            controller.InitializeElevators(4);
            var car1 = controller.Elevators.Where(x => x.ElevatorCar == ElevatorCar.One).First();
            var car2 = controller.Elevators.Where(x => x.ElevatorCar == ElevatorCar.Two).First();
            var car3 = controller.Elevators.Where(x => x.ElevatorCar == ElevatorCar.Three).First();
            var car4 = controller.Elevators.Where(x => x.ElevatorCar == ElevatorCar.Four).First();

            car1.SetCurentFloor(10);
            car1.Direction = Direction.Down;
            car1.IsMoving = true;

            car2.SetCurentFloor(1);
            car2.Direction = Direction.Up;
            car2.IsMoving = false;

            car3.SetCurentFloor(5);
            car3.Direction = Direction.Up;
            car3.IsMoving = true;

            car4.SetCurentFloor(4);
            car4.Direction = Direction.Down;
            car4.IsMoving = true;

            return new List<object[]>
            {
                new object[] { controller, 2, Direction.Up, car2.ElevatorCar.GetName() },
                new object[] { controller, 3, Direction.Up, car2.ElevatorCar.GetName() },
                new object[] { controller, 4, Direction.Up, car2.ElevatorCar.GetName() },
                new object[] { controller, 5, Direction.Up, car2.ElevatorCar.GetName() },
            };
        }

        [Theory]
        [MemberData(nameof(GetMovingElevatorTestData))]
        public void AssignElevator_ShouldAssignNearestMovingElevator(ElevatorController controller, int requestedFloor,
            Direction requestDirection, string expectedElevator)
        {
            // Act
            var assignedElevator = controller.AssignElevator(requestedFloor, requestDirection);

            // Assert
            Assert.Equal(expectedElevator, assignedElevator.ElevatorCar.GetName());
        }

        [Theory]
        [MemberData(nameof(GetIdleElevatorTestData))]
        public void AssignElevator_ShouldAssignNearestIdleElevator_WhenNoMovingElevatorInRequestedDirection(ElevatorController controller, int requestedFloor,
            Direction requestDirection, string expectedElevator)
        {
            // Act
            var assignedElevator = controller.AssignElevator(requestedFloor, requestDirection);

            // Assert
            Assert.Equal(expectedElevator, assignedElevator.ElevatorCar.GetName());
        }

        [Fact]
        public void AssignElevator_ShouldReturnNull_WhenNoElevatorsAvailable()
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
