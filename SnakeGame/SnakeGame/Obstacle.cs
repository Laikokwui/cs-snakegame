using System;

namespace Snake
{
    class Obstacle
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Obstacle()
        {
            Random random = new Random();
            this.X = random.Next(3, Console.WindowWidth - 3);
            this.Y = random.Next(3, Console.WindowHeight - 3);
        }
    }
}