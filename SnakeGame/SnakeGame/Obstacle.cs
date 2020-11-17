using System;

namespace SnakeGame
{
    public class Obstacle
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Obstacle()
        {
            Random random = new Random();
            this.X = random.Next(3, Console.WindowWidth - 3);
            this.Y = random.Next(3, Console.WindowHeight - 3);
        }

        // for testing purposes as i need to import in random class and custom width and height
        public Obstacle(int width, int height, Random random)
        {
            this.X = random.Next(0, width);
            this.Y = random.Next(0, height);
        }
    }
}