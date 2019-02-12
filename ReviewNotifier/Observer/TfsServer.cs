using System.Collections.Generic;
using ReviewNotifier.Models;

namespace ReviewNotifier.Observer
{
    class TfsServer : ITFS
    {
        private readonly List<IObserver> _observers = new List<IObserver>();

        public void AttachObserver(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void DetachObserver(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void NotifyAll(ReviewInfo message)
        {
            foreach (var obs in _observers)
            {
                obs.Update(message);
            }
        }
    }
}