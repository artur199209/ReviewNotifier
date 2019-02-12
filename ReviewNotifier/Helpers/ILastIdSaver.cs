namespace ReviewNotifier
{
    interface ILastIdSaver
    {
        long GetValueFromFile();
        void SaveValueToFile(long id);
    }
}