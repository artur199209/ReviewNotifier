using System;
using System.IO;
using ReviewNotifier.Interfaces;

namespace ReviewNotifier.Helpers
{
    public class LastIdSettings : IIdSettings
    {
        private readonly string _path = "JsonSettings/LastId".FullFileLocation();
        public int Get()
        {
            try
            {
                if (!File.Exists(_path)) return 1;
                string text = File.ReadAllText(_path);
                return int.Parse(text);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public void Save(int id)
        {
            try
            {
                Directory.CreateDirectory("JsonSettings/".FullFileLocation());
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