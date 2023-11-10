using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zarp.Core
{
    public interface BasicRule
    {
        public string Id { get; }
    }

    public struct ApplicationInfo : BasicRule
    {
        public string Id { get { return ExecutablePath; } }
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
    public class ApplicationNameAscending : IComparer<ApplicationInfo>
    {
        public int Compare(ApplicationInfo x, ApplicationInfo y)
        {
            return string.Compare(x.Name, y.Name);
        }
    }

    public struct WebsiteInfo : BasicRule
    {
        public string Id { get { return Domain; } }
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
}
