using System;
using System.Collections.Generic;
using System.Text;

namespace SnakeGame
{
    class User
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public double Time { get; set; }

        public User(string name)
        {
            Name = name;
            Score = 0;
        }

        // increase the user score
        public void AddScore(int score)
        {
            Score += score;
        }
    }
}
