using System;
using System.IO;
using System.Reflection;

namespace ReviewNotifier.Helpers
{
    class LastIdSaver : ILastIdSaver
    {
        private string _path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "JsonSettings/LastCodeReviewId.json");
        public long GetValueFromFile()
        {
            try
            {                
                string text = File.ReadAllText(_path);
                return int.Parse(text);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public void SaveValueToFile(int id)
        {
            try
            {
                File.WriteAllText(_path, id.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}