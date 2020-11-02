using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Media;

namespace Snake
{
    class Program
    {
        static void Main(string[] args)
        {
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
            Console.Clear();

            byte right = 0;
            byte left = 1;
            byte down = 2;
            byte up = 3;

            int lastFoodTime = 0;
            int foodDissapearTime = 16000;

            Position[] directions = new Position[]
            {
                new Position(1,0), // right
                new Position(-1,0), // left
                new Position(0,1), // down
                new Position(0,-1), // up
            };

            Console.BufferHeight = Console.WindowHeight;
            double sleepTime = 100; // time when the food stay at the same place
            lastFoodTime = Environment.TickCount;

            int score = 0; // score
            int goal = 5; // goal

            // Draw the GameBoard
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

            // Draw the Score
            Console.SetCursorPosition(Console.WindowWidth / 2 - 5, 0);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Score: " + score);

            // set number of obstacles, Initialize, Display
            int NumofObstacle = 10; 
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
            Food food = new Food(); // initialize Food
            food.CheckFoodCollision(Obstacles);
            food.DrawFood();

            // intilize Snake, draw snake, direction of the snake
            Snake snake = new Snake();
            snake.DrawSnake();
            int direction = right;

            // Play background music
            SoundPlayer bgm1 = new SoundPlayer();
            bgm1.SoundLocation = "../../../music/bgm1.wav";
            bgm1.PlayLooping();

            // looping
            while (true)
            {
                

                // check for key pressed
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo userInput = Console.ReadKey();
                    if (userInput.Key == ConsoleKey.LeftArrow)
                    {
                        if (direction != right) { direction = left; }
                    }
                    if (userInput.Key == ConsoleKey.RightArrow)
                    {
                        if (direction != left) { direction = right; }
                    }
                    if (userInput.Key == ConsoleKey.UpArrow)
                    {
                        if (direction != down) { direction = up; }
                    }
                    if (userInput.Key == ConsoleKey.DownArrow)
                    {
                        if (direction != up) { direction = down; }
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
                    Console.Clear();
                    // Gameover menu (Hit Yourself)
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 5, Console.WindowHeight / 3);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Game over!");
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 3 + 1);
                    Console.WriteLine("You hit yourself!");
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 5, Console.WindowHeight / 3 + 2);
                    Console.WriteLine("Score: " + score);
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 3 + 4);
                    Console.WriteLine("Press ENTER to quit");
                    ConsoleKeyInfo keyInfo = Console.ReadKey();
                    while (keyInfo.Key != ConsoleKey.Enter)
                    {
                        keyInfo = Console.ReadKey();
                    }
                    Environment.Exit(0);
                    return;
                }

                // check for snake collison with obstacles
                if (Obstacles.Contains(SnakeHead))
                {
                    Console.Clear();
                    // Gameover menu (Hit Obstacle)
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 5, Console.WindowHeight / 3);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Game over!");
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 3 + 1);
                    Console.WriteLine("You hit an Obstacle!");
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 5, Console.WindowHeight / 3 + 2);
                    Console.WriteLine("Score: " + score);
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 3 + 4);
                    Console.WriteLine("Press ENTER to quit");
                    ConsoleKeyInfo keyInfo = Console.ReadKey();
                    while (keyInfo.Key != ConsoleKey.Enter)
                    {
                        keyInfo = Console.ReadKey();
                    }
                    Environment.Exit(0);
                    return;
                }

                // check for collision with the food
                if (NewSnakeHead.X == food.X && NewSnakeHead.Y == food.Y)
                {
                    // Play background music
                    SoundPlayer eat = new SoundPlayer();
                    eat.SoundLocation = "../../../effect/eat.wav";
                    eat.Play();
                    
                    score += 1; // increase the score
                    snake.IncreaseSnakeBody(); // increasing the length of the snake

                    // Display updated score
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 5, 0);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Score: " + score);

                    // spawn new food
                    food = new Food();
                    food.CheckFoodCollision(Obstacles);
                    food.DrawFood();
                }

                // draw the snake body
                Console.SetCursorPosition(SnakeHead.X, SnakeHead.Y);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("*");

                // draw the snake head
                snake.SnakeBody.Enqueue(NewSnakeHead);
                Console.SetCursorPosition(NewSnakeHead.X, NewSnakeHead.Y);
                Console.ForegroundColor = ConsoleColor.Green;
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
                if (score == goal)
                {
                    Console.Clear(); // clear the screen

                    // Game over menu (winning)
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 8, Console.WindowHeight / 3);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Stage Clear!");
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 5, Console.WindowHeight / 3 + 1);
                    Console.WriteLine("Score: " + score);

                    //Press enter to quit
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 3 + 3);
                    Console.WriteLine("Press ENTER to quit");
                    ConsoleKeyInfo keyInfo = Console.ReadKey();

                    while (keyInfo.Key != ConsoleKey.Enter)
                    {
                        keyInfo = Console.ReadKey();
                    }
                    Environment.Exit(0);
                    return;
                }

                // food timer (change food location)
                if (Environment.TickCount - lastFoodTime >= foodDissapearTime)
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
        }
    }
}
