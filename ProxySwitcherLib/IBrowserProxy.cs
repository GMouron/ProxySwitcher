using System;
using System.Collections.Generic;
using System.Text;

namespace ProxySwitcher.Lib
{
    public interface IBrowserProxy
    {
        ProxyStatus Status
        {
            get;
            set;
        }
        String Name
        {
            get;
        }

    }
}
