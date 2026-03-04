// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using console;
using core;
using ErrorOr;

Console.WriteLine("Loading program...");

var calculator = ProgramLoader.Load();

Console.WriteLine("Program loaded!");

Console.WriteLine("Enter investor ID and date (format: InvestorId;date):");

var line = Console.ReadLine();

while (!string.IsNullOrWhiteSpace(line))
{
    var input = line.Split(";");
    var investorId = input[0];
    if (DateOnly.TryParse(input[1], out DateOnly date))
    {
        var sw = new Stopwatch();
        ErrorOr<double> result;

        using (var spinner = new Spinner(Console.CursorLeft, Console.CursorTop))
        {
            spinner.Start();
            sw.Start();
            result = calculator.Calculate(investorId, date);
            sw.Stop();
        }

        string message = result.Match(
            value => $"The total for investor {investorId} is {value:C}€",
            errors => $"Error: {errors.First().Description}"
        );

        Console.WriteLine($"{message}. Elapsed time: {sw.Elapsed}");
    }
    else
    {
        Console.WriteLine($"Could not parse date {input[1]}. Example of valid date format: YYYY-MM-DD");
    }

    line = Console.ReadLine();
}
