using System.IO;
using System.Reflection;
using System.Xml;
using log4net;
using ReviewNotifier.Interfaces;

namespace ReviewNotifier
{
    public class Log4NetConfig : ILog4NetConfig
    {
        private static ILog _log;

        public Log4NetConfig()
        {
            XmlDocument log4NetConfig = new XmlDocument();
            log4NetConfig.Load(File.OpenRead("log4net.config"));
            var repo = LogManager.CreateRepository(Assembly.GetEntryAssembly(),
                typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4NetConfig["log4net"]);
            _log = LogManager.GetLogger(typeof(Program));
        }

        public void LogErrorMessage(string message) => _log.Error(message);

        public void LogInfoMessage(string message) => _log.Info(message);
    }
}