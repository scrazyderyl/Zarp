using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Zarp.Core
{
    class EventManager
    {
        private HashSet<string> AlwaysAllowedApplications;
        private HashSet<string> AlwaysBlockedApplications;
        private HashSet<string> AlwaysAllowedWebsites;
        private HashSet<string> AlwaysBlockedWebsites;

        private HashSet<string> AllowedApplications;
        private HashSet<string> BlockedApplications;
        private HashSet<string> AllowedWebsites;
        private HashSet<string> BlockedWebsites;

        private Event? activeEvent;

        public EventManager()
        {
            AlwaysAllowedApplications = new HashSet<string>();
            AlwaysBlockedApplications = new HashSet<string>();
            AlwaysAllowedWebsites = new HashSet<string>();
            AlwaysBlockedWebsites = new HashSet<string>();

            AllowedApplications = new HashSet<string>();
            BlockedApplications = new HashSet<string>();
            AllowedWebsites = new HashSet<string>();
            BlockedWebsites = new HashSet<string>();

            activeEvent = null;
        }

        public void Clear()
        {
            AllowedApplications = new HashSet<string>();
            BlockedApplications = new HashSet<string>();
            AllowedWebsites = new HashSet<string>();
            BlockedWebsites = new HashSet<string>();

            activeEvent = null;
        }

        public void SetActiveEvent(Event newEvent)
        {
            Clear();

            foreach (ApplicationRule rule in newEvent.ApplicationRules) {
                switch (rule.Policy)
                {
                    case RulePolicy.Allow:
                        if (!AlwaysBlockedApplications.Contains(rule.App.ExecutablePath))
                        {
                            AllowedApplications.Add(rule.App.ExecutablePath);
                        }
                        break;
                    case RulePolicy.Block:
                        if (!AlwaysAllowedApplications.Contains(rule.App.ExecutablePath))
                        {
                            BlockedApplications.Add(rule.App.ExecutablePath);
                        }
                        break;
                }
            }

            foreach (WebsiteRule rule in newEvent.WebsiteRules)
            {
                switch (rule.Policy)
                {
                    case RulePolicy.Allow:
                        if (!AlwaysBlockedWebsites.Contains(rule.Domain))
                        {
                            AllowedApplications.Add(rule.Domain);
                        }
                        break;
                    case RulePolicy.Block:
                        if (!AlwaysAllowedWebsites.Contains(rule.Domain))
                        {
                            BlockedApplications.Add(rule.Domain);
                        }
                        break;
                }
            }

            activeEvent = newEvent;
        }

        public bool IsAppAllowed(string executablePath)
        {
            if (activeEvent == null)
            {
                return AlwaysAllowedApplications.Contains(executablePath);
            }

            return false;
        }

        public void IsWebsiteAllowed(string domain)
        {

        }
    }

    public struct ApplicationInfo
    {
        public string? Name;
        public string ExecutablePath;
        public string? IconPath;
        public int IconIndex;

        public ApplicationInfo(string executablePath, string? name = null, string? iconPath = null, int iconIndex = 0)
        {
            Name = name;
            ExecutablePath = executablePath;
            IconPath = iconPath;
            IconIndex = iconIndex;
        }
    }

    enum RulePolicy
    {
        Allow,
        Block
    }

    struct ApplicationRule
    {
        public RulePolicy Policy;
        public ApplicationInfo App;

        public ApplicationRule(ApplicationInfo app, RulePolicy policy)
        {
            App = app;
            Policy = policy;
        }
    }

    struct WebsiteRule
    {
        public RulePolicy Policy;
        public string Domain;

        public WebsiteRule(RulePolicy policy, string domain)
        {
            Policy = policy;
            Domain = domain;
        }
    }

    enum EventPolicy
    {
        Individual, Whitelist, Blacklist, OfflineBreak
    }

    struct Event
    {
        public string Title;
        public int Duration;
        public EventPolicy Policy;
        public ApplicationRule[] ApplicationRules;
        public WebsiteRule[] WebsiteRules;

        public Event(string title, int duration, EventPolicy policy, ApplicationRule[] applicationRules, WebsiteRule[] websiteRules)
        {
            Title = title;
            Duration = duration;
            Policy = policy;
            ApplicationRules = applicationRules;
            WebsiteRules = websiteRules;
        }
    }
    struct Reward
    {
        public Event Event;
        public int duration;
    }

    struct FocusSessionPreset
    {
        public String Title;
        public Event[] Events;
        public int Loops;

        public FocusSessionPreset(string title, Event[] events, int loops)
        {
            Title = title;
            Events = events;
            Loops = loops;
        }
    }
}
