using ReviewNotifier.Models;

namespace ReviewNotifier.Observer
{
    interface INotifier
    {
        void Send(ReviewInfo message);
    }
}