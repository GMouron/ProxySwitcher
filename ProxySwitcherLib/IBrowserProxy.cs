using System;
using System.Collections.Generic;
using System.Text;

namespace ProxySwitcherLib
{
    public interface IBrowserProxy
    {
        public ProxyStatus Status
        {
            get;
            set;
        }

    }
}
