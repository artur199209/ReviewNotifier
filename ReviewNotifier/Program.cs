using System;
using System.IO;
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
            timer = new Timer();
            timer.AutoReset = true;
            timer.Interval = 60000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            Console.ReadKey();
        }

        
        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            LastIdSaver lastIdSaver = new LastIdSaver();
            LoginBuilder loginBuilder = new LoginBuilder();
            CodeReview codeReview = new CodeReview(lastIdSaver, loginBuilder);
            var msg = codeReview.ExecuteWiqlQuery();
            TfsServer tfsServer = new TfsServer();
            MsTeams msTeams = new MsTeams(lastIdSaver);
            tfsServer.AttachObserver(msTeams);

            foreach (var item in msg)
            {
                tfsServer.NotifyAll(item);
            }
            //Task.Run(() => tfsServer.NotifyAll(msg));
        }
    }
}