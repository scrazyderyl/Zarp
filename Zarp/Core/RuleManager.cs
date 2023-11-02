using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Zarp.Core
{
    class RuleManager
    {
        private static String[] AlwaysIgnored = new string[] { "", "Settings", "Program Manager", "Microsoft Text Input Application" };
        private HashSet<String> IgnoredApps = new HashSet<string>(AlwaysIgnored);
        private HashSet<String> AlwaysAllowedApps = new HashSet<string>();
        private HashSet<String> AlwaysBlockedApps = new HashSet<string>();

        public bool IsAllowed(String title)
        {
            return false;
        }

        public void Save()
        {
            
        }
    }

    public struct ApplicationInfo
    {
        public string Name;
        public string? IconPath;
        public int? IconIndex;
        public string? ExecutablePath;

        public ApplicationInfo(string name, string? iconPath = null, int iconIndex = 0, string? executablePath = null)
        {
            Name = name;
            IconPath = iconPath;
            IconIndex = iconIndex;
            ExecutablePath = executablePath;
        }
    }

    interface Rule
    {

    }

    enum RulePolicy
    {
        Allowed,
        Blocked
    }

    struct ApplicationRule : Rule
    {
        public ApplicationInfo App;
        public RulePolicy Policy;

        public ApplicationRule(ApplicationInfo app, RulePolicy policy)
        {
            App = app;
            Policy = policy;
        }
    }

    struct WebsiteRule : Rule
    {
        public String Domain;
        public RulePolicy Policy;
        public bool Partial;

        public WebsiteRule(string domain, RulePolicy policy, bool partial)
        {
            Domain = domain;
            Policy = policy;
            Partial = partial;
        }
    }

    struct BreakRule : Rule
    {
        public uint Duration;
        public bool IsOverridable;
        public bool Offline;

        public BreakRule(uint duration, bool isOverridable, bool offline)
        {
            Duration = duration;
            IsOverridable = isOverridable;
            Offline = offline;
        }
    }

    struct Reward
    {
        public ApplicationInfo app;
        public int duration;
    }
    
    struct Event
    {
        public int Id;
        public String Title;
        public Rule[] Rules;
        public bool IsCancellable;

        public Event(int id, string title, Rule[] rules, bool isCancellable)
        {
            Id = id;
            Title = title;
            Rules = rules;
            IsCancellable = isCancellable;
        }
    }

    struct FocusSessionPreset
    {
        public String Title;
        public bool Repeating;
        public Rule[] Rules;
        public Event[] Events;

        public FocusSessionPreset(string title, bool repeating, Rule[] rules, Event[] events)
        {
            Title = title;
            Repeating = repeating;
            Rules = rules;
            Events = events;
        }
    }
}
