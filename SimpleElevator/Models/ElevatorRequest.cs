using SimpleElevator.Helpers;

namespace SimpleElevator.Models
{
    public class ElevatorRequest : RequestBase
    {
        public int PickupFloor { get; set; }
        public int DestinationFloor { get; set; }

        public ElevatorRequest(int requestedFloor, int destinationFloor, Direction direction)
        {
            PickupFloor = requestedFloor;
            DestinationFloor = destinationFloor;
            Direction = direction;
        }
    }
}
