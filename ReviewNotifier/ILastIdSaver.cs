using System;
using System.Collections.Generic;
using System.Text;

namespace ReviewNotifier
{
    interface ILastIdSaver
    {
        long GetValueFromFile();
        void SaveValueToFile(long id);
    }
}