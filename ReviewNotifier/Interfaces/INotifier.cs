using ReviewNotifier.Models;

namespace ReviewNotifier.Interfaces
{
    interface INotifier
    {
        void Send(CodeReview message);
    }
}