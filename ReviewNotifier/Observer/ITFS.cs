using ReviewNotifier.Models;

namespace ReviewNotifier
{
    interface ITFS
    {
        void AttachObserver(IObserver observer);
        void DetachObserver(IObserver observer);
        void NotifyAll(ReviewInfo message);
    }
}