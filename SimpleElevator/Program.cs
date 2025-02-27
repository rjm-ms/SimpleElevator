﻿using SimpleElevator;
using SimpleElevator.Helpers;

int onStartElevatorCount;
bool isValidNumberOfElevatorsInput;

do
{
    Console.Write("Enter number of available elevators (1 to 4 only): ");
    isValidNumberOfElevatorsInput = int.TryParse(Console.ReadLine(), out onStartElevatorCount) &&
                   onStartElevatorCount >= 1 &&
                   onStartElevatorCount <= 4;

    if (!isValidNumberOfElevatorsInput)
    {
        ElevatorHelpers.Print("Invalid input. Please enter a number between 1 and 4.", ConsoleColor.Red);
    }
}
while (!isValidNumberOfElevatorsInput);


int onStartRequestCount;
bool isValidRequestCountInput;
do
{
    Console.Write($"Enter number of random calls to {onStartElevatorCount} available elevator(s): ");
    isValidRequestCountInput = int.TryParse(Console.ReadLine(), out onStartRequestCount) &&
                   onStartRequestCount > 0;

    if (!isValidRequestCountInput)
    {
        ElevatorHelpers.Print("Invalid input. Please enter a positive number.", ConsoleColor.Red);
    }
}
while (!isValidRequestCountInput);

ElevatorHelpers.Print($"\nElevator simulation started: {onStartElevatorCount} elevator(s) will accomodate {onStartRequestCount} random request(s).");

var elevatorController = new ElevatorController();
elevatorController.StartSimulation(onStartElevatorCount, onStartRequestCount);

do
{
    await Task.Delay(ElevatorHelpers.ConvertMinutesToMilliseconds(1));
}
while (elevatorController.IsBusy());

Console.WriteLine("All requests have been processed.");
