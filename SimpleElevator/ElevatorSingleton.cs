using SimpleElevator.Helpers;
using SimpleElevator.Models;

namespace SimpleElevator
{
    public class ElevatorSingleton
    {
        public ElevatorCar ElevatorCar { get; private set; }
        public int CurrentFloor { get; private set; } = 1;
        public Direction Direction { get; set; } = Direction.Up;
        public bool IsMoving { get; set; } = false;
        public List<ElevatorRequestDetail> PickupFloors { get; private set; } = [];
        public List<ElevatorRequestDetail> DestinationFloors { get; private set; } = [];
        public void SetCurentFloor(int floor) => CurrentFloor = floor;
        public void AddPickupFloor(Guid requestId, int floor, Direction direction) => PickupFloors.Add(new ElevatorRequestDetail(requestId, floor, direction));
        public void AddDestinationFloor(Guid requestId, int floor, Direction direction) => DestinationFloors.Add(new ElevatorRequestDetail(requestId, floor, direction));

        // The instances dictionary holds a lazy-initialized instance of the Elevator class for each identifier.
        private static readonly Dictionary<int, Lazy<ElevatorSingleton>> instances = [];
        private static readonly object lockObj = new();

        // Provides access to the singleton instance for each identifier.
        public static ElevatorSingleton GetInstance(ElevatorCar elevator)
        {
            lock (lockObj)
            {
                if (!instances.TryGetValue((int)elevator, out Lazy<ElevatorSingleton>? value))
                {
                    value = new Lazy<ElevatorSingleton>(() => new ElevatorSingleton(elevator));
                    instances[(int)elevator] = value;
                }

                return value.Value;
            }
        }

        // Ensures that the Elevator class cannot be instantiated from outside the class.
        private ElevatorSingleton(ElevatorCar elevator)
        {
            ElevatorCar = elevator;
            CurrentFloor = 1;
            Console.WriteLine($"OnStart: {elevator.GetDescription()} is on floor {CurrentFloor}");
        }

        public void MoveUp()
        {
            bool hasStopAction = false;
            if (CurrentFloor < 10)
            {
                Direction = Direction.Up;
                if (PickupFloors.Any(p => p.Floor == CurrentFloor))
                {
                    hasStopAction = true;
                    Stop(StopAction.Loading);
                }

                if (DestinationFloors.Any(p => p.Floor == CurrentFloor))
                {
                    hasStopAction = true;
                    Stop(StopAction.Unloading);
                }

                if (!hasStopAction)
                {
                    IsMoving = true;
                    ElevatorHelpers.Print($"{ElevatorCar.GetDescription()} is on floor {CurrentFloor} (moving up)", ElevatorCar.GetColor());
                    Thread.Sleep(Constants.ElevatorFloorMoveTimeSec);
                    CurrentFloor++;
                }
            }
        }

        public void MoveDown()
        {
            if (CurrentFloor >= 1)
            {
                Direction = Direction.Down;
                if (PickupFloors.Any(p => p.Floor == CurrentFloor))
                {
                    Stop(StopAction.Loading);
                }


                if (DestinationFloors.Any(p => p.Floor == CurrentFloor) &&
                  DestinationFloors.FirstOrDefault(p => p.Floor == CurrentFloor)?.Direction == Direction.Down)
                {
                    Stop(StopAction.Unloading);
                }
                else
                {
                    IsMoving = true;
                    ElevatorHelpers.Print($"{ElevatorCar.GetDescription()} is on floor {CurrentFloor} (moving down)", ElevatorCar.GetColor());
                    Thread.Sleep(Constants.ElevatorFloorMoveTimeSec);
                    CurrentFloor--;
                }
            }
        }

        public void Stop(StopAction action = StopAction.Idle)
        {
            try
            {
                string description = action.GetDescription();
                if (action == StopAction.Idle && DestinationFloors.Any(x => x.Floor == 1))
                {
                    description = StopAction.Unloading.GetDescription();
                }

                ElevatorHelpers.Print($"{ElevatorCar.GetDescription()} is on floor {CurrentFloor} (stopped). {description}", ElevatorCar.GetColor());
                Thread.Sleep(Constants.ElevatorPassengerTransitionTimeSec); // 10 seconds for passengers to enter/leave

                //Update passengers list
                var records = new List<ElevatorRequestDetail>();
                switch (action)
                {
                    case StopAction.Loading:
                        records = [.. PickupFloors
                        .Where(p => p.Floor == CurrentFloor)
                        .OrderBy(x => x.AssignedDate)];

                        foreach (var item in records)
                        {
                            // The destination for the car should be picked after the person has entered the elevator.
                            int destinationFloor = ElevatorHelpers.GenerateRandomFloorDestination(item.Direction, item.Floor);
                            AddDestinationFloor(item.Id, destinationFloor, item.Direction);
                            ElevatorHelpers.Print($"Entered passengers on floor {item.Floor} selected floor {destinationFloor} as their destination.");

                            PickupFloors.Remove(item);
                        }
                        break;
                    case StopAction.Unloading:
                        records = [.. DestinationFloors
                        .Where(p => p.Floor == CurrentFloor)
                        .OrderBy(x => x.AssignedDate)];

                        foreach (var item in records)
                        {
                            // Do not unload passengers who still exists in PickupFloors
                            if (!PickupFloors.Any(p => p.Id == item.Id))
                                DestinationFloors.Remove(item);
                        }

                        if (DestinationFloors.Count == 0)
                        {
                            Direction = Direction.Down;
                            while (CurrentFloor > 1)
                            {
                                MoveDown();
                            }
                            Stop();
                            Direction = Direction.Up;
                        }

                        break;
                    case StopAction.Idle:
                        PickupFloors.Clear();
                        DestinationFloors.Clear();
                        IsMoving = false;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        // TODO: (Cyclomatic Complexity) Refactor this method to make it more readable and maintainable 
        public void ProcessRequest()
        {
            try
            {
                IsMoving = true;
                while (IsMoving)
                {
                    var pickupRecord = PickupFloors.OrderBy(x => x.AssignedDate).FirstOrDefault();
                    var pickupFloor = pickupRecord?.Floor;
                    var pickupFloorDirection = pickupRecord?.Direction;

                    if (pickupFloor.HasValue && pickupFloor >= CurrentFloor)
                    {
                        Direction = Direction.Up;
                        while (CurrentFloor <= pickupFloor)
                        {
                            MoveUp();
                            if (CurrentFloor == pickupFloor && pickupFloorDirection.HasValue 
                                && pickupFloorDirection.Equals(Direction.Down))
                            {
                                break;
                            }
                        }
                    }

                    int? destinationFloor = FindNearestDestination();
                    if (destinationFloor.HasValue && destinationFloor >= CurrentFloor)
                    {
                        Direction = Direction.Up;
                        while (CurrentFloor <= destinationFloor)
                        {
                            MoveUp();
                        }
                    }
                    else if (destinationFloor.HasValue && destinationFloor < CurrentFloor)
                    {
                        Direction = Direction.Down;
                        while (CurrentFloor >= destinationFloor)
                        {
                            MoveDown();
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void MoveToFloor(int floor, Direction direction)
        {
            Direction = direction;
            if (direction == Direction.Up)
            {
                while (CurrentFloor <= floor)
                {
                    MoveUp();
                }
            }
            else
            {
                MoveDown();
            }

        }

        private int? FindNearestDestination()
        {
            var record = DestinationFloors.OrderByDescending(x => x.Floor).FirstOrDefault();

            if (record is null)
                return null;

            foreach (var destination in DestinationFloors.Where(x => x.Direction == Direction))
            {
                if (Direction == Direction.Up)
                {
                    if (record.Floor > destination.Floor && CurrentFloor <= destination.Floor)
                        record.Floor = destination.Floor;
                }
                else
                {
                    if (record.Floor < destination.Floor && CurrentFloor >= destination.Floor)
                        record.Floor = destination.Floor;
                }
            }
            return record.Floor;
        }
    }
}
