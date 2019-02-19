using ReviewNotifier.Models;

namespace ReviewNotifier.Interfaces
{
    interface IConfig
    {
        Settings GetSettings();
    }
}
