using System.Collections.Generic;
using Zarp.Core.App;

namespace Zarp.Core.Datatypes
{
    internal class Reward : Preset
    {
        public int EarnedTime;
        public RewardRequirement RequirementType;
        public FocusSession? CompletionRequirement;
        public int ActiveTimeRequirement;
        public RuleSet Rules;

        public Reward(string name, int earnedTime, RewardRequirement requirementType, FocusSession? completionRequirement, int activeTimeRequirement) : base(name)
        {
            EarnedTime = earnedTime;
            RequirementType = requirementType;
            CompletionRequirement = completionRequirement;
            ActiveTimeRequirement = activeTimeRequirement;
            Rules = new RuleSet();
        }

        public Reward(string name, Reward other) : base(name)
        {
            EarnedTime = other.EarnedTime;
            RequirementType = other.RequirementType;
            CompletionRequirement = other.CompletionRequirement;
            ActiveTimeRequirement = other.ActiveTimeRequirement;
            Rules = new RuleSet(other.Rules);
        }

        public override bool IsModifiable => !Session.IsRewardEnabled(this);

        public override bool IsDeletable => !Session.IsRewardEnabled(this);

        public override string Type => "Reward";

        public override IEnumerable<IDependency>? Dependencies => null;

        public override IEnumerable<IDependency>? Dependents => null;

        public override Preset Duplicate(string name) => new Reward(name, this);
    }

    public enum RewardRequirement
    {
        FocusSessionCompletion, ActiveTime
    }
}
