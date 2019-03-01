using System;
using System.IO;
using log4net;
using ReviewNotifier.Interfaces;

namespace ReviewNotifier.Helpers
{
    public class LastIdSettings : IIdSettings
    {
        private readonly string _path = "JsonSettings/LastId".FullFileLocation();
        private readonly ILog _logger = Log4NetConfig.GetLogger();

        public int Get()
        {
            try
            {
                if (!File.Exists(_path)) return 1;
                string text = File.ReadAllText(_path);
                return int.Parse(text);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
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
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }
    }
}