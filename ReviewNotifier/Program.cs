using ReviewNotifier.Helpers;
using System;
using System.Linq;
using System.Timers;

namespace ReviewNotifier
{
    class Program
    {
        private static LastIdSettings _lastIdSettings;
        private static TeamsNotifier _teams;
        private static TfsDataConnector _tfs;
        private static int _lastId;

        static void Main(string[] args)
        {
            var settings = new Config().GetSettings();
            var loginBuilder = new LoginBuilder(settings);

            _lastIdSettings = new LastIdSettings();
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
            //Timer_Elapsed(null, null);
            Console.ReadKey();
        }

        
        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Getting reviews");
            var reviews = _tfs.GetReviewData(_lastId);

            foreach (var review in reviews)
            {
                _teams.Send(review);
            }

            _lastId = reviews.Any() ? reviews.Max(x => x.Id) : Math.Max(_lastId, 1);
            _lastIdSettings.Save(_lastId);
            Console.WriteLine("_____________________________________________");

        }
    }
}