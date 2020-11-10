using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Media;
using SnakeGame;
using System.Diagnostics;

namespace Snake
{
    class Program
    {
        static void Main(string[] args)
        {
            Leaderboard leaderboard = new Leaderboard();
            leaderboard.ImportRecord();
            leaderboard.StoreRecord();

            bool trail = false; // leave trail
            bool invincible = false; // invincible effect
            bool backToMenu = false; 
            int invincibleTime = 0;
            int invincibleDeactivated = 0;
            int lastFoodTime = 0;
            int foodDisapearTime = 0;
            int goal = 0;
            int NumofObstacle = 0;
            double sleepTime = 0;

            while (true)
            {
                int option = MainMenu();

                if (option == 0)
                {
                    User user = new User(""); // initialize user

                    while (user.Name == "" || user.Name.Length > 6)
                    {
                        Console.Clear();

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.SetCursorPosition(Console.WindowWidth / 2 - 15, Console.WindowHeight - 10);
                        Console.WriteLine("Username cannot be empty!");
                        Console.SetCursorPosition(Console.WindowWidth / 2 - 20, Console.WindowHeight - 11);
                        Console.WriteLine("Maximum 6 Letters for the Username!");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.SetCursorPosition(Console.WindowWidth / 2 - 15, 7);
                        Console.WriteLine("Insert \"/q\" to Main Menu");

                        // user input
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 2 - 5);
                        Console.WriteLine("Enter username:");
                        
                        Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 2 - 4);
                        string username = Console.ReadLine();
                        user.Name = username;
                    }

                    if (user.Name != "/q")
                    {
                        int difficulty = DificultyPage(); // open difficulty selection menu

                        if (difficulty == 0) // easy mode setting
                        {
                            goal = 5;
                            NumofObstacle = 10;
                            foodDisapearTime = 15000;
                            invincibleDeactivated = 20000;
                            sleepTime = 100;
                        }
                        if (difficulty == 1) // normal mode setting
                        {
                            goal = 10;
                            NumofObstacle = 15;
                            foodDisapearTime = 10000;
                            invincibleDeactivated = 15000;
                            sleepTime = 75;
                        }
                        if (difficulty == 2) // hard mode setting
                        {
                            goal = 20;
                            NumofObstacle = 20;
                            foodDisapearTime = 5000;
                            invincibleDeactivated = 10000;
                            sleepTime = 50;
                        }

                        InstructionPage(goal, difficulty); // open instruction page

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

                        lastFoodTime = Environment.TickCount; // food timer start

                        List<Position> Obstacles = new List<Position>(); // create a list to store all the obstacles

                        for (int i = 0; i < NumofObstacle; i++) // create obstacles with random coordinate
                        {
                            Obstacle obs = new Obstacle();
                            while (Obstacles.Contains(new Position(obs.X, obs.Y))) { obs = new Obstacle(); } // create obstacle
                            Obstacles.Add(new Position(obs.X, obs.Y));
                        }

                        foreach (Position obstacle in Obstacles) // print out the obstacles
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.SetCursorPosition(obstacle.X, obstacle.Y);
                            Console.Write('#');
                        }

                        food.CheckFoodCollision(Obstacles); // check food collision with obstacle
                        snake.DrawSnake(); // draw snake
                        int direction = right; // direction of the snake

                        // load music
                        SoundPlayer lose = new SoundPlayer();
                        lose.SoundLocation = "../../../effect/gameover.wav";

                        SoundPlayer hurt = new SoundPlayer();
                        hurt.SoundLocation = "../../../effect/hurt.wav";

                        SoundPlayer eat = new SoundPlayer();
                        eat.SoundLocation = "../../../effect/eat.wav";

                        SoundPlayer success = new SoundPlayer();
                        success.SoundLocation = "../../../effect/sucess.wav";

                        SoundPlayer bgm1 = new SoundPlayer();
                        bgm1.SoundLocation = "../../../music/bgm1.wav";

                        bgm1.Play(); // Play background music

                        // Stopwatch
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();

                        while (true) // game loop
                        {
                            Gameboard(user, snake); // Draw the GameBoard
                            food.DrawFood(); // draw food

                            // show timer
                            double ts = stopwatch.Elapsed.TotalSeconds;
                            Console.SetCursorPosition(Console.WindowWidth - 30, 0);
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("Timer: " + Math.Round(ts, 5));

                            if (Console.KeyAvailable) // check for key pressed
                            {
                                ConsoleKeyInfo userInput = Console.ReadKey();
                                if (userInput.Key == ConsoleKey.LeftArrow) { if (direction != right) { direction = left; } }
                                if (userInput.Key == ConsoleKey.RightArrow) { if (direction != left) { direction = right; } }
                                if (userInput.Key == ConsoleKey.UpArrow) { if (direction != down) { direction = up; } }
                                if (userInput.Key == ConsoleKey.DownArrow) { if (direction != up) { direction = down; } }
                                if (userInput.Key == ConsoleKey.Spacebar)
                                {
                                    Console.Clear();
                                    PausedMenu(user, ts, "You Paused the Game", "Paused Menu");
                                    stopwatch.Stop();
                                    bgm1.Stop();
                                    while (Console.ReadKey().Key != ConsoleKey.Spacebar) {
                                        if (Console.ReadKey().Key == ConsoleKey.Enter) {
                                            backToMenu = true;
                                            break;
                                        }
                                    }
                                    if (backToMenu) { break; } // back to main menu
                                    Console.Clear();
                                    lastFoodTime = Environment.TickCount;
                                    invincibleTime = Environment.TickCount;
                                    stopwatch.Start();
                                    bgm1.Play();
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

                            if (snake.SnakeBody.Contains(NewSnakeHead)) // check for snake collison with self
                            {
                                if (snake.Life > 0) // check snake life
                                {
                                    snake.Life--; // minus snake life
                                    GameState(user, snake); // Display updated score and life
                                    Obstacles.Remove(SnakeHead); // destroy obstacle
                                }
                                else
                                {
                                    stopwatch.Stop(); // stop timer
                                    Console.Clear(); // clear screen
                                    lose.Play(); // Play music
                                    GameOverMenu(user, ts, "You Hit Yourself!", "Game Over!"); // display game over menu
                                    ConsoleKeyInfo keyInfo = Console.ReadKey(); // read key
                                    while (keyInfo.Key != ConsoleKey.Enter) { keyInfo = Console.ReadKey(); }
                                    break;
                                }
                            }
                            
                            if (Obstacles.Contains(SnakeHead) && !invincible) // check for snake collison with obstacles
                            {
                                hurt.PlaySync(); // Play background music

                                if (snake.Life > 0) // check snake life
                                {
                                    snake.Life--;
                                    GameState(user, snake); // Display updated score and life
                                    Obstacles.Remove(SnakeHead); // destroy the obstacle
                                    bgm1.Play(); // play bgm
                                }
                                else
                                {
                                    stopwatch.Stop(); // stop the timer
                                    Console.Clear(); // clear screen
                                    lose.Play(); // Play music
                                    GameOverMenu(user, ts, "You Hit an Obstacle!", "Game Over!"); // display game over menu
                                    ConsoleKeyInfo keyInfo = Console.ReadKey();
                                    while (keyInfo.Key != ConsoleKey.Enter) { keyInfo = Console.ReadKey(); }
                                    break;
                                }
                            }

                            if (NewSnakeHead.X == food.X && NewSnakeHead.Y == food.Y) // check for collision with the food
                            {
                                lastFoodTime = 0;
                                user.Score++; // increase the score
                                snake.IncreaseSnakeBody(); // increasing the length of the snake
                                bgm1.SoundLocation = "../../../music/bgm1.wav";

                                if (food.foodType == FoodType.HEART)
                                {
                                    snake.Life++;
                                    eat.SoundLocation = "../../../effect/drink.wav"; // change eat sound to drink
                                }
                                if (food.foodType == FoodType.STAR)
                                {
                                    invincible = true;
                                    eat.SoundLocation = "../../../effect/sparkle.wav"; // change eat sound to sparkle
                                    bgm1.SoundLocation = "../../../music/bgm2.wav"; // change bgm sound to bgm2
                                    invincibleTime = Environment.TickCount;
                                }
                                eat.PlaySync(); // Play eat music
                                GameState(user, snake); // Display game state
                                food = new Food(); // spawn new food
                                food.CheckFoodCollision(Obstacles); // check collision
                                food.DrawFood(); // display food
                                bgm1.Play(); // play background music
                            }

                            if (user.Score == goal) // set winning condition score 
                            {
                                stopwatch.Stop(); // stop the timer
                                user.Time = Math.Round(ts, 5);
                                Console.Clear(); // clear the screen
                                GameOverMenu(user, ts, "You Reach Goal State!", "Stage Clear!"); // display game over menu
                                success.Play(); // play music
                                leaderboard.AddUser(user); // add into leaderboard
                                leaderboard.StoreRecord(); // store user record into txt file
                                ConsoleKeyInfo keyInfo = Console.ReadKey(); // read key
                                while (keyInfo.Key != ConsoleKey.Enter) { keyInfo = Console.ReadKey(); } // loop until press enter
                                break; // stop game loop
                            }

                            // Snake Body color
                            ConsoleColor SnakeBodyColor = ConsoleColor.Green;
                            if (invincible) { SnakeBodyColor = ConsoleColor.Blue; }

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
                            if (!trail) { Console.Write(" "); } // trail checker

                            // Remove Invincible effect
                            if (Environment.TickCount - invincibleTime >= invincibleDeactivated) { invincible = false; }
                            
                            if (Environment.TickCount - lastFoodTime >= foodDisapearTime) // Change food location
                            {
                                Console.SetCursorPosition(food.X, food.Y);
                                Console.Write(" "); // remove previous food
                                food = new Food();
                                food.CheckFoodCollision(Obstacles);
                                food.DrawFood(); // display the new food
                                lastFoodTime = Environment.TickCount;
                            }

                            foreach (Position obstacle in Obstacles) // Obstacle would not get overwrite by snake
                            {
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.SetCursorPosition(obstacle.X, obstacle.Y);
                                Console.Write('#');
                            }

                            sleepTime -= 0.01;
                            Thread.Sleep((int)sleepTime);
                        }
                    }
                    Console.Clear();
                }
                if (option == 1)
                {
                    Console.Clear();
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 10, 3);
                    Console.WriteLine("Timer" + '\t' + '\t' + "Username" + " Score");
                    leaderboard.DisplayRecord();
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 15, 0);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Press Enter to Back to Main Menu");
                    ConsoleKeyInfo keyInfo = Console.ReadKey();
                    while (keyInfo.Key != ConsoleKey.Enter) { keyInfo = Console.ReadKey(); }
                    Console.Clear();
                }
                if (option == 2) {  break; }
            }
        }

        static int MainMenu()
        {
            int count = 0; // default selection
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

        static int DificultyPage()
        {
            int count = 1; // default selection
            string[] Menu = { "Easy mode", "Normal mode", "Hard mode" };
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
                    if (userInput.Key == ConsoleKey.Enter) { break; }
                }
            }
            return count;
        }

        static void DisplayMenu(string[] Menu, int selection)
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

        static void InstructionPage(int goal, int difficult)
        {
            Console.Clear(); // clear screen

            string difficulty = "";
            if (difficult == 0) { difficulty = "Easy Mode"; }
            if (difficult == 1) { difficulty = "Normal Mode"; }
            if (difficult == 2) { difficulty = "Hard Mode"; }

            // Display Instructions
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Snake Game - " + difficulty + "\n");
            Console.WriteLine("One point PER food");
            Console.WriteLine("Get " + goal.ToString() + " point to win!");
            Console.WriteLine("Watch OUT For the obstacle!");
            Console.WriteLine("Food will change location, so be Quick!");
            Console.WriteLine("\nHow to Play:");
            Console.WriteLine("Left Arrow Key - Move Left");
            Console.WriteLine("Up Arrow Key - Move Up");
            Console.WriteLine("Right Arrow Key - Move Right");
            Console.WriteLine("Down Arrow Key - Move Down");
            Console.WriteLine("Spacebar - Paused or Resume");
            Console.WriteLine("\n FoodType");
            Console.WriteLine("$ - Star (gives invincibility)");
            Console.WriteLine("& - Heart (give one snake life)");
            Console.WriteLine("@ - Default (no effect)");
            Console.WriteLine("\n Press ENTER to continue");

            ConsoleKeyInfo keyInfo = Console.ReadKey();
            while (keyInfo.Key != ConsoleKey.Enter) { keyInfo = Console.ReadKey(); }

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

            GameState(user, snake); // Draw the Score and life
        }

        static void GameState(User user, Snake snake)
        {
            Console.SetCursorPosition(2, 0);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Username: " + user.Name + "\t" + "Score: " + user.Score + "\t" + "Life: " + snake.Life);
        }

        static void GameOverMenu(User user, double ts, string outcome, string menutype)
        {
            // Game over menu (winning)
            Console.SetCursorPosition(Console.WindowWidth / 2 - 8, Console.WindowHeight / 3);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(menutype);
            Console.SetCursorPosition(Console.WindowWidth / 2 - 15, Console.WindowHeight / 3 + 2);
            Console.WriteLine(outcome);
            Console.SetCursorPosition(Console.WindowWidth / 2 - 13, Console.WindowHeight / 3 + 3);
            Console.WriteLine("Score: " + user.Score);
            Console.SetCursorPosition(Console.WindowWidth / 2 - 14, Console.WindowHeight / 3 + 4);
            Console.WriteLine("Time Used: " + Math.Round(ts, 5));

            // Press ENTER to back to Main Menu
            Console.SetCursorPosition(Console.WindowWidth / 2 - 16, Console.WindowHeight / 3 + 6);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Press ENTER to back to Main Menu");
        }

        static void PausedMenu(User user, double ts, string outcome, string menutype)
        {
            // Paused menu (winning)
            Console.SetCursorPosition(Console.WindowWidth / 2 - 8, Console.WindowHeight / 3);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(menutype);
            Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 3 + 2);
            Console.WriteLine(outcome);
            Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 3 + 3);
            Console.WriteLine("Current Score: " + user.Score);
            Console.SetCursorPosition(Console.WindowWidth / 2 - 14, Console.WindowHeight / 3 + 4);
            Console.WriteLine("Current Time: " + Math.Round(ts, 5));

            // Press space to resume
            Console.SetCursorPosition(Console.WindowWidth / 2 - 16, Console.WindowHeight / 3 + 6);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Press Space to resume");

            // Press ENTER to back to Main Menu
            Console.SetCursorPosition(Console.WindowWidth / 2 - 16, Console.WindowHeight / 3 + 8);
            Console.WriteLine("Press ENTER (Twice) to back to Main Menu");
        }
    }
}
