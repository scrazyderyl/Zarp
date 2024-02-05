namespace Zarp.Core.Datatypes
{
    public interface BasicRule
    {
        public string Id { get; }
    }

    public struct ApplicationInfo : BasicRule
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

    public struct WebsiteInfo : BasicRule
    {
        public string Id => Domain;
        public string Name;
        public string Domain;

        public WebsiteInfo(string name, string domain)
        {
            Name = name;
            Domain = domain;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
