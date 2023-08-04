using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace UplauncherReplacer
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            bool restart = false;

            // Check if "-restart" argument is passed
            foreach (string arg in args)
            {
                if (arg.Equals("-restart", StringComparison.OrdinalIgnoreCase))
                {
                    restart = true;
                    break;
                }
            }

            string text = @".\\majs\\UpLauncher.exe";
            string text2 = @".\\UpLauncher.exe";
            string root = @".\\majs";
            string checksum = @".\\checksum.sulax";

            if (!restart)
            {
                try
                {
                    Process[] processesByName = Process.GetProcessesByName("UpLauncher");
                    foreach (Process process in processesByName)
                    {
                        process.WaitForExit(10);
                        process.Kill();
                    }
                }
                catch
                {
                    Console.WriteLine("Process not found");
                }

                Thread.Sleep(500);

                try
                {
                    File.Copy(text, text2, true);
                    File.Delete(text);
                    if (Directory.Exists(root))
                    {
                        Directory.Delete(root);
                    }
                    File.Delete(checksum);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not replace uplauncher.exe ! Download it and replace it manually !!");
                    File.WriteAllText("error.txt", ex.ToString());
                    Console.Read();
                }
            }

            // Restart the uplauncher if "-restart" argument is passed
            if (restart)
            {
                // Kill the uplauncher process before restarting it
                Process[] processesByName = Process.GetProcessesByName("UpLauncher");
                foreach (Process process in processesByName)
                {
                    process.Kill();
                }
                Process.Start(text2);
            }
        }
    }
}
