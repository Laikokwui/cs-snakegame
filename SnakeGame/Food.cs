using System;
using System.Collections.Generic;

namespace SnakeGame
{
    public enum FoodType
    {
        DEFAULT,
        HEART,
        STAR
    }

    public class Food
    {
        public int X { get; set; }
        public int Y { get; set; }
        
        public FoodType foodType { get; set; }

        public Food()
        {
            Random random = new Random();
            this.X = random.Next(3, Console.WindowWidth - 3);
            this.Y = random.Next(3, Console.WindowHeight - 3);
            this.foodType = (FoodType)random.Next(0, 3);
        }

        // for testing purposes as i need to import in random class and custom width and height
        public Food(int width, int height, Random random)
        {
            this.X = random.Next(0, width);
            this.Y = random.Next(0, height);
            this.foodType = (FoodType)random.Next(0, 3);
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

        // for testing purposes as i need to import in random class and custom width and height
        public void CheckFoodCollision(List<Position> obs, int width, int height, Random random)
        {
            // Check is it collapse the obstacle
            while (obs.Contains(new Position(this.X, this.Y)))
            {
                this.X = random.Next(0, width);
                this.Y = random.Next(0, height);
            }
        }

        // Draw out the food
        public void DrawFood()
        {
            Console.SetCursorPosition(this.X, this.Y);
            if (this.foodType == FoodType.DEFAULT)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write('@');
            }
            if (this.foodType == FoodType.HEART)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write('&');
            }
            if (this.foodType == FoodType.STAR)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write('$');
            }
        }
    }
}
