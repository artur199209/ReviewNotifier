using System;
using System.IO;

namespace ReviewNotifier.Helpers
{
    public static class Logger
    {
        private static readonly string Path = "Logs/logs.txt".FullFileLocation();

        public static void SaveLog(string messsage)
        {
            try
            {
                Directory.CreateDirectory("Logs/".FullFileLocation());
                File.AppendAllText(Path, messsage +  Environment.NewLine);
            }
            catch (Exception ex)
            {
                // ignored
            }
        }
    }
}