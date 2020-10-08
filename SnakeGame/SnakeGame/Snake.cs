using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeGame
{
    public class Snake
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Score { get; set; }
        public Queue<Position> SnakeLength { get; }

        public Snake(int score)
        {
            this.X = 0;
            this.Y = 0;
            Score = score;
            SnakeLength = new Queue<Position>();
        }

        public void DrawSnake()
        {
            for (int i = 0; i <= 3; i++)
            {
                SnakeLength.Enqueue(new Position(0, i));
            }
        }
    }
}
