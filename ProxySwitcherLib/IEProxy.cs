using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace ProxySwitcherLib
{
    public class IEProxy:IBrowserProxy
    {
        private const string PROXY_ENABLE_KEY = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings\ProxyEnable";
        public ProxyStatus Status
        {
            get
            {
                if ((bool)Registry.CurrentUser.GetValue(IEProxy.PROXY_ENABLE_KEY, false))
                {
                    return ProxyStatus.On;
                } 
                else 
                {
                    return ProxyStatus.Off;
                }
 
            }
            set
            {
                switch (value)
                {
                    case ProxyStatus.On:
                        Registry.CurrentUser.SetValue(IEProxy.PROXY_ENABLE_KEY, true);
                        break;
                    case ProxyStatus.Off:
                        Registry.CurrentUser.SetValue(IEProxy.PROXY_ENABLE_KEY, false);
                        break;
                }
            }
        }

    }
}
