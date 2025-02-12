using SimpleElevator;
using SimpleElevator.Helpers;

Console.Write("Enter number of random calls for the elevator: ");
var onStartRequestCount = Convert.ToInt32(Console.ReadLine());
var elevatorController = new ElevatorController();
await elevatorController.StartSimulation(onStartRequestCount);

do
{
    await Task.Delay(ElevatorHelpers.ConvertMinutesToMilliseconds(1));
}
while (elevatorController.IsBusy());

Console.WriteLine("All requests have been processed.");
