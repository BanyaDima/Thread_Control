using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class FileWhocher
    {
        private string _path;

        public FileWhocher(string path)
        {
            _path = path;
        }

        public void Start()
        {
            Task task = new Task(() =>
            {
                DateTime timeChange = File.GetLastWriteTime(_path);
                while (true)
                {
                    DateTime newTimeChange = File.GetLastWriteTime(_path);
                    if (timeChange != newTimeChange)
                    {
                        timeChange = newTimeChange;
                        Task task1 = new Task(() =>
                        Change?.Invoke(_path, EventArgs.Empty));
                        task1.Start();
                    }
                }
            }
            );
            task.Start();
        }

        public event EventHandler Change;
    }

    class Program
    {
        static object locker1 = new object();
        static object locker2 = new object();
        static string path = @"C:\New folder\1.txt";

        static void Main(string[] args)
        {
            File.Create(path).Dispose();

            FileWhocher whocher = new FileWhocher(path);
            whocher.Change += WhocerChenge;

            whocher.Start();

            while (true)
            {
                Console.WriteLine(" Do you whont change 0 to 1?");
                string answer = Console.ReadLine();

                if (answer == "y")
                {
                    lock (locker1)
                    {
                        File.WriteAllText(path, "1");
                    }
                }
            }
        }

        private static void WhocerChenge(object path, EventArgs e)
        {
            Console.WriteLine("File Changed");
           
            if (File.ReadAllText((string)path) == "1")
            {
                lock (locker2)
                {
                    File.WriteAllText((string)path, "0");
                    Console.WriteLine("Rollback to 0");
                }
                Thread.Sleep(10000);
            }
        }
    }
}
