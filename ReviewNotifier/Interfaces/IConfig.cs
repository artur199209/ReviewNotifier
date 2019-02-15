
using ReviewNotifier.Models;

namespace ReviewNotifier
{
    interface IConfig
    {
        Settings GetSettings();
    }
}
