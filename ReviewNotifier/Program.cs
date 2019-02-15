using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using ReviewNotifier.Config;
using System.Threading.Tasks;
using System.Timers;
using ReviewNotifier.Helpers;
using ReviewNotifier.Observer;

namespace ReviewNotifier
{
    class Program
    {
        private static Timer timer;

        static void Main(string[] args)
        {
            timer = new Timer
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
            var configuration = Configuration.ConfigInstance;
            var webHookUrl = configuration.GetSection("webHookUrl").Value;
            var lastIdSettings = new LastIdSettings();
            var loginBuilder = new LoginBuilder(configuration);
            var teams = new TeamsNotifier(webHookUrl);

            var lastId = lastIdSettings.Get();
            var tfs = new TfsDataConnector(loginBuilder, lastId);
            var reviews = tfs.GetReviewData();

            foreach (var review in reviews)
            {
                teams.Send(review);
            }

            lastId = reviews.Any() ? reviews.Max(x => x.Id) : 1;
            lastIdSettings.Save(lastId);

        }
    }
}