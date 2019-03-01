using ReviewNotifier.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Timers;
using System.Xml;
using log4net;

namespace ReviewNotifier
{
    class Program
    {
        private static LastIdSettings _lastIdSettings;
        private static TeamsNotifier _teams;
        private static TfsDataConnector _tfs;
        private static int _lastId;
        private static ILog _logger;

        static void Main(string[] args)
        {
            XmlDocument log4NetConfig = new XmlDocument();
            log4NetConfig.Load(File.OpenRead("log4net.config"));
            var repo = LogManager.CreateRepository(Assembly.GetEntryAssembly(),
                typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4NetConfig["log4net"]);
            _logger = Log4NetConfig.GetLogger();
            
            var settings = new Config().GetSettings();
            var loginBuilder = new LoginBuilder(settings);
            _lastIdSettings = new LastIdSettings();
            _logger.Info("Getting lastID from file...");
            _lastId = _lastIdSettings.Get();
            _teams = new TeamsNotifier(settings.WebHookUrl);
            _tfs = new TfsDataConnector(settings, loginBuilder);

            var timer = new Timer
            {
                AutoReset = true,
                Interval = 60000
            };
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            Console.ReadKey();
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _logger.Info("Getting reviews");
            var reviews = _tfs.GetReviewData(_lastId);

            _logger.Info("Sending info to Teams...");

            foreach (var review in reviews)
            {
                _teams.Send(review);
            }

            _lastId = reviews.Any() ? reviews.Max(x => x.Id) : Math.Max(_lastId, 1);
            _logger.Info("Saving ID...");
            _lastIdSettings.Save(_lastId);
            _logger.Info("_____________________________________________");

        }
    }
}