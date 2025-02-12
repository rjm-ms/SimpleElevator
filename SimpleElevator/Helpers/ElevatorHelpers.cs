namespace SimpleElevator.Helpers
{
    public static class ElevatorHelpers
    {
        public static int GenerateRandomFloorDestination(Direction direction, int requestedFloor)
        {
            Random rnd = new();
            int destinationFloor;

            if (direction == Direction.Up)
            {
                // Generate a random floor above the requested floor
                destinationFloor = rnd.Next(requestedFloor + 1, 11);
            }
            else
            {
                // Generate a random floor below the requested floor
                destinationFloor = rnd.Next(1, requestedFloor);
            }

            return destinationFloor;
        }

        public static void Print(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White; // Reset
        }

        public static int ConvertMinutesToMilliseconds(int minutes) => minutes * 60 * 1000;
    }
}

