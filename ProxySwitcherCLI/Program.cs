using System;
using System.Collections.Generic;
using System.Text;
using ProxySwitcher.Lib;

namespace ProxySwitcher.CLI
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                printProxyStatus();
            }
            else
            {
                switch(args[0]) {
                    case "--status":
                        printProxyStatus();
                        break;
                    case "--set-status":
                        handleSetProxyStatus(args);
                        break;
                    case "--help":
                    case "-h":
                    case "/?":
                        printHelp();
                        break;
                    default:
                        printHelp();
                        break;
                }
            }
        }

        private static void printProxyStatus()
        {
            foreach (IBrowserProxy proxy in getProxies())
            {
                Console.WriteLine(proxy.Name + " proxy is " + proxy.Status);
            }
        }

        private static void printHelp()
        {
            Console.WriteLine("--status to see proxy status");
            Console.WriteLine("--set-status [on|off] to switch proxy on or off");
        }

        private static void handleSetProxyStatus(string[] args)
        {
            if (args.Length > 1)
            {
                try
                {
                    ProxyStatus status = (ProxyStatus)Enum.Parse(typeof(ProxyStatus), args[1], true);
                    foreach (IBrowserProxy proxy in getProxies())
                    {
                        proxy.Status = status;
                    }
                    printProxyStatus();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    printHelp();
                }
            }
            else
            {
                printHelp();
            }
        }

        private static IBrowserProxy[] getProxies()
        {
            return new IBrowserProxy[]{new IEProxy(), new FirefoxProxy()};
        }
    }

}
