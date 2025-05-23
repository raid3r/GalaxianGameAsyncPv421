using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAwaitTaskPv421.Models;

public class ScorePanel
{
    public int X { get; set; } = 90;
    public int Y { get; set; } = 0;
    private int Width { get; set; } = 10;
    private int Height { get; set; } = 3;

    public int Value { get; set; } = 0; 

    private Object _lock = new Object();

    public async Task MoveAsync()
    {
        Draw();
        while (true)
        {
            await Task.Delay(50);
            Draw();


        }
    }

    public void AddScore(int score)
    {
        lock (_lock)
        {
            Value += score;
        }
    }

    public void Draw()
    {
        DrawFrame();
    }

    public void Clear()
    {
        
    }

    private ConsoleColor GetRandomColor()
    {
        var colors = Enum.GetValues(typeof(ConsoleColor));
        Random random = new Random();
        int index = random.Next(colors.Length);
        return (ConsoleColor)colors.GetValue(index);
    }

    private void DrawFrame()
    {
        Console.ForegroundColor = GetRandomColor();
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (i == 0 || i == Width - 1 || j == 0 || j == Height - 1)
                {
                    lock (Console.Out)
                    {
                        Console.SetCursorPosition(X+ i, Y + j);
                        Console.Write("*");
                    }
                }
            }
        }
        lock (Console.Out)
        {
            Console.SetCursorPosition(X + 2, Y + 1);
            Console.ForegroundColor = GetRandomColor();
            Console.Write($"{Value}".PadLeft(6));
        }
    }
}
