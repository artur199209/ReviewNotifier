using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using ReviewNotifier.Helpers;

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
            CodeReview codeReview = new CodeReview();
            var msg = codeReview.ExecuteWiqlQuery();
            TfsServer tfsServer = new TfsServer();
            MsTeams msTeams = new MsTeams();
            tfsServer.AttachObserver(msTeams);

            foreach (var item in msg)
            {
                tfsServer.NotifyAll(item);
            }
            //Task.Run(() => tfsServer.NotifyAll(msg));
        }
    }
}