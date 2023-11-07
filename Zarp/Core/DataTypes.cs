using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zarp.Core
{
    public struct ApplicationInfo
    {
        public string ExecutablePath;
        public string? Name;
        public string? IconPath;
        public int IconIndex;

        public ApplicationInfo(string executablePath, string? name = null, string? iconPath = null, int iconIndex = 0)
        {
            ExecutablePath = executablePath;
            Name = name;
            IconPath = iconPath;
            IconIndex = iconIndex;
        }

        public override string ToString()
        {
            return Name ?? ExecutablePath;
        }
    }

    public struct WebsiteInfo
    {
        public string Name;
        public string Domain;

        public WebsiteInfo(string name, string domain)
        {
            Name = name;
            Domain = domain;
        }

        public override string ToString()
        {
            return Domain;
        }
    }

    public enum EventType
    {
        Regular, OfflineBreak
    }
}
