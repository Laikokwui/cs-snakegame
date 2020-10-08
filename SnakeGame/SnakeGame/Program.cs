using System;
using System.Linq;

namespace SnakeGame
{
    class Program
    {
        static void Main(string[] args)
        {
            byte left = 0;
            byte right = 1;
            byte up = 2;
            byte down = 3;

            GameBoard board = new GameBoard(79,24);
            Food food = new Food(board);
            Snake snake = new Snake(0);

            Position[] directions = new Position[]
            {
                new Position(0,-1), // left
                new Position(0,1), // right
                new Position(-1,0), // up
                new Position(1,0), // down
            };

            // start game
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            
            bool gameLive = true;

            ConsoleKeyInfo consoleKey; // holds whatever key is pressed

            int x = 0, y = 2; // y is 2 to allow the top row for directions & space

            Console.BackgroundColor = ConsoleColor.DarkGray;  // clear to color
            Console.Clear();

            food.DrawFood(); 
            snake.DrawSnake();
            int direction = right;

            int delayInMillisecs = 100;  // delay to slow down the character movement so you can see it

            do
            {
                ConsoleColor cc = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(0, 0);

                Console.WriteLine("Arrows move up/down/right/left. Press 'esc' quit.");
                Console.WriteLine("Score: " + snake.Score);

                Console.SetCursorPosition(x, y); 
                Console.ForegroundColor = cc; // set color

                if (Console.KeyAvailable) // check key input
                {
                    consoleKey = Console.ReadKey(true);
                    switch (consoleKey.Key)
                    {
                        case ConsoleKey.UpArrow: // UP
                            if (direction != down) direction = up;
                            break;
                        case ConsoleKey.DownArrow: // DOWN
                            if (direction != up) direction = down;
                            break;
                        case ConsoleKey.LeftArrow: // LEFT
                            if (direction != right) direction = left;
                            break;
                        case ConsoleKey.RightArrow: // RIGHT
                            if (direction != left) direction = right;
                            break;
                        case ConsoleKey.Escape: // END
                            gameLive = false; // Game Over
                            break;
                    }
                }
                
                Console.SetCursorPosition(x, y); // find the current position in the console grid

                Position SnakeHead = snake.SnakeLength.Last();
                Position NextDirection = directions[direction];
                Position NewSnakeHead = new Position(SnakeHead.row + NextDirection.row, SnakeHead.column + NextDirection.column);

                snake.X += 1; // move horizontally
                if (NewSnakeHead.column < 0)
                {
                    NewSnakeHead.column = 1; // set snake head position before end to avoid error
                    gameLive = false; // Game Over
                }
                if (NewSnakeHead.row < 0)
                {
                    NewSnakeHead.row = 1; // set snake head position before end to avoid error
                    gameLive = false; // Game Over
                }
                snake.Y += 1; // move vertically
                if (NewSnakeHead.row >= board.Y) { gameLive = false; } // touch right edge
                if (NewSnakeHead.column >= board.X) { gameLive = false; } // touch bottom edge

                // draw the snake
                Console.SetCursorPosition(SnakeHead.column, SnakeHead.row);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write('*');

                snake.SnakeLength.Enqueue(NewSnakeHead);
                Console.SetCursorPosition(NewSnakeHead.column, NewSnakeHead.row);
                Console.ForegroundColor = ConsoleColor.Gray;

                // snake head in different direction
                if (direction == right) { Console.Write(">"); }
                if (direction == left) { Console.Write("<"); }
                if (direction == up) { Console.Write("^"); }
                if (direction == down) { Console.Write("v"); }

                // get the snake head by 
                //Position last = snake.SnakeLength.Dequeue();
                //Console.SetCursorPosition(last.column, last.row);
                //Console.Write(' ');

                // if snakehead hit the food
                if (NewSnakeHead.column == food.X && NewSnakeHead.row == food.Y)
                {
                    snake.Score += 1; // add score
                    food = new Food(board); // get new food position
                    food.DrawFood(); // print out the food inn new position
                }
         
                System.Threading.Thread.Sleep(delayInMillisecs); // delay

            } while (gameLive);

            Console.Clear(); // clear the console screen
            Console.ForegroundColor = ConsoleColor.Black; // text color black
            Console.BackgroundColor = ConsoleColor.White; // background color white
            Console.WriteLine("GAME OVER"); // game over message
            Console.WriteLine("Total Score: " + snake.Score); // total score
        }
    }
}
