using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zarp.Core
{
    public struct ApplicationInfo : IComparable<ApplicationInfo>
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

        public int CompareTo(ApplicationInfo other)
        {
            return string.Compare(Name, other.Name);
        }

        public override string ToString()
        {
            return Name ?? ExecutablePath;
        }
    }

    public struct WebsiteInfo : IComparable<WebsiteInfo>
    {
        public string Name;
        public string Domain;

        public WebsiteInfo(string name, string domain)
        {
            Name = name;
            Domain = domain;
        }
        public int CompareTo(WebsiteInfo other)
        {
            return string.Compare(Name, other.Name);
        }

        public override string ToString()
        {
            return Domain;
        }
    }
}
