using System;

namespace RadencyTaskETL
{
    public class UserInterface
    {
        public void StartLoop()
        {
            FilesTracker tracker;
            try
            {
                tracker = new FilesTracker();
            }
            catch (NullReferenceException)
            {
                Print("App.config file is not available or empty", ConsoleColor.Red);
                Console.ReadKey();
                return;
            }
            
            Start();

            while (true)
            {
                string input = Console.ReadLine()?.ToLower();
                if (input != null && input.StartsWith("s"))
                    if (tracker.State == State.Working)
                        Stop();
                    else
                        Start();
                else if (tracker.State == State.Stopped)
                {
                    if (string.IsNullOrEmpty(input)) return;
                    else if (input.StartsWith("r")) Reset();
                }
            }

            void Start()
            {
                tracker.Start();
                Print("App is working...\nS - Stop");
            }

            void Stop()
            {
                tracker.Stop();
                Print("App stopped...\nS - Start | R - Reset | Enter - Exit application");
            }
            
            void Reset()
            {
                tracker.Reset();
                Print("App have been reset...");
                Start();
            }
        }

        private void Print(string message, ConsoleColor color = ConsoleColor.Yellow)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}