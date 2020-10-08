using System;
namespace SnakeGame
{
    public class Food
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Food(GameBoard board)
        {
            Random random = new Random();
            this.X = random.Next(2, board.X - 2);
            this.Y = random.Next(2, board.Y - 2);
        }

        public void DrawFood()
        {
            Console.SetCursorPosition(this.X, this.Y);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write('@');
        }
    }
}
