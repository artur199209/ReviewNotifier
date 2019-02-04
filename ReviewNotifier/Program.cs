using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;

namespace ReviewNotifier
{
    class Program
    {
        private static Timer timer;
        private static string jsonTemplate;

        static void Main(string[] args)
        {
            //TextReader tr = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "NotificationTemplate.json"));
            //jsonTemplate = tr.ReadToEnd();
         
            //timer = new Timer();
            //timer.AutoReset = true;
            //timer.Interval = 60000;
            //timer.Elapsed += Timer_Elapsed;
            //timer.Start();

            //CodeReview codeReview = new CodeReview();
            //codeReview.ExecuteWiqlQuery();

            CodeReview codeReview = new CodeReview();

            Console.ReadKey();
        }

        
        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CodeReview codeReview = new CodeReview();
            var msg = codeReview.ExecuteWiqlQuery();
            TfsServer tfsServer = new TfsServer();
            MsTeams msTeams = new MsTeams(jsonTemplate);
            tfsServer.AttachObserver(msTeams);

            foreach (var item in msg)
            {
                tfsServer.NotifyAll(item);
            }
            //Task.Run(() => tfsServer.NotifyAll(msg));
        }
    }
}