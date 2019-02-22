using System;
using System.IO;

namespace ReviewNotifier.Helpers
{
    public static class Logger
    {
        private static readonly string Path = "Logs/logs.txt".FullFileLocation();

        public static void Write(string message)
        {
            try
            {
                Directory.CreateDirectory("Logs/".FullFileLocation());
                File.AppendAllText(Path, message +  Environment.NewLine);
                Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                // ignored
            }
        }
    }
}