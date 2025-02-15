using SimpleElevator.Helpers;

namespace SimpleElevator.Models
{
    public class ElevatorRequest : RequestBase
    {
        public int PickupFloor { get; set; }

        public ElevatorRequest(int requestedFloor, Direction direction)
        {
            PickupFloor = requestedFloor;
            Direction = direction;
        }
    }
}
