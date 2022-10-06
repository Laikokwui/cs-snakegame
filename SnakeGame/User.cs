
namespace SnakeGame
{
    public class User
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public double Time { get; set; }

        public User(string name)
        {
            Name = name;
            Score = 0;
        }

        public User(string name, int score, double time)
        {
            Name = name;
            Score = score;
            Time = time;
        }
    }
}
