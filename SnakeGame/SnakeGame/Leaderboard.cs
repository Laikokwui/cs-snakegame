using System;
using System.IO;
using System.Collections.Generic;

namespace SnakeGame
{
    class Leaderboard
    {
        private List<User> userlist;
        private List<string> Sorted;

        public Leaderboard()
        {
            userlist = new List<User>();
            Sorted = new List<string>();
        }

        //add user to the list
        public void AddUser(User user)
        {
            userlist.Add(user);
        }

        //display the ranking record from the text file when user clears the game
        public void DisplayRecord()
        {
            int row = 10;
            var path = "../../../textfile/leaderboard.txt";
            Console.SetCursorPosition(Console.WindowWidth / 2 - 10, 3);
            Console.WriteLine("Timer" + '\t' + '\t' + "Username" + " Score");
            using (StreamReader file = new StreamReader(path))
            {
                string ln;
                while ((ln = file.ReadLine()) != null)
                {
                    if (row == 1)
                    {
                        break;
                    }
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 2 - row);
                    Console.WriteLine(ln);
                    row--;
                }
            }
        }

        //add the user record into the text file
        public void StoreRecord()
        {
            var path = "../../../textfile/leaderboard.txt";
            StreamWriter sw = File.AppendText(path);
            foreach (User user in userlist)
            {
                sw.WriteLine(user.Time.ToString() + '\t' + user.Name + '\t' + user.Score.ToString());
            }
            sw.Close();
        }

        //sort
        public void sortLeaderBoard()
        {
            var path = "../../../textfile/leaderboard.txt";
            using (StreamReader file = new StreamReader(path))
            {
                string ln;
                while ((ln = file.ReadLine()) != null)
                {
                    Sorted.Add(ln);
                }
            }
            if (Sorted.Count > 0)
            {
                Sorted.Sort();
                System.IO.File.WriteAllText(path, string.Empty);
                StreamWriter sw = File.AppendText(path);
                foreach (string user in Sorted)
                {
                    sw.WriteLine(user);
                }
                sw.Close();
            }
        }

        public List<User> getUsers
        {
            get { return userlist; }
        }
    }
}
