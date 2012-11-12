using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace ProxySwitcher.Lib
{
    public class IEProxy:IBrowserProxy
    {
        private const string PROXY_ENABLE_KEY = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings\ProxyEnable";
        private const string INTERNET_SETTINGS_KEY = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings\";
        private const string PROXY_ENABLE_VALUE = "ProxyEnable";
        public ProxyStatus Status
        {
            get
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(INTERNET_SETTINGS_KEY);

                switch((int)key.GetValue(PROXY_ENABLE_VALUE, -1, RegistryValueOptions.None)) {
                    case 0:
                    return ProxyStatus.Off;
                    case 1:
                    return ProxyStatus.On;
                    default:
                        return ProxyStatus.Unknown;
                }
 
            }
            set
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(INTERNET_SETTINGS_KEY, true);
                
                switch (value)
                {
                    case ProxyStatus.On:
                        key.SetValue(IEProxy.PROXY_ENABLE_VALUE, 1,RegistryValueKind.DWord);
                        break;
                    case ProxyStatus.Off:
                        key.SetValue(IEProxy.PROXY_ENABLE_VALUE, 0, RegistryValueKind.DWord);
                        break;
                    default:
                        throw new NotSupportedException("You can't set proxy value to " + value);
                }
            }
        }
        public String Name
        {
            get
            {
                return "Internet Explorer";
            }

        }
    }
}
