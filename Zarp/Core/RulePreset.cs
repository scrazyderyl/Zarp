using System.Collections;
using System.Collections.Generic;

namespace Zarp.Core
{
    public class RulePreset : Preset
    {
        public bool IsApplicationWhitelist;
        public bool IsWebsiteWhitelist;

        private Dictionary<string, ApplicationInfo> ApplicationRules;
        private Dictionary<string, WebsiteInfo> WebsiteRules;

        public RulePreset(string name, bool isApplicationWhistlist, bool isWebsiteWhitelist)
        {
            Name = name;
            IsApplicationWhitelist = isApplicationWhistlist;
            IsWebsiteWhitelist = isWebsiteWhitelist;

            ApplicationRules = new Dictionary<string, ApplicationInfo>();
            WebsiteRules = new Dictionary<string, WebsiteInfo>();
        }

        public void AddApplicationRules(List<ApplicationInfo> rules)
        {
            foreach (ApplicationInfo rule in rules)
            {
                try
                {
                    ApplicationRules.Add(rule.ExecutablePath, rule);
                }
                catch { }
            }
        }

        public void RemoveApplicationRule(ApplicationInfo rule)
        {
            ApplicationRules.Remove(rule.ExecutablePath);
        }

        public IEnumerable<ApplicationInfo> GetApplicationRules()
        {
            return ApplicationRules.Values;
        }

        public void AddWebsiteRule(List<WebsiteInfo> rules)
        {
            foreach (WebsiteInfo rule in rules)
            {
                try
                {
                    WebsiteRules.Add(rule.Domain, rule);
                }
                catch { }
            }
        }

        public void RemoveWebsiteRule(WebsiteInfo rule)
        {
            WebsiteRules.Remove(rule.Domain);
        }

        public IEnumerable<WebsiteInfo> GetWebsiteRules()
        {
            return WebsiteRules.Values;
        }

        public bool IsApplicationBlocked(string executablePath)
        {
            if (IsApplicationWhitelist)
            {
                return !ApplicationRules.ContainsKey(executablePath);
            } else
            {
                return ApplicationRules.ContainsKey(executablePath);
            }
        }

        public bool IsWebsiteBlocked(string domain)
        {
            if (IsWebsiteWhitelist)
            {
                return !WebsiteRules.ContainsKey(domain);
            } else
            {
                return WebsiteRules.ContainsKey(domain);
            }
        }
    }
}
