using System.Collections.Generic;
using System.Linq;

namespace Snake
{
    class Snake
    {
        public Queue<Position> SnakeBody { get; set; }
        public int Life { get; set; }

        public Snake()
        {
            SnakeBody = new Queue<Position>();
        }

        //draw the snake
        public void DrawSnake()
        {
            for (int i = 0; i <= 3; i++)
            {
                SnakeBody.Enqueue(new Position(i + 3, 2));
            }
        }

        //increase the length of the snake by 1
        public void IncreaseSnakeBody()
        {
            Position temp = new Position(SnakeBody.Last().X + 1, SnakeBody.Last().Y);
            SnakeBody.Enqueue(temp);
        }
    }
}
