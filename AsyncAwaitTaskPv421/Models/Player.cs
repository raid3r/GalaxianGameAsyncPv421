using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAwaitTaskPv421.Models;

public class Player
{
    public int X { get; set; } = 0;
    
    public Player(int startX)
    {
        X = startX;
        Draw();
    }

    public void HandleKey(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.LeftArrow:
                if (X > 1)
                {
                    Clear();
                    X--;
                    Draw();
                }
                break;
            case ConsoleKey.RightArrow:
                if (X < Console.WindowWidth - 2)
                {
                    Clear();
                    X++;
                    Draw();
                }
                break;
            default:
                break;
        }
    }

    public void Draw()
    {
        lock (Console.Out)
        {
            Console.SetCursorPosition(X, Console.WindowHeight - 2);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("^");
        }
    }

    public void Clear()
    {
        lock (Console.Out)
        {
            Console.SetCursorPosition(X, Console.WindowHeight - 2);
            Console.Write(" ");
        }
    }

}
