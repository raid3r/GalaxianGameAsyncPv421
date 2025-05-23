using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAwaitTaskPv421.Models;

internal class Explosion
{
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;

    /*
     *             [==]
     *
     *              *
     *             ****    
     *               *
     *             
     *             *  *
     *             ** *
     *             *    *
     * 
     */

    private int frame = 0; // 0 - 3

    public async Task MoveAsync()
    {
       do
        {
            Draw();
            await Task.Delay(200);
            Clear();
            frame++;
        } while (frame < 3);
    }

    List<List<string>> frames = [
        new List<string>
        {
            "       ",
            "       ",
            "  ***  ",
            "       ",
            "       ",
        },
        new List<string>
        {
            "       ",
            " * *   ",
            "  * ** ",
            "  *    ",
            "       ",
        },
        new List<string>
        {
            "*  *   ",
            " * *  *",
            "*    * ",
            "*   *  ",
            "   *   ",
        }
        ];

    public void Draw()
    {
        DrawFrame(frames[frame]);
    }

    private void DrawFrame(List<string> frame, bool clear = false)
    {
        var centerX = X;
        var centerY = Y;
        var startX = centerX - 3;
        var startY = centerY - 2;

        for (int row = 0; row < frame.Count; row++)
        {
            for (int col = 0; col < frame[row].Length; col++)
            {
                if (row + startY < 2 || row + startY >= Console.WindowHeight - 2)
                {
                    continue;
                }
                if (col + startX < 2 || col + startX >= Console.WindowWidth - 2)
                {
                    continue;
                }

                if (frame[row][col] == ' ')
                {
                    continue;
                }

                lock (Console.Out)
                {
                    Console.SetCursorPosition(startX+ col, startY+ row);
                    if (clear)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(" ");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(frame[row][col]);
                    }
                }
            }
        }
    }

    public void Clear()
    {
        DrawFrame(frames[frame], true);
    }
}
