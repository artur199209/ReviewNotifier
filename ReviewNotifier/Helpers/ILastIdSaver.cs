namespace ReviewNotifier
{
    interface ILastIdSaver
    {
        long GetValueFromFile();
        void SaveValueToFile(int id);
    }
}