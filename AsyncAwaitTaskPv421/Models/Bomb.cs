using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAwaitTaskPv421.Models;

public class Bomb
{
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    public int Speed { get; set; } = 300;
    private bool isActive = true;

    public async Task MoveAsync()
    {
        Draw();
        while (true) {
            await Task.Delay(Speed);
            if (!isActive)
            {
                break;
            }
            Clear();
            Y++;
            if (Y == Console.BufferHeight)
            {
                break;
            }
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
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("U");
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
