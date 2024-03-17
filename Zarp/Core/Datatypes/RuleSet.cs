using System;
using System.Collections.Generic;
using System.Linq;

namespace Zarp.Core.Datatypes
{
    internal class RuleSet : Preset, IDependency
    {
        internal BasicRuleCollection<ApplicationInfo> ApplicationRules;
        internal Dictionary<Guid, FocusSession> _Dependents;

        public RuleSet() : this(string.Empty, false) { }

        public RuleSet(string name, bool isApplicationWhistlist) : base(name)
        {
            _Name = name;
            ApplicationRules = new BasicRuleCollection<ApplicationInfo>(isApplicationWhistlist);
            _Dependents = new Dictionary<Guid, FocusSession>();
        }

        public RuleSet(RuleSet other) : this(other, string.Empty) { }

        public RuleSet(RuleSet other, string name) : base(name)
        {
            _Name = name;
            ApplicationRules = new BasicRuleCollection<ApplicationInfo>(other.ApplicationRules);
            _Dependents = new Dictionary<Guid, FocusSession>();
        }

        public override bool IsModifiable => _Dependents.Count == 0;

        public override bool IsDeletable => _Dependents.Count == 0;

        public override string Type => "Rule Set";

        public override IEnumerable<IDependency>? Dependencies => null;

        public override IEnumerable<IDependency>? Dependents => _Dependents.Cast<IDependency>();

        public override Preset Duplicate(string name) => new RuleSet(this, name);
    }
}
