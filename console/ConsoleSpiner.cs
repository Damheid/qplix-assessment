// See https://aka.ms/new-console-template for more information
namespace console;

public class ConsoleSpiner
{
    int counter;

    public ConsoleSpiner()
    {
        counter = 0;
    }

    public void Turn()
    {
        counter++;
        switch (counter % 4)
        {
            case 0: Console.Write("/"); break;
            case 1: Console.Write("-"); break;
            case 2: Console.Write("\\"); break;
            case 3: Console.Write("|"); break;
        }
        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
    }
}

// Source - https://stackoverflow.com/a/33685254
// Posted by ThisGuy, modified by community. See post 'Timeline' for change history
// Retrieved 2026-03-01, License - CC BY-SA 4.0

  public class Spinner : IDisposable
  {
     private const string Sequence = @"/-\|";
     private int counter = 0;
     private readonly int left;
     private readonly int top;
     private readonly int delay;
     private bool active;
     private readonly Thread thread;
     private ConsoleColor initialColor;

     public Spinner(int left, int top, int delay = 100)
     {
        this.left = left;
        this.top = top;
        this.delay = delay;
        thread = new Thread(Spin);
        initialColor = Console.ForegroundColor;
     }

     public void Start()
     {
        active = true;
        if (!thread.IsAlive)
           thread.Start();
     }

     public void Stop()
     {
        active = false;
        Console.SetCursorPosition(left, top);
        Console.ForegroundColor = initialColor;
     }

     private void Spin()
     {
        while (active)
        {
           Turn();
           Thread.Sleep(delay);
        }
     }

     private void Draw(char c)
     {
        Console.SetCursorPosition(left, top);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(c);
     }

     private void Turn()
     {
        Draw(Sequence[++counter % Sequence.Length]);
     }

     public void Dispose()
     {
        Stop();
     }
  }
