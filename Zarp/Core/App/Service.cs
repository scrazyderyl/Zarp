using Zarp.Core.Datatypes;

namespace Zarp.Core.App
{
    public class Service
    {
        internal static PresetCollection<FocusSessionPreset> FocusSessionPresets = new PresetCollection<FocusSessionPreset>();
        internal static PresetCollection<RulePreset> RulePresets = new PresetCollection<RulePreset>();
        internal static PresetCollection<RewardPreset> RewardPresets = new PresetCollection<RewardPreset>();
        internal static Blocker Blocker = new Blocker();

        static Service()
        {
            Blocker.Enable();
        }

        public static object? DialogReturnValue;
    }
}
