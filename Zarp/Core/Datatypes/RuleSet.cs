using System;
using System.Collections.Generic;
using System.Linq;

namespace Zarp.Core.Datatypes
{
    internal class RuleSet : Preset, IDependency
    {
        internal HashSet<ApplicationInfo> _ApplicationRules;
        internal bool _IsApplicationWhitelist;

        internal Dictionary<Guid, FocusSession> _Dependents;

        public RuleSet() : this(string.Empty, false) { }

        public RuleSet(string name, bool isApplicationWhistlist) : base(name)
        {
            _IsApplicationWhitelist = isApplicationWhistlist;
            _ApplicationRules = new HashSet<ApplicationInfo>();
            _Dependents = new Dictionary<Guid, FocusSession>();
        }

        public RuleSet(RuleSet other) : this(other, string.Empty) { }

        public RuleSet(RuleSet other, string name) : base(name)
        {
            _IsApplicationWhitelist |= other._IsApplicationWhitelist;
            _ApplicationRules = new HashSet<ApplicationInfo>();
            _Dependents = new Dictionary<Guid, FocusSession>();
        }

        public bool IsApplicationBlocked(ApplicationInfo info) => _ApplicationRules.Contains(info) ^ _IsApplicationWhitelist;

        public override bool IsModifiable => _Dependents.Count == 0;

        public override bool IsDeletable => _Dependents.Count == 0;

        public override string Type => "Rule Set";

        public override IEnumerable<IDependency>? Dependencies => null;

        public override IEnumerable<IDependency>? Dependents => _Dependents.Cast<IDependency>();

        public override Preset Duplicate(string name) => new RuleSet(this, name);
    }
}
