using System.Collections.Generic;

namespace Zarp.Core.Datatypes
{
    internal class ApplicationStartInfo : ApplicationInfo
    {
        public string Path;
        public string Arguments;
        public Dictionary<string, List<WindowLocation>>? Layout;

        public ApplicationStartInfo(string path) : this(path, string.Empty) { }

        public ApplicationStartInfo(string path, string arguments) : base(path)
        {
            Path = path;
            Arguments = arguments;
        }

        public ApplicationStartInfo(string name, string executablePath, string arguments) : base(name, executablePath)
        {
            Path = executablePath;
            Arguments = arguments;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            ApplicationStartInfo other = (ApplicationStartInfo)obj;

            return other.FileName == FileName && other.Arguments == Arguments && other.CompanyName == CompanyName;
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}
