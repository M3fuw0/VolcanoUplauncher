using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace UplauncherReplacer
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            bool restart = args.Any(arg => arg.Equals("-restart", StringComparison.OrdinalIgnoreCase));

            string upLauncherPath = @".\\majs\\UpLauncher.exe";
            string targetPath = @".\\UpLauncher.exe";
            string root = @".\\majs";
            string checksum = @".\\checksum.karashi";
            string errorFilePath = @".\\error.txt";

            try
            {
                if (!restart)
                {
                    KillUpLauncherProcesses();
                    Thread.Sleep(500);

                    if (File.Exists(errorFilePath))
                    {
                        File.Delete(errorFilePath);
                    }

                    File.Copy(upLauncherPath, targetPath, true);
                    File.Delete(upLauncherPath);

                    if (Directory.Exists(root))
                    {
                        Directory.Delete(root, true);
                    }
                    File.Delete(checksum);
                }

                Process.Start(targetPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not replace uplauncher.exe! Download it and replace it manually!!");
                File.WriteAllText(errorFilePath, ex.ToString());
                Console.Read();
            }

            // Restart the uplauncher if "-restart" argument is passed
            if (restart)
            {
                RestartUpLauncher(targetPath);
            }
        }

        private static void KillUpLauncherProcesses()
        {
            Process[] processesByName = Process.GetProcessesByName("UpLauncher");
            foreach (Process process in processesByName)
            {
                if (!process.HasExited)
                {
                    process.Kill();
                    process.WaitForExit();
                }
                //Process.Start(text2);
            }
        }

        private static void RestartUpLauncher(string targetPath)
        {
            KillUpLauncherProcesses();
            Process.Start(targetPath);
        }
    }
}
