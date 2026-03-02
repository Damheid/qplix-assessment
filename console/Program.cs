// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using console;
using core;

Console.WriteLine("Loading program...");

var calculator = ProgramLoader.Load();

Console.WriteLine("Program loaded!");

Console.WriteLine("Enter investor ID and date (format InvestorId;date):");

var line = Console.ReadLine();

while (!string.IsNullOrWhiteSpace(line))
{
    var input = line.Split(";");
    var investorId = input[0];
    var date = DateOnly.Parse(input[1]);
    var sw = new Stopwatch();
    double total = 0;

    using (var spinner = new Spinner(Console.CursorLeft, Console.CursorTop))
    {
        spinner.Start();
        sw.Start();
        total = calculator.Calculate(investorId, date);
        sw.Stop();
    }

    Console.WriteLine($"The total for investor {investorId} is {total:N1}€. Elapsed time: {sw.Elapsed}");
    
    line = Console.ReadLine();
}
