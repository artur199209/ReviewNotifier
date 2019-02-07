using System;
using System.Timers;

namespace ReviewNotifier
{
    class Program
    {
        static void Main(string[] args)
        {
            Timer timer = new Timer();
            timer.AutoReset = true;
            timer.Interval = 2000;
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
        }
    }
}