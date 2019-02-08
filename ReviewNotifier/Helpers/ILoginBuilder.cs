using System;
using System.Collections.Generic;
using System.Text;

namespace ReviewNotifier
{
    interface ILoginBuilder
    {
        StringBuilder GetCreateByQuery();
    }
}