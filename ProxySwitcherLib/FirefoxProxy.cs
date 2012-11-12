using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace ProxySwitcher.Lib
{
    public class FirefoxProxy:IBrowserProxy
    {
        private const string PREFS_FILENAME = "prefs.js";
        private const string PROXY_STATUS_KEY = "network.proxy.type";
        private static readonly Regex regex = null;
        private const string PROXY_TYPE_LINE = @"user_pref(""network.proxy.type"", {0});";

        static FirefoxProxy() {
            regex = new Regex(".*" + PROXY_STATUS_KEY +  ".*(\\d).*",RegexOptions.Compiled);
        }

        public ProxyStatus Status
        {
            get
            {
                // We let file not found exception propagate
                string filePath = getFirefoxUserPrefsFilePath();

                bool found = false;
                Match match = null;
                // Reading prefs file and keeping the match, if it exists
                using (StreamReader reader = new StreamReader(filePath))
                {
                    while (!found && !reader.EndOfStream)
                    {
                        Match m = regex.Match(reader.ReadLine());
                        if (m.Success)
                        {
                            match = m;
                            found = true;
                        }
                    }
                }
                // If no lines were matched, then the prefs is not set, it means it's using system wide proxy
                // Therefore the status is unknown 
                if (!found)
                {
                    return ProxyStatus.Unknown;
                }
                string val = match.Groups[1].Value;
                // Values found here: https://developer.mozilla.org/en-US/docs/Mozilla/Preferences/Mozilla_networking_preferences#Proxy
                //0 = direct
                //1 = manual
                //2 = PAC
                //3 = mapped to 0
                //4 = WPAD (Web Proxy Autodiscovery Protocol)
                switch (val)
                {
                    case "0":
                    case "3":
                        return ProxyStatus.Off;
                    case "1":
                        return ProxyStatus.On;
                    case "2":
                    case "4":
                    default:
                        return ProxyStatus.Unknown;

                }
            }
            set
            {
                string firefoxProxyValue = "0";
                switch (value)
                {
                    case ProxyStatus.On:
                        firefoxProxyValue = "1";
                        break;
                    case ProxyStatus.Off:
                        firefoxProxyValue = "0";
                        break;
                    default:
                        throw new NotSupportedException("You can't set proxy value to "  + value);
                }

                string filePath = getFirefoxUserPrefsFilePath();
                // We'll create a temp file in case something wrong happens
                // It will replace the real prefs file at the end
                string tempPath = System.IO.Path.GetTempFileName();
                using (StreamWriter writer = new StreamWriter(tempPath))
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            // We skip the line which sets the proxy
                            // It will be written at the end
                            // That way we are sure it is written even if the line does not exist originately
                            if (!regex.IsMatch(line))
                            {
                                writer.WriteLine(line);
                            }

                        }
                    }
                    writer.WriteLine(String.Format(PROXY_TYPE_LINE, firefoxProxyValue));
                }
                // Actual replacement, same file content except for the proxy, creates a backup also
                System.IO.File.Replace(tempPath, filePath, filePath + ".bak");
            }
        }
        private string getFirefoxUserPrefsFilePath()
        {
            string[] pathParts = {"Mozilla","Firefox","Profiles"};

            string basePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            foreach (string pathPart in pathParts) {
                basePath += Path.DirectorySeparatorChar + pathPart;
            }
            string[] directories = Directory.GetDirectories(basePath, "*default");
            // if no directories are found, then we'll throw an exception
            if (directories.Length > 0)
            {
                string profilePath = "";
                // Most of the time, there will be only 1 "default" directory)
                if (directories.Length == 1)
                {
                    profilePath = directories[0];
                }
                else
                {
                    // If there is more than one directory, we will take the one with the last access time
                    // (will be considered as "active")
                    DateTime newest = DateTime.MinValue;
                    foreach (string profile in directories)
                    {
                        DateTime profileTime = Directory.GetLastAccessTime(profile);
                        if (newest.CompareTo(profileTime) < 0)
                        {
                            profilePath = profile;
                            newest = profileTime;
                        }
                    }
                }
                return Path.Combine(profilePath, PREFS_FILENAME);
            }
            else
            {
                throw new FileNotFoundException("Could not find preferences file");
            }
        }
        public string Name
        {
            get
            {
                return "Firefox";
            }

        }
    }
}
