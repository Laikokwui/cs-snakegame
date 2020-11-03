using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Media;
using SnakeGame;
using System.Reflection;
using System.Diagnostics;

namespace Snake
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Leaderboard leaderboard = new Leaderboard();
                int option = MainMenu();
                int difficulty = 1;

                if (option == 0)
                {
                    User user = new User(""); // initialize user
                    
                    while (user.Name == "" && user.Name.Length <= 10)
                    {
                        // user input
                        Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 2 - 5);
                        Console.WriteLine("Enter username:");
                        Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 2 - 4);
                        string username = Console.ReadLine();
                        user.Name = username;
                    }
                    
                    difficulty = DificultyPage(difficulty); // open difficulty selection menu
                    InstructionPage(); // open instruction page

                    Food food = new Food(); // initialize Food
                    Snake snake = new Snake(); // initialize snake

                    byte right = 0;
                    byte left = 1;
                    byte down = 2;
                    byte up = 3;

                    Position[] directions = new Position[]
                    {
                        new Position(1,0), // right
                        new Position(-1,0), // left
                        new Position(0,1), // down
                        new Position(0,-1), // up
                    };

                    Console.BufferHeight = Console.WindowHeight;
                    int invinsibleTime = 0;
                    bool invinsible = false;
                    int invinsibleDeactivated = 15000;
                    int lastFoodTime = 0;
                    int foodDisapearTime = 15000;
                    double sleepTime = 100;
                    int goal = 0;
                    int NumofObstacle = 0;

                    if (difficulty == 0)
                    {
                        goal = 5;
                        NumofObstacle = 10;
                        foodDisapearTime = 15000;
                        sleepTime = 100;
                    }
                    if (difficulty == 1)
                    {
                        goal = 10;
                        NumofObstacle = 15;
                        foodDisapearTime = 10000;
                        sleepTime = 75;
                    }
                    if (difficulty == 2)
                    {
                        goal = 20;
                        NumofObstacle = 20;
                        foodDisapearTime = 5000;
                        sleepTime = 50;
                    }

                    lastFoodTime = Environment.TickCount;

                    // set number of obstacles, Initialize, Display
                    List<Position> Obstacles = new List<Position>(); // create a list to store all the obstacles

                    // create obstacles with random coordinate
                    for (int i = 0; i < NumofObstacle; i++)
                    {
                        Obstacle obs = new Obstacle();
                        while (Obstacles.Contains(new Position(obs.X, obs.Y)))
                        {
                            obs = new Obstacle();
                        }
                        Obstacles.Add(new Position(obs.X, obs.Y));
                    }

                    //print out the obstacles
                    foreach (Position obstacle in Obstacles)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.SetCursorPosition(obstacle.X, obstacle.Y);
                        Console.Write('#');
                    }

                    // draw food
                    food.CheckFoodCollision(Obstacles);
                    food.DrawFood();

                    // intilize Snake, draw snake, direction of the snake
                    snake.DrawSnake();
                    int direction = right;

                    Gameboard(user, snake); // Draw the GameBoard

                    // Play background music
                    SoundPlayer bgm1 = new SoundPlayer();
                    bgm1.SoundLocation = "../../../music/bgm1.wav";
                    bgm1.PlayLooping();

                    // Stopwatch
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    // looping
                    while (true)
                    {
                        double ts = stopwatch.Elapsed.TotalSeconds;
                        Console.SetCursorPosition(Console.WindowWidth - 30, 0);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Timer: " + ts);

                        // check for key pressed
                        if (Console.KeyAvailable)
                        {
                            ConsoleKeyInfo userInput = Console.ReadKey();
                            if (userInput.Key == ConsoleKey.LeftArrow) { if (direction != right) { direction = left; } }
                            if (userInput.Key == ConsoleKey.RightArrow) { if (direction != left) { direction = right; } }
                            if (userInput.Key == ConsoleKey.UpArrow) { if (direction != down) { direction = up; } }
                            if (userInput.Key == ConsoleKey.DownArrow) { if (direction != up) { direction = down; } }
                            if (userInput.Key == ConsoleKey.Spacebar) 
                            {
                                while (Console.ReadKey().Key != ConsoleKey.Spacebar) {
                                }
                            }
                        }

                        // update snake position
                        Position SnakeHead = snake.SnakeBody.Last();
                        Position nextDirection = directions[direction];
                        Position NewSnakeHead = new Position(SnakeHead.X + nextDirection.X, SnakeHead.Y + nextDirection.Y);

                        // check for snake if exceed the width or height
                        if (NewSnakeHead.X < 2) { NewSnakeHead.X = Console.WindowWidth - 3; }
                        if (NewSnakeHead.Y < 2) { NewSnakeHead.Y = Console.WindowHeight - 3; }
                        if (NewSnakeHead.Y >= Console.WindowHeight - 2) { NewSnakeHead.Y = 2; }
                        if (NewSnakeHead.X >= Console.WindowWidth - 2) { NewSnakeHead.X = 2; }

                        // check for snake collison with self
                        if (snake.SnakeBody.Contains(NewSnakeHead))
                        {
                            if (snake.Life > 0)
                            {
                                snake.Life--;
                                // Display updated score and life
                                Console.SetCursorPosition(Console.WindowWidth / 3 - 5, 0);
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("Score: " + user.Score + "\t" + "Life: " + snake.Life);
                                Obstacles.Remove(SnakeHead);
                            }
                            else
                            {
                                stopwatch.Stop();
                                Console.Clear();

                                // Play music
                                SoundPlayer lose = new SoundPlayer();
                                lose.SoundLocation = "../../../effect/gameover.wav";
                                lose.Play();

                                // Gameover menu (Hit Yourself)
                                Console.SetCursorPosition(Console.WindowWidth / 2 - 5, Console.WindowHeight / 3);
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Game over!");
                                Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 3 + 1);
                                Console.WriteLine("You hit yourself!");
                                Console.SetCursorPosition(Console.WindowWidth / 2 - 5, Console.WindowHeight / 3 + 2);
                                Console.WriteLine("Score: " + user.Score);
                                Console.SetCursorPosition(Console.WindowWidth / 2 - 17, Console.WindowHeight / 3 + 2);
                                Console.WriteLine("Time Used: " + ts);

                                Console.SetCursorPosition(Console.WindowWidth / 2 - 16, Console.WindowHeight / 3 + 4);
                                Console.WriteLine("Press ENTER to quit");

                                ConsoleKeyInfo keyInfo = Console.ReadKey();
                                while (keyInfo.Key != ConsoleKey.Enter)
                                {
                                    keyInfo = Console.ReadKey();
                                }
                                break;
                            }
                        }

                        // check for snake collison with obstacles
                        if (Obstacles.Contains(SnakeHead) && !invinsible)
                        {
                            // Play background music
                            SoundPlayer hurt = new SoundPlayer();
                            hurt.SoundLocation = "../../../effect/hurt.wav";
                            hurt.PlaySync();
                            if (snake.Life > 0)
                            {
                                snake.Life--;
                                // Display updated score and life
                                Console.SetCursorPosition(Console.WindowWidth / 3 - 5, 0);
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.WriteLine("Score: " + user.Score + "\t" + "Life: " + snake.Life);
                                Obstacles.Remove(SnakeHead);
                            }
                            else
                            {
                                stopwatch.Stop(); // stop the timer
                                Console.Clear();

                                // Play music
                                SoundPlayer lose = new SoundPlayer();
                                lose.SoundLocation = "../../../effect/gameover.wav";
                                lose.Play();

                                // Gameover menu (Hit Obstacle)
                                Console.SetCursorPosition(Console.WindowWidth / 2 - 5, Console.WindowHeight / 3);
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Game over!");
                                Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 3 + 1);
                                Console.WriteLine("You hit an Obstacle!");
                                Console.SetCursorPosition(Console.WindowWidth / 2 - 5, Console.WindowHeight / 3 + 2);
                                Console.WriteLine("Score: " + user.Score);
                                Console.SetCursorPosition(Console.WindowWidth / 2 - 17, Console.WindowHeight / 3 + 2);
                                Console.WriteLine("Time Used: " + ts);

                                // Press ENTER to back to Main Menu
                                Console.SetCursorPosition(Console.WindowWidth / 2 - 16, Console.WindowHeight / 3 + 4);
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("Press ENTER to back to Main Menu");

                                ConsoleKeyInfo keyInfo = Console.ReadKey();
                                while (keyInfo.Key != ConsoleKey.Enter)
                                {
                                    keyInfo = Console.ReadKey();
                                }
                                break;
                            }
                            bgm1.Play();
                        }

                        // check for collision with the food
                        if (NewSnakeHead.X == food.X && NewSnakeHead.Y == food.Y)
                        {
                            lastFoodTime = 0;
                            // Play background music
                            SoundPlayer eat = new SoundPlayer();
                            eat.SoundLocation = "../../../effect/eat.wav";

                            user.AddScore(1); // increase the score
                            snake.IncreaseSnakeBody(); // increasing the length of the snake
                            bgm1.SoundLocation = "../../../music/bgm1.wav";
                            if (food.foodType == FoodType.HEART) { 
                                snake.Life++;
                                eat.SoundLocation = "../../../effect/drink.wav";
                            }
                            if (food.foodType == FoodType.STAR)
                            {
                                invinsible = true;
                                eat.SoundLocation = "../../../effect/sparkle.wav";
                                bgm1.SoundLocation = "../../../music/bgm2.wav";
                                invinsibleTime = Environment.TickCount;
                            }
                            eat.PlaySync();

                            // Display updated score and life
                            Console.SetCursorPosition(Console.WindowWidth / 3 - 5, 0);
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("Score: " + user.Score + "\t" + "Life: " + snake.Life);

                            // spawn new food
                            food = new Food();
                            food.CheckFoodCollision(Obstacles);
                            food.DrawFood();
                            bgm1.Play();
                        }

                        ConsoleColor SnakeBodyColor = ConsoleColor.Green;
                        if (invinsible) { SnakeBodyColor = ConsoleColor.Blue; }

                        // draw the snake body
                        Console.SetCursorPosition(SnakeHead.X, SnakeHead.Y);
                        Console.ForegroundColor = SnakeBodyColor;
                        Console.Write("*");

                        // draw the snake head
                        snake.SnakeBody.Enqueue(NewSnakeHead);
                        Console.SetCursorPosition(NewSnakeHead.X, NewSnakeHead.Y);
                        Console.ForegroundColor = SnakeBodyColor;

                        // check which snake head to draw
                        if (direction == right) { Console.Write(">"); }
                        if (direction == left) { Console.Write("<"); }
                        if (direction == up) { Console.Write("^"); }
                        if (direction == down) { Console.Write("v"); }

                        // Snake moving animations and not leave his body as trail.
                        Position last = snake.SnakeBody.Dequeue();
                        Console.SetCursorPosition(last.X, last.Y);
                        Console.Write(" "); // remove one body block when the snake move forward one block

                        // set winning condition score               
                        if (user.Score == goal)
                        {
                            stopwatch.Stop(); // stop the timer
                            user.Time = ts;
                            Console.Clear(); // clear the screen

                            // Game over menu (winning)
                            Console.SetCursorPosition(Console.WindowWidth / 2 - 8, Console.WindowHeight / 3);
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Stage Clear!");
                            Console.SetCursorPosition(Console.WindowWidth / 2 - 5, Console.WindowHeight / 3 + 1);
                            Console.WriteLine("Score: " + user.Score);
                            Console.SetCursorPosition(Console.WindowWidth / 2 - 17, Console.WindowHeight / 3 + 2);
                            Console.WriteLine("Time Used: " + ts);

                            // Press ENTER to back to Main Menu
                            Console.SetCursorPosition(Console.WindowWidth / 2 - 16, Console.WindowHeight / 3 + 4);
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Press ENTER to back to Main Menu");

                            SoundPlayer success = new SoundPlayer();
                            success.SoundLocation = "../../../effect/sucess.wav";
                            success.PlaySync();

                            leaderboard.AddUser(user);
                            leaderboard.sortLeaderBoard();
                            leaderboard.StoreRecord();

                            ConsoleKeyInfo keyInfo = Console.ReadKey();
                            while (keyInfo.Key != ConsoleKey.Enter)
                            {
                                keyInfo = Console.ReadKey();
                            }
                            break;
                        }

                        if (Environment.TickCount - invinsibleTime >= invinsibleDeactivated)
                        {
                            invinsible = false;
                        }
                        else
                        {
                            //print out the obstacles
                            foreach (Position obstacle in Obstacles)
                            {
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.SetCursorPosition(obstacle.X, obstacle.Y);
                                Console.Write('#');
                            }
                        }

                        // food timer (change food location)
                        if (Environment.TickCount - lastFoodTime >= foodDisapearTime)
                        {
                            Console.SetCursorPosition(food.X, food.Y);
                            Console.Write(" "); // remove previous food
                            food = new Food();
                            food.CheckFoodCollision(Obstacles);
                            food.DrawFood(); // display the new food
                            lastFoodTime = Environment.TickCount;
                        }

                        sleepTime -= 0.01;
                        Thread.Sleep((int)sleepTime);
                    }
                    Console.Clear();
                }
                if (option == 1)
                {
                    Console.Clear();
                    leaderboard.DisplayRecord();
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 15, 0);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Press Enter to Back to Main Menu");
                    ConsoleKeyInfo keyInfo = Console.ReadKey();
                    while (keyInfo.Key != ConsoleKey.Enter)
                    {
                        keyInfo = Console.ReadKey();
                    }
                    Console.Clear();
                }
                if (option == 2) 
                {
                    break;
                }
            }
        }

        static int MainMenu()
        {
            int count = 0;
            String[] Menu = { "Play", "LeaderBoard", "Quit" };
            DisplayMenu(Menu, count);

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo userInput = Console.ReadKey();
                    if (userInput.Key == ConsoleKey.UpArrow)
                    {
                        if (count == 0) { count = 2; }
                        else { count--; }
                        DisplayMenu(Menu, count);
                    }
                    if (userInput.Key == ConsoleKey.DownArrow)
                    {
                        if (count == 2) { count = 0; }
                        else { count++; }
                        DisplayMenu(Menu, count);
                    }
                    if (userInput.Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                        break;
                    }
                }
            }
            return count;
        }

        static int DificultyPage(int count)
        {
            String[] Menu = { "Easy mode", "Normal mode", "Hard mode" };
            DisplayMenu(Menu, count);

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo userInput = Console.ReadKey();
                    if (userInput.Key == ConsoleKey.UpArrow)
                    {
                        if (count == 0) { count = 2; }
                        else { count--; }
                        DisplayMenu(Menu, count);
                    }
                    if (userInput.Key == ConsoleKey.DownArrow)
                    {
                        if (count == 2) { count = 0; }
                        else { count++; }
                        DisplayMenu(Menu, count);
                    }
                    if (userInput.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                }
            }
            return count;
        }

        static void DisplayMenu(String[] Menu, int selection)
        {
            Console.Clear();
            Console.SetCursorPosition(Console.WindowWidth / 2 - 11, Console.WindowHeight / 2 - 5);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Snake Game");
            for (int i = 0; i < Menu.Length; i++)
            {
                Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 2 + i);
                if (i == selection)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(Menu[i]);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Menu[i]);
                }
            }
        }

        static void InstructionPage()
        {
            Console.Clear(); // clear screen

            // Print Instructions First
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Snake Game\n");
            Console.WriteLine("Eat one food get one point!");
            Console.WriteLine("Get 5 point to win!");
            Console.WriteLine("Watch OUT For the obstacle!");
            Console.WriteLine("Food will change location, so be Quick!");
            Console.WriteLine("\n Press ENTER to continue");
            ConsoleKeyInfo keyInfos = Console.ReadKey();
            while (keyInfos.Key != ConsoleKey.Enter)
            {
                keyInfos = Console.ReadKey();
            }
            Console.Clear(); // clear screen
        }

        static void Gameboard(User user, Snake snake)
        {
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = 2; i < Console.WindowWidth - 2; i++)
            {
                Console.SetCursorPosition(i, 1);
                Console.Write("-");
            }
            for (int i = 2; i < Console.WindowWidth - 2; i++)
            {
                Console.SetCursorPosition(i, Console.WindowHeight - 2);
                Console.Write("-");
            }
            for (int i = 2; i < Console.WindowHeight - 2; i++)
            {
                Console.SetCursorPosition(Console.WindowWidth - 2, i);
                Console.Write("|");
            }
            for (int i = 2; i < Console.WindowHeight - 2; i++)
            {
                Console.SetCursorPosition(1, i);
                Console.Write("|");
            }

            // Draw the Score and life
            Console.SetCursorPosition(Console.WindowWidth / 3 - 5, 0);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Score: " + user.Score + "\t" + "Life: " + snake.Life);
        }
    }
}
