using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Zarp.Core
{
    class RuleManager
    {
        private HashSet<string> AlwaysAllowedApplications;
        private HashSet<string> AlwaysBlockedApplications;
        private HashSet<string> AlwaysAllowedWebsites;
        private HashSet<string> AlwaysBlockedWebsites;

        private HashSet<string> CurrentApplications;
        private HashSet<string> CurrentWebsites;

        private Event? activeEvent;

        internal RuleManager()
        {
            AlwaysAllowedApplications = new HashSet<string>();
            AlwaysBlockedApplications = new HashSet<string>();
            AlwaysAllowedWebsites = new HashSet<string>();
            AlwaysBlockedWebsites = new HashSet<string>();
            CurrentApplications = new HashSet<string>();
            CurrentWebsites = new HashSet<string>();
            activeEvent = null;
        }

        public void Clear()
        {
            CurrentApplications = new HashSet<string>();
            CurrentWebsites = new HashSet<string>();
            activeEvent = null;
        }

        public void SetActiveEvent(Event newEvent)
        {
            Clear();

            foreach (ApplicationInfo application in newEvent.Applications)
            {
                CurrentApplications.Add(application.ExecutablePath);
            }

            foreach (WebsiteInfo website in newEvent.Websites)
            {
                CurrentApplications.Add(website.Domain);
            }

            activeEvent = newEvent;
        }

        public bool IsAppAllowed(string executablePath)
        {
            if (activeEvent == null)
            {
                return !AlwaysBlockedApplications.Contains(executablePath);
            }

            switch (activeEvent.Value.Type)
            {
                case EventType.Regular:
                    if (activeEvent.Value.Whitelist)
                    {
                        // On whitelist
                        return CurrentApplications.Contains(executablePath) || AlwaysAllowedApplications.Contains(executablePath);
                    }

                    // Not on blacklist
                    return !(CurrentApplications.Contains(executablePath) || AlwaysBlockedApplications.Contains(executablePath));
                case EventType.OfflineBreak:
                    return false;
            }

            return false;
        }

        public void IsWebsiteAllowed(string domain)
        {

        }
    }

    public struct ApplicationInfo
    {
        public string Name;
        public string ExecutablePath;
        public string? IconPath;
        public int IconIndex;

        public ApplicationInfo(string name, string executableName, string? iconPath = null, int iconIndex = 0)
        {
            Name = name;
            ExecutablePath = executableName;
            IconPath = iconPath;
            IconIndex = iconIndex;
        }
    }

    struct WebsiteInfo
    {
        public string Domain;

        public WebsiteInfo(string domain)
        {
            Domain = domain;
        }
    }

    enum EventType
    {
        Regular, OfflineBreak
    }

    struct Event
    {
        public string Title;
        public int Duration;
        public EventType Type;
        public bool Whitelist;
        public ApplicationInfo[] Applications;
        public WebsiteInfo[] Websites;

        public Event(string title, int duration, EventType type, bool whitelist, ApplicationInfo[] applications, WebsiteInfo[] websites)
        {
            Title = title;
            Duration = duration;
            Type = type;
            Whitelist = whitelist;
            Applications = applications;
            Websites = websites;
        }
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
