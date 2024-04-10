using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;

namespace FolderSynchronization
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 4) 
            {
                Console.WriteLine("Insufficient number of arguments.");
                return;
            }
            if (!int.TryParse(args[3], out int intervalInSeconds)) 
            {
                Console.WriteLine("Invalid interval argument.");
                return;
            }

            string source = args[0];    
            string replica = args[1];    
            string logPath = args[2];    
            bool isSynching = false;

            var timer = new System.Timers.Timer(ConvertSecondsToMilliseconds(intervalInSeconds));
            timer.Elapsed += (sender, e) =>
            {
                if (!isSynching)
                {
                    isSynching = true;
                    Synchronization.SyncFolder(source, replica, logPath);
                    isSynching = false;
                }
            };
            timer.Start();

            Console.WriteLine("\nThe application started. It will now sync the two folders at the specified interval.");
            Console.WriteLine("\nTo exit the application press the Enter key...\n");
            Console.ReadLine();

            timer.Stop();
            timer.Dispose();

            Console.WriteLine("Application ended.");
        }

        private static int ConvertSecondsToMilliseconds (int seconds)
        {
            const int Converter = 1000; 
            return seconds * Converter;
        }
    }
}