using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncAwaitTaskPv421.Models;

public class Game
{
    private const int Width = 100;
    private const int Height = 30;

    // Setting up game parameters       
    private int maxRockets = 5; // Maximum number of rockets allowed
    private int bombCount = 5; // Number of bombs to create
    private int enemyCount = 4; // Number of enemies to create init
    private int totalEmeniesMax = 10; // Total number of enemies to create

    private bool isRunning = true; // Flag to control the game loop
    private bool isGameOver = false; // Flag to check if the game is over

    private Random random = new(); // Create a random number generator
    private Player player = new(Width / 2); // Create a player object
    private List<Rocket> rockets = []; // List to hold multiple rockets 
    private List<Enemy> enemies = []; // List to hold multiple enemies
    private List<Bomb> bombs = []; // List to hold multiple bombs
    private ScorePanel scorePanel = new(); // Create a score panel object

    private void CreateEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Task.Run(async () =>
            {
                await CreateEnemyAsync(); // Create a new enemy asynchronously
            }); // Start the enemy creation asynchronously
        }
    }

    private void DrawFrame()
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

    private async Task CreateEnemyAsync()
    {
        var enemy = new Enemy() { X = random.Next(0, 80), Y = random.Next(3, 10), Speed = random.Next(100, 300) }; // Create a new enemy object
        lock (enemies)
        {
            enemies.Add(enemy); // Add the enemy to the list
        }

        await enemy.MoveAsync(); // Start the enemy movement asynchronously
        lock (enemies)
        {
            enemies.Remove(enemy); // Remove the enemy from the list after it has moved
        }

    }

    private async Task CreateBombsAsync()
    {
        while (true)
        {
            await Task.Delay(2000); // Delay for 2 second before starting the bomb creation
            if (isGameOver)
            {
                break; // Exit the loop if the game is over
            }

            lock (enemies)
            {
                if (enemies.Count == 0)
                {
                    break; // Exit the loop if there are no enemies left
                }

                enemies.ForEach(e =>
                {
                    var throwBomb = random.Next(0, 100) < 20; // Randomly decide whether to throw a bomb
                    if (throwBomb)
                    {
                        var bomb = new Bomb() { X = e.X, Y = e.Y + 1, Speed = random.Next(200, 500) }; // Create a new bomb object
                        lock (bombs)
                        {
                            if (bombs.Count < bombCount)
                            {
                                bombs.Add(bomb); // Add the bomb to the list    
                            }
                        }
                        Task.Run(async () =>
                        {
                            await bomb.MoveAsync(); // Start the bomb movement asynchronously
                            lock (bombs)
                            {
                                bombs.Remove(bomb); // Remove the bomb from the list after it has moved
                            }
                        }); // Start the bomb movement asynchronously
                    }
                });
            }
        }
    }

    private async Task RocketFireAsync(int x, int y)
    {
        lock (rockets)
        {
            if (rockets.Count >= maxRockets)
            {
                return; // Exit the method if the maximum number of rockets is reached
            }


            var rocket = new Rocket() { X = x, Y = y, Speed = 100 }; // Create a new rocket object
            rockets.Add(rocket); // Add the rocket to the list

            Task.Run(async () =>
            {
                await rocket.MoveAsync();
                lock (rockets)
                {
                    rockets.Remove(rocket); // Remove the rocket from the list after it has moved
                }
            }); // Start the rocket movement asynchronously

        }
    }

    private async Task MakeExplosionAsync(int x, int y)
    {
        var explosion = new Explosion() { X = x, Y = y }; // Create a new explosion object
        await explosion.MoveAsync(); // Start the explosion movement asynchronously
    }

    private async Task RocketAndEnemyCollideCheckAsync()
    {
        while (true)
        {
            await Task.Delay(20);
            if (!isRunning || isGameOver)
            {
                break; // Exit the loop if the game is not running or if the game is over
            }

            lock (rockets)
            {
                foreach (var rocket in rockets)
                {
                    bool hit = false;
                    lock (enemies)
                    {
                        foreach (var enemy in enemies)
                        {

                            if (enemy.Hit(rocket.X, rocket.Y))
                            {
                                Task.Run(async () =>
                                {
                                    await MakeExplosionAsync(enemy.X, enemy.Y); // Create an explosion at the enemy's position
                                });
                                scorePanel.AddScore(100); // Add score for hitting an enemy
                                rocket.Destroy(); // Destroy the rocket
                                hit = true; // Set the hit flag to true
                                break; // Exit the loop if the rocket hits an enemy
                            }

                        }
                        if (hit)
                        {
                            break; // Exit the loop if the rocket hits an enemy
                        }
                    }
                }
            }
        }
    }

    private async Task RocketAndBombCollideCheckAsync()
    {

        while (true)
        {
            await Task.Delay(20);
            if (!isRunning || isGameOver)
            {
                break; // Exit the loop if the game is not running or if the game is over
            }

            lock (rockets)
            {
                foreach (var rocket in rockets)
                {
                    lock (bombs)
                    {
                        foreach (var bomb in bombs)
                        {
                            if (rocket.X == bomb.X && rocket.Y == bomb.Y)
                            {
                                Task.Run(async () =>
                                {
                                    await MakeExplosionAsync(bomb.X, bomb.Y); // Create an explosion at the bomb's position
                                });
                                scorePanel.AddScore(50); // Add score for hitting a bomb
                                bomb.Destroy(); // Destroy the bomb
                                rocket.Destroy(); // Destroy the rocket
                            }
                        }
                    }
                }
            }
        }
    }

    private async Task BombAndPlayerCollideCheckAsync()
    {
        while (true)
        {
            await Task.Delay(20);
            if (!isRunning || isGameOver)
            {
                break; // Exit the loop if the game is not running or if the game is over
            }

            foreach (var bomb in bombs)
            {
                if (bomb.X == player.X && bomb.Y == Height - 3)
                {
                    Task.Run(async () =>
                    {
                        await MakeExplosionAsync(bomb.X, bomb.Y); // Create an explosion at the bomb's position
                    });
                    bomb.Destroy(); // Destroy the bomb
                    isGameOver = true; // Set the game over flag to true
                    player.Clear(); // Clear the player from the console
                }
            }
        }
    }

    private async Task AddEnemiesAsync()
    {
        var totalEnemies = totalEmeniesMax; // Set the total number of enemies to create
        while (true)
        {
            await Task.Delay(100);
            if (!isRunning || isGameOver)
            {
                break; // Exit the loop if the game is not running or if the game is over
            }

            if (enemies.Count < enemyCount && totalEnemies > 0)
            {
                totalEnemies--;

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
    }

    private async Task CheckWinOrLooseAsync()
    {
        while (true)
        {
            await Task.Delay(20);

            // Win
            if (enemies.Count == 0)
            {
                lock (Console.Out)
                {
                    Console.SetCursorPosition(10, 11);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("You win!");
                    Console.SetCursorPosition(10, 12);
                    Console.WriteLine("Press Enter to exit...");
                    Console.ReadLine();
                }
                isRunning = false; // Set the running flag to false
                break; // Exit the game loop if all enemies are defeated
            }

            // Game Over
            if (isGameOver)
            {
                lock (Console.Out)
                {
                    Console.SetCursorPosition(10, 11);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Game Over!");
                    Console.SetCursorPosition(10, 12);
                    Console.WriteLine("Press Enter to exit...");
                    Console.ReadLine();
                }
                isRunning = false; // Set the running flag to false
                break; // Exit the game loop if the game is over
            }

        }
    }
    
    
    public void Run()
    {
        Init(); // Initialize the game
        CreateEnemies(); // Create enemies at the start of the game
        player.Draw(); // Draw the player on the console

        Task.Run(async () =>
        {
            await CreateBombsAsync(); // Create bombs at the start of the game
        }); // Start the bomb creation asynchronously

        Task.Run(async () =>
        {
            await RocketAndEnemyCollideCheckAsync(); // Check for rocket and enemy collisions asynchronously
        }); // Start the collision check asynchronously

        Task.Run(async () =>
        {
            await RocketAndBombCollideCheckAsync(); // Check for rocket and bomb collisions asynchronously
        }); // Start the collision check asynchronously

        Task.Run(async () =>
        {
            await BombAndPlayerCollideCheckAsync(); // Check for bomb and player collisions asynchronously
        }); // Start the collision check asynchronously

        Task.Run(async () =>
        {
            await AddEnemiesAsync(); // Add enemies to the game asynchronously
        }); // Start the enemy addition asynchronously

        Task.Run(async () =>
        {
            await CheckWinOrLooseAsync(); // Check for win or lose conditions asynchronously
        }); // Start the win/lose check asynchronously

        Task.Run(async () =>
        {
            await scorePanel.MoveAsync(); // Start the score panel movement asynchronously
        }); // Start the bomb creation asynchronously


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
                        Task.Run(async () =>
                        {
                            await RocketFireAsync(player.X, Height - 3); // Fire a rocket
                        }); // Start the rocket firing asynchronously
                        break;
                    default:
                        break;
                }

                if (!isGameOver)
                {
                    player.HandleKey(key); // Handle player input
                }
                else
                {
                    player.Clear(); // Clear the player from the console
                }
            }

            Thread.Sleep(10); // Simulate some delay before the next iteration
            if (!isRunning)
            {
                break; // Exit the game loop if the running flag is false
            }
        }
    }
}
