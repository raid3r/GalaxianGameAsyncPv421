using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAwaitTaskPv421.Models;

public class Game
{
    public void Run()
    {
        Init(); // Initialize the game

        bool isRunning = true; // Flag to control the game loop

        var random = new Random(); // Create a random number generator

        var player = new Player(Width / 2); // Create a player object
                                            // 
        List<Rocket> rockets = []; // List to hold multiple rockets 
        int maxRockets = 2; // Maximum number of rockets allowed

        List<Enemy> enemies = []; // List to hold multiple enemies
        int enemyCount = 10; // Number of enemies to create
        for (int i = 0; i < enemyCount; i++)
        {
            var enemy = new Enemy() { X = random.Next(0, 80), Y = random.Next(3, 10), Speed = random.Next(100, 300) }; // Create a new enemy object
            lock (enemies)
            {
                enemies.Add(enemy); // Add the enemy to the list
            }

            Task.Run(async () =>
            {
                await enemy.MoveAsync(); // Start the enemy movement asynchronously
                lock (enemies)
                {
                    enemies.Remove(enemy); // Remove the enemy from the list after it has moved
                }
            });
        }

        Task.Run(async () =>
        {
            var totalEnemies = 20;

            while (true)
            {
                await Task.Delay(20);
                lock (rockets)
                {
                    foreach (var rocket in rockets)
                    {
                        lock (enemies)
                        {
                            foreach (var enemy in enemies)
                            {
                                enemy.Hit(rocket.X, rocket.Y);
                            }
                        }
                    }
                }

                if (enemies.Count < 10 && totalEnemies > 0)
                {
                    totalEnemies--;
                    lock (enemies)
                    {
                        var enemy = new Enemy() { X = random.Next(0, 80), Y = random.Next(3, 10), Speed = random.Next(100, 300) }; // Create a new enemy object
                        lock (enemies)
                        {
                            enemies.Add(enemy); // Add the enemy to the list
                        }

                        Task.Run(async () =>
                        {
                            await enemy.MoveAsync(); // Start the enemy movement asynchronously
                            lock (enemies)
                            {
                                enemies.Remove(enemy); // Remove the enemy from the list after it has moved
                            }
                        });
                    }
                }

                if (enemies.Count == 0)
                {
                    lock (Console.Out)
                    {
                        Console.SetCursorPosition(10, 11);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("You win!");
                        Console.SetCursorPosition(10, 12);
                        Console.WriteLine("Press any key to exit...");
                    }
                    isRunning = false; // Set the running flag to false
                    break; // Exit the game loop if all enemies are defeated
                }
            }
        });

        while (true)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.Escape:
                        return; // Exit the game loop
                    case ConsoleKey.Spacebar:
                        if (rockets.Count < maxRockets)
                        {
                            var rocket = new Rocket() { X = player.X, Y = Height - 3, Speed = 100 }; // Create a new rocket object
                            lock (rockets)
                            {
                                rockets.Add(rocket); // Add a new rocket to the list
                            }

                            Task.Run(async () =>
                            {
                                await rocket.MoveAsync();
                                lock (rockets)
                                {
                                    rockets.Remove(rocket); // Remove the rocket from the list after it has moved
                                }
                            }); // Start the rocket movement asynchronously
                        }
                        break;
                    default:
                        break;
                }

                player.HandleKey(key); // Handle player input
            }

            Thread.Sleep(10); // Simulate some delay before the next iteration
            if (!isRunning)
            {
                break; // Exit the game loop if the running flag is false
            }
        }
        Console.ReadKey(); // Wait for a key press before exiting
    }

    private const int Width = 100;
    private const int Height = 30;


    public void DrawFrame()
    {
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (i == 0 || i == Width - 1 || j == 0 || j == Height - 1)
                {
                    Console.SetCursorPosition(i, j);
                    Console.Write("*");
                    continue;
                }

            }
        }
    }

    private void Init()
    {
        Console.CursorVisible = false; // Hide the cursor
        Console.Clear(); // Clear the console
        Console.SetWindowSize(Width, Height); // Set the console window size
        DrawFrame(); // Draw the game frame
    }



}
