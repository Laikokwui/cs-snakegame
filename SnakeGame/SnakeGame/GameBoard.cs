using System;
namespace SnakeGame
{
    public class GameBoard
    {
        public int X { get; set; }
        public int Y { get; set; }

        public GameBoard(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
