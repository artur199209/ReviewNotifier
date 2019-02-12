using ReviewNotifier.Models;

namespace ReviewNotifier.Observer
{
    interface IObserver
    {
        void Update(ReviewInfo message);
    }
}