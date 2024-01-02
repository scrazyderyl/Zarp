namespace Zarp.Core.Datatypes
{
    public class RewardPreset : Preset
    {
        public int EarnedTime;
        public TimeUnit EarnedTimeUnit;
        public RewardRequirement RequirementType;
        public FocusSessionPreset? CompletionRequirement;
        public int ActiveTimeRequirement;
        public TimeUnit ActiveTimeUnit;
        public RulePreset Rules;

        public RewardPreset(string name) : base(name)
        {
            EarnedTime = 30;
            EarnedTimeUnit = TimeUnit.Minutes;
            RequirementType = RewardRequirement.FocusSessionCompletion;
            CompletionRequirement = null;
            ActiveTimeRequirement = 60;
            ActiveTimeUnit = TimeUnit.Minutes;
            Rules = new RulePreset();
        }

        public RewardPreset(string name, RewardPreset preset) : base(name)
        {
            EarnedTime = preset.EarnedTime;
            EarnedTimeUnit = preset.EarnedTimeUnit;
            RequirementType = preset.RequirementType;
            CompletionRequirement = preset.CompletionRequirement;
            ActiveTimeRequirement = preset.ActiveTimeRequirement;
            ActiveTimeUnit = preset.ActiveTimeUnit;
            Rules = new RulePreset(preset.Rules);
        }

        public override Preset Duplicate(string name) => new RewardPreset(name, this);
    }

    public enum RewardRequirement
    {
        FocusSessionCompletion, ActiveTime
    }
}
