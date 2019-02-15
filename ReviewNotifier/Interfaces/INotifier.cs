using ReviewNotifier.Models;

namespace ReviewNotifier
{
    interface INotifier
    {
        void Send(CodeReview message);
    }
}