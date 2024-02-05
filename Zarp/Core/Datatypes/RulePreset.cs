namespace Zarp.Core.Datatypes
{
    internal class RulePreset : IPreset
    {
        public string Name { get; set; }
        public BasicRuleCollection<ApplicationInfo> ApplicationRules;

        public RulePreset() : this(string.Empty, false) { }

        public RulePreset(string name) : this(name, false) { }

        public RulePreset(string name, bool isApplicationWhistlist)
        {
            Name = name;
            ApplicationRules = new BasicRuleCollection<ApplicationInfo>(isApplicationWhistlist);
        }

        public RulePreset(RulePreset preset) : this(string.Empty, preset) { }

        public RulePreset(string name, RulePreset preset)
        {
            Name = name;
            ApplicationRules = new BasicRuleCollection<ApplicationInfo>(preset.ApplicationRules);
        }

        public IPreset Duplicate(string name) => new RulePreset(name, this);
    }
}
