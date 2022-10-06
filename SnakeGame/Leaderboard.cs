using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace SnakeGame
{
    public class Leaderboard
    {
        private List<User> userlist;

        public Leaderboard()
        {
            userlist = new List<User>();
        }

        // add user into record
        public void AddUser(User user)
        {
            userlist.Add(user);
        }

        // display record
        public void DisplayRecord()
        {
            var path = "../../../textfile/leaderboard.txt";
            
            using (StreamReader file = new StreamReader(path))
            {
                int row = 10;
                int count = 1;
                string ln;

                while ((ln = file.ReadLine()) != null)
                {
                    if (row == -10) { break; }
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 12, Console.WindowHeight / 2 - row);
                    Console.WriteLine(count.ToString() + ". " + ln);
                    count++;
                    row--;
                }
            }
        }

        // add userlist record into the text file
        public void StoreRecord()
        {
            var path = "../../../textfile/leaderboard.txt";

            // before store record, sort by timer
            IEnumerable<User> sorted_userlist = SortUser();

            using (StreamWriter writer = new StreamWriter(path))
            {
                foreach (User user in sorted_userlist)
                {
                    writer.WriteLine(user.Time.ToString() + '\t' + user.Name + '\t' + user.Score.ToString());
                }
            }
        }

        public IEnumerable<User> SortUser()
        {
            return userlist.OrderBy(user => user.Time);
        }

        public void ImportRecord()
        {
            var path = "../../../textfile/leaderboard.txt";
            using (StreamReader file = new StreamReader(path))
            {
                string ln;
                while ((ln = file.ReadLine()) != null)
                {
                    string[] record = ln.Split('\t');
                    User user = new User(record[1], int.Parse(record[2].Trim()), double.Parse(record[0]));
                    AddUser(user);
                }
            }
        }
    }
}
