namespace Zarp.Core.Datatypes
{
    public class RulePreset : Preset
    {
        public BasicRuleCollection<ApplicationInfo> ApplicationRules;
        public BasicRuleCollection<WebsiteInfo> WebsiteRules;

        public RulePreset() : this(string.Empty, true, true) { }

        public RulePreset(string name) : this(name, true, true) { }

        public RulePreset(string name, bool isApplicationWhistlist, bool isWebsiteWhitelist) : base(name)
        {
            ApplicationRules = new BasicRuleCollection<ApplicationInfo>(isApplicationWhistlist);
            WebsiteRules = new BasicRuleCollection<WebsiteInfo>(isWebsiteWhitelist);
        }

        public RulePreset(RulePreset preset) : this(string.Empty, preset) { }

        public RulePreset(string name, RulePreset preset) : base(name)
        {
            ApplicationRules = new BasicRuleCollection<ApplicationInfo>(preset.ApplicationRules);
            WebsiteRules = new BasicRuleCollection<WebsiteInfo>(preset.WebsiteRules);
        }

        public override Preset Duplicate(string name) => new RulePreset(name, this);
    }
}
