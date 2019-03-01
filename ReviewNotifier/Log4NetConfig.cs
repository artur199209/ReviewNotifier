using System.Reflection;
using log4net;

namespace ReviewNotifier
{
    public class Log4NetConfig
    { 
        public static ILog GetLogger()
        {
            return LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }
    }
}