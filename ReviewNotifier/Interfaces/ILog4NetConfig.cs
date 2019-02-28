namespace ReviewNotifier.Interfaces
{
    interface ILog4NetConfig
    {
        void LogErrorMessage(string message);
        void LogInfoMessage(string message);
    }
}