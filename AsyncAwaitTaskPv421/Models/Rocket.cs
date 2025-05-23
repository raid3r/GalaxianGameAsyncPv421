using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAwaitTaskPv421.Models;

public class Rocket
{
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    public int Speed { get; set; } = 250;
    private bool isActive = true;

    public async Task MoveAsync()
    {
        Draw();
        while (true) {
            await Task.Delay(Speed);
            Clear();
            if (!isActive)
            {
                break;
            }
            if (Y == 1)
            {
                break;
            }
            Y--;
            Draw();
        }
    }

    public void Destroy()
    {
        isActive = false;
    }

    public void Draw()
    {
        lock (Console.Out)
        {
            Console.SetCursorPosition(X, Y);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("|");
        }
    }

    public void Clear()
    {
        lock (Console.Out)
        {
            Console.SetCursorPosition(X, Y);
            Console.Write(" ");
        }
    }

}
