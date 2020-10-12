using System;
using System.Collections.Generic;

namespace Snake
{
    class Food
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Food()
        {
            Random random = new Random();
            this.X = random.Next(3, Console.WindowWidth - 3);
            this.Y = random.Next(3, Console.WindowHeight - 3);
        }

        // generate a new food location
        public void CheckFoodCollision(List<Position> obs)
        {
            Random random = new Random();
            // Check is it collapse the obstacle
            while (obs.Contains(new Position(this.X, this.Y)))
            {
                this.X = random.Next(3, Console.WindowWidth - 3);
                this.Y = random.Next(3, Console.WindowHeight - 3);
            }
        }

        // Draw out the food
        public void DrawFood()
        {
            Console.SetCursorPosition(this.X, this.Y);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write('@');
        }
    }
}
