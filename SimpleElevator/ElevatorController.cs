using SimpleElevator.Helpers;
using SimpleElevator.Models;

namespace SimpleElevator
{
    public class ElevatorController
    {
        private int AssignedRequestCount = 0;
        private int QueuedRequestCount = 0;
        private int OnStartRequestCount = 0;
        public List<ElevatorSingleton> Elevators { get; set; } = new List<ElevatorSingleton>();
        public bool IsBusy() => Elevators.Any(x => x.IsMoving);
        public Queue<ElevatorRequest> ElevatorRequests { get; set; } = new Queue<ElevatorRequest>();
        public ElevatorController()
        {

        }

        public async Task StartSimulation(int onStartRequestCount)
        {
            OnStartRequestCount = onStartRequestCount;
            InitializeElevators();
            StartAddToQueueBackgroundProcess();
            StartElevatorDispatchBackgroundProcess();
        }


        public void InitializeElevators()
        {
            Elevators.Add(ElevatorSingleton.GetInstance(ElevatorCar.One));
            Elevators.Add(ElevatorSingleton.GetInstance(ElevatorCar.Two));
            Elevators.Add(ElevatorSingleton.GetInstance(ElevatorCar.Three));
            Elevators.Add(ElevatorSingleton.GetInstance(ElevatorCar.Four));
        }

        void StartAddToQueueBackgroundProcess()
        {
            void timerCallback(object state)
            {
                if (OnStartRequestCount > QueuedRequestCount)
                {
                    var random = new Random();

                    Direction direction = random.Next(2) == 0 ? Direction.Up : Direction.Down;
                    int requestedFloor = random.Next(1, 11); // Generates a number from 1 to 10
                    int destinationFloor = ElevatorHelpers.GenerateRandomFloorDestination(direction, requestedFloor);

                    if ((direction == Direction.Up && requestedFloor < 10) ||
                        (direction == Direction.Down && requestedFloor > 1))
                    {
                        var request = new ElevatorRequest(requestedFloor, destinationFloor, direction);
                        ElevatorRequests.Enqueue(request);
                        QueuedRequestCount++;
                    }
                }
            }
            // Add to queue every 10-30 seconds
            new Timer(timerCallback, null, 0, new Random().Next(10, 30) * 1000);
        }

        void StartElevatorDispatchBackgroundProcess()
        {
            TimerCallback timerCallback = async (object state) =>
            {
                DispatchElevators();
            };
            // Check queue and dispatch every 1 sec
            new Timer(timerCallback, null, 0, 1 * 1000);
        }

        public ElevatorSingleton AssignElevator(int requestedFloor, Direction direction)
        {
            int minDistance = int.MaxValue;

            // Find the nearest moving elevator in the requested direction
            ElevatorSingleton? nearestElevator = FindNearestMovingElevator(requestedFloor, direction, ref minDistance);

            // If no moving elevator is found, prioritize idle elevators
            if (nearestElevator == null)
            {
                nearestElevator = FindNearestIdleElevator(requestedFloor, ref minDistance);
            }

            return nearestElevator;
        }

        ElevatorSingleton FindNearestMovingElevator(int requestedFloor, Direction direction, ref int minDistance)
        {
            ElevatorSingleton? nearestElevator = null;

            foreach (var elevator in Elevators)
            {
                int distance = Math.Abs(elevator.CurrentFloor - requestedFloor);
                if (distance < minDistance && elevator.Direction == direction)
                {
                    if ((direction == Direction.Up && requestedFloor > elevator.CurrentFloor) ||
                        (direction == Direction.Down && requestedFloor < elevator.CurrentFloor))
                    {
                        minDistance = distance;
                        nearestElevator = elevator;
                    }
                }
            }

            return nearestElevator;
        }

        ElevatorSingleton FindNearestIdleElevator(int requestedFloor, ref int minDistance)
        {
            ElevatorSingleton? nearestElevator = null;

            foreach (var elevator in Elevators)
            {
                if (!elevator.IsMoving)
                {
                    int distance = Math.Abs(elevator.CurrentFloor - requestedFloor);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestElevator = elevator;
                    }
                }
            }

            return nearestElevator;
        }

        public void DispatchElevators()
        {
            if (ElevatorRequests.Count > 0)
            {
                var request = ElevatorRequests.Peek();
                var assignedElevator = AssignElevator(request.PickupFloor, request.Direction);

                if (assignedElevator != null)
                {
                    request = ElevatorRequests.Dequeue();
                    var requestId = Guid.NewGuid();
                    assignedElevator.AddPickupFloor(requestId, request.PickupFloor, request.Direction);
                    assignedElevator.AddDestinationFloor(requestId, request.DestinationFloor, request.Direction);

                    ElevatorHelpers.Print($"({++AssignedRequestCount}/{OnStartRequestCount}) `{request.Direction}` request on floor " +
                        $"{request.PickupFloor} received (destination: floor {request.DestinationFloor}). Assigned to `{assignedElevator.ElevatorCar.GetDescription()}`");

                    if (!assignedElevator.IsMoving)
                    {
                        assignedElevator.ProcessRequest();
                    }
                }
            }
        }
    }
}
