using ReviewNotifier.Models;

namespace ReviewNotifier.Observer
{
    interface ITFS
    {
        void AttachObserver(IObserver observer);
        void DetachObserver(IObserver observer);
        void NotifyAll(ReviewInfo message);
    }
}