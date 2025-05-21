using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAwaitTaskPv421.Models;

public class Enemy
{
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    public int Speed { get; set; } = 250;
    private bool isActive = true;

    public bool Hit(int rocketX, int rocketY)
    {
        if (!isActive)
        {
            return true;
        }

        List<int> xCoords =  [X, X + 1, X + 2, X + 3];
        isActive = !(Y == rocketY && xCoords.Contains(rocketX));

        if (!isActive)
        {
            lock (Console.Out)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.SetCursorPosition(10, 10);
                Console.WriteLine("Hit!");
            }

            Task.Run(async () => {
                await Task.Delay(1000);
                lock (Console.Out)
                {
                    Console.SetCursorPosition(10, 10);
                    Console.WriteLine("    ");
                }
            });
        }

        return !isActive;
    }

    public async Task MoveAsync()
    {
        Draw();
        var toDownMove = 20;

        var random = new Random();
        int moves = 1;
        int nextMove = 0;
        while (true)
        {
            await Task.Delay(Speed);
            

            Clear();
            if (!isActive)
            {
                break;
            }

            if (moves == 0)
            {
                nextMove = random.Next(0, 2); // Randomly choose to move left or right
                moves = random.Next(4, 10); // Randomly choose the number of moves
            }
            moves--;
            toDownMove--;
            if (toDownMove == 0)
            {
                Y++;
                toDownMove = 20;
            }

            switch (nextMove)
            {
                case 0:
                    if (X > 2)
                    {
                        X--;
                    }
                    else
                    {
                        moves = 0; // Reset moves if at the edge
                    }
                    break;
                case 1:
                    if (X < Console.WindowWidth - 5)
                    {
                        X++;
                    }
                    else
                    {
                        moves = 0; // Reset moves if at the edge
                    }
                    break;
            }
            Draw();
        }
    }

    public void Draw()
    {
        lock (Console.Out)
        {
            Console.SetCursorPosition(X, Y);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[==]");
        }
    }

    public void Clear()
    {
        lock (Console.Out)
        {
            Console.SetCursorPosition(X, Y);
            Console.Write("    ");
        }
    }

}
