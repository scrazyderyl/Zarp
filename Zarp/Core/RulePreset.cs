using System.Collections;
using System.Collections.Generic;

namespace Zarp.Core
{
    public class RulePreset : Preset
    {
        public BasicRuleCollection<ApplicationInfo> ApplicationRules;
        public BasicRuleCollection<WebsiteInfo> WebsiteRules;

        public RulePreset(string name) : this(name, true, true)
        {

        }

        public RulePreset(string name, bool isApplicationWhistlist, bool isWebsiteWhitelist)
        {
            Name = name;
            ApplicationRules = new BasicRuleCollection<ApplicationInfo>(isApplicationWhistlist);
            WebsiteRules = new BasicRuleCollection<WebsiteInfo>(isWebsiteWhitelist);
        }
    }
}
