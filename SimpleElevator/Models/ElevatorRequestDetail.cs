using SimpleElevator.Helpers;

namespace SimpleElevator.Models
{
    public class ElevatorRequestDetail : RequestBase
    {
        public Guid Id { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;

        public ElevatorRequestDetail(Guid requestId, int floor, Direction direction)
        {
            Id = requestId;
            Floor = floor;
            Direction = direction;
        }
    }
}
