using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SnakeGame;
using System.IO;
using System.Collections.Generic;

namespace SnakeGameTest
{
    [TestClass]
    public class snakeGameTest
    {
        // check leaderboard is sort properly according to the timer
        [TestMethod]
        public void TestSortLeaderboard()
        {
            // Expected output
            string FirstRecordExpected = "27.86647-teoh-5";
            string SecondRecordExpected = "47.72789-ian-10";
            string ThirdRecordExpected = "50.18213-kokwui-10";

            // create three user
            User user1 = new User("kokwui", 10, 50.18213);
            User user2 = new User("teoh", 5, 27.86647);
            User user3 = new User("ian", 10, 47.72789);

            // create a leaderboard
            Leaderboard leaderboard = new Leaderboard();

            // add user into leaderboard
            leaderboard.AddUser(user1);
            leaderboard.AddUser(user2);
            leaderboard.AddUser(user3);

            // sort by time
            IEnumerable<User> sorted_userlist = leaderboard.SortUser();

            // empty array act as the text file
            String[] records = { "", "", "" };

            // int for for loop
            int i = 0;

            // store the sorted record into array
            foreach (User user in sorted_userlist)
            {
                records[i] = user.Time.ToString() + "-" + user.Name + "-" + user.Score.ToString();
                i++;
            }

            // Check output
            Assert.AreEqual(FirstRecordExpected, records[0]);
            Assert.AreEqual(SecondRecordExpected, records[1]);
            Assert.AreEqual(ThirdRecordExpected, records[2]);
        }

        // test whether food can generate and overlap the obstacle
        [TestMethod]
        public void TestFoodOverlapObstacle()
        {
            // 5x2 space which mean only have 10 space to choose
            int width = 5;
            int height = 2;
            Random random = new Random();

            // list of obstacle
            List<Position> obstacles = new List<Position>();

            // generate obstacle
            obstacles = Program.GenerateObstacle(obstacles, 9, width, height, random);

            // generate new food
            Food food = new Food(width, height, random);

            // check food overlap with obstacle and 
            food.CheckFoodCollision(obstacles, width, height, random);

            // get food position
            Position foodpos = new Position(food.X, food.Y);

            // check is it overlap the position.
            bool contain = obstacles.Contains(foodpos);

            Assert.AreEqual(false, contain); // expected to be false because food cannot be overlap with obstacle
        }

        // test whether obstacle generate differently
        [TestMethod]
        public void TestGenerateObstacle()
        {
            int width = 50;
            int height = 20;
            Random random = new Random();

            // list of obstacle
            List<Position> ObstacleSet1 = new List<Position>();
            List<Position> ObstacleSet2 = new List<Position>();

            // generate obstacle
            ObstacleSet1 = Program.GenerateObstacle(ObstacleSet1, 10, width, height, random);
            ObstacleSet2 = Program.GenerateObstacle(ObstacleSet2, 10, width, height, random);

            // check both of the obstacle set is the same
            bool same = ObstacleSet1.Equals(ObstacleSet2);

            Assert.AreEqual(false, same); // expected to be false because obstacle set must be different
        }
    }
}
