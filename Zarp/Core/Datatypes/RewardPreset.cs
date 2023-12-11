using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Zarp.Core.Datatypes
{
    public class RewardPreset : RulePreset
    {
        public int EarnedTime;
        public TimeUnit EarnedTimeUnit;
        public RewardRequirement RequirementType;
        public FocusSessionPreset? CompletionRequirement;
        public int ActiveTimeRequirement;
        public TimeUnit ActiveTimeUnit;

        public RewardPreset(string name) : base(name, true, true)
        {
            EarnedTime = 30;
            EarnedTimeUnit = TimeUnit.Minutes;
            RequirementType = RewardRequirement.FocusSessionCompletion;
            CompletionRequirement = null;
            ActiveTimeRequirement = 60;
            ActiveTimeUnit = TimeUnit.Minutes;
        }
    }

    public enum RewardRequirement
    {
        FocusSessionCompletion, ActiveTime
    }
}
