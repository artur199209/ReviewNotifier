﻿namespace ReviewNotifier
{
    interface IObserver
    {
        void Update(ReviewInfo message);
    }
}