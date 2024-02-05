namespace Zarp.Core.Datatypes
{
    public struct ApplicationInfo : IBasicRule
    {
        public string Id => ExecutablePath;
        public string ExecutablePath;
        public string Name;

        public ApplicationInfo(string executablePath, string name)
        {
            ExecutablePath = executablePath;
            Name = name;
        }

        public override string ToString()
        {
            return Name ?? ExecutablePath;
        }
    }
}
