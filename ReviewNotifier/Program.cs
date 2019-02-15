using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
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
            var lastIdSettings = new LastIdSettings();
            var loginBuilder = new LoginBuilder();

            var lastId = lastIdSettings.Get();
            var codeReview = new CodeReview(loginBuilder, lastId);
            var msg = codeReview.ExecuteWiqlQuery();
            var tfsServer = new TfsServer();
            var msTeams = new MsTeams();
            tfsServer.AttachObserver(msTeams);

            foreach (var item in msg)
            {
                tfsServer.NotifyAll(item);
            }

            lastId = msg.Any() ? msg.Max(x => x.Id) : 1;
            lastIdSettings.Save(lastId);

        }
    }
}