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
            if (CurrentFloor < 10)
            {
                Direction = Direction.Up;
                if (PickupFloors.Any(p => p.Floor == CurrentFloor) &&
                    PickupFloors.FirstOrDefault(p => p.Floor == CurrentFloor)?.Direction == Direction.Up)
                {
                    Stop(StopAction.Loading);
                }
                if (DestinationFloors.Any(p => p.Floor == CurrentFloor) &&
                  DestinationFloors.FirstOrDefault(p => p.Floor == CurrentFloor)?.Direction == Direction.Up)
                {
                    Stop(StopAction.Unloading);
                }
                else
                {
                    IsMoving = true;
                    ElevatorHelpers.Print($"{ElevatorCar.GetDescription()} is on floor {CurrentFloor} (moving up)", ElevatorCar.GetColor());
                    Thread.Sleep(Constants.ElevatorFloorMoveTimeSec);
                }
                CurrentFloor++;
            }
        }

        public void MoveDown()
        {
            if (CurrentFloor >= 1)
            {
                Direction = Direction.Down;
                if (PickupFloors.Any(p => p.Floor == CurrentFloor) &&
                    PickupFloors.FirstOrDefault(p => p.Floor == CurrentFloor)?.Direction == Direction.Down)
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
                }
                CurrentFloor--;
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
                        .OrderBy(x => x.RequestDate)];

                        foreach (var item in records)
                            PickupFloors.Remove(item);
                        break;
                    case StopAction.Unloading:
                        records = [.. DestinationFloors
                        .Where(p => p.Floor == CurrentFloor)
                        .OrderBy(x => x.RequestDate)];

                        foreach (var item in records)
                        {
                            // Do not unload passengers who still exists in PickupFloors
                            if (!PickupFloors.Any(p => p.Id == item.Id))
                                DestinationFloors.Remove(item);
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
                    var pickupFloor = PickupFloors.OrderBy(x => x.RequestDate).FirstOrDefault()?.Floor;
                    var destinationFloor = DestinationFloors.OrderByDescending(x => x.Floor).FirstOrDefault()?.Floor;

                    if (destinationFloor is null)
                        break;

                    if (pickupFloor.HasValue && pickupFloor >= CurrentFloor)
                    {
                        Direction = Direction.Up;
                        while (CurrentFloor < pickupFloor)
                        {
                            MoveUp();
                        }

                        destinationFloor = FindNearestDestination();
                        if (destinationFloor >= CurrentFloor)
                        {
                            Direction = Direction.Up;
                            while (CurrentFloor < destinationFloor)
                            {
                                MoveUp();
                            }
                            Stop(StopAction.Unloading);
                        }
                        else if (destinationFloor < CurrentFloor)
                        {
                            Direction = Direction.Down;
                            while (CurrentFloor > destinationFloor)
                            {
                                MoveDown();
                            }
                            Stop(StopAction.Unloading);
                        }
                    }
                    else if (pickupFloor.HasValue && pickupFloor <= CurrentFloor)
                    {
                        Direction = Direction.Down;
                        while (CurrentFloor >= pickupFloor)
                        {
                            MoveDown();
                        }
                    }
                    else if (pickupFloor is null && destinationFloor <= CurrentFloor)
                    {
                        while (CurrentFloor >= destinationFloor)
                        {
                            if (destinationFloor == CurrentFloor && destinationFloor == 1)
                            {
                                Stop(StopAction.Idle);
                                break;
                            }
                            else
                                MoveDown();
                        }
                    }
                    else if (pickupFloor is null && destinationFloor >= CurrentFloor)
                    {
                        Direction = Direction.Up;
                        while (destinationFloor >= CurrentFloor)
                        {
                            if (destinationFloor == CurrentFloor && destinationFloor == 10)
                            {
                                Stop(StopAction.Unloading);
                                break;
                            }
                            else
                                MoveUp();
                        }
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
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private int FindNearestDestination()
        {
            int destinationFloor = DestinationFloors.OrderByDescending(x => x.Floor).First().Floor;
            foreach (var destination in DestinationFloors.Where(x => x.Direction == Direction))
            {
                if (Direction == Direction.Up)
                {
                    if (destinationFloor > destination.Floor && CurrentFloor <= destination.Floor)
                        destinationFloor = destination.Floor;
                }
                else
                {
                    if (destinationFloor < destination.Floor && CurrentFloor >= destination.Floor)
                        destinationFloor = destination.Floor;
                }
            }
            return destinationFloor;
        }
    }
}
