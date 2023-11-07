using System.Collections.Generic;

namespace Zarp.Core
{
    public class RulePreset
    {
        public string Title;

        private bool IsApplicationWhitelist;
        private bool IsWebsiteWhitelist;
        private Dictionary<string, ApplicationInfo> ApplicationRules;
        private Dictionary<string, WebsiteInfo> WebsiteRules;

        public RulePreset(string title, bool isApplicationWhistlist, bool isWebsiteWhitelist)
        {
            Title = title;
            IsApplicationWhitelist = isApplicationWhistlist;
            IsWebsiteWhitelist = isWebsiteWhitelist;

            ApplicationRules = new Dictionary<string, ApplicationInfo>();
            WebsiteRules = new Dictionary<string, WebsiteInfo>();
        }

        public void AddApplicationRule(ApplicationInfo rule)
        {
            try
            {
                ApplicationRules.Add(rule.ExecutablePath, rule);
            } catch
            {

            }
        }

        public void RemoveApplicationRule(ApplicationInfo rule)
        {
            ApplicationRules.Remove(rule.ExecutablePath);
        }

        public void AddWebsiteRule(WebsiteInfo rule)
        {
            try
            {
                WebsiteRules.Add(rule.Domain, rule);
            } catch
            {

            }
        }

        public void RemoveWebsiteRule(WebsiteInfo rule)
        {
            WebsiteRules.Remove(rule.Domain);
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

        public override string ToString()
        {
            return Title;
        }
    }
}
