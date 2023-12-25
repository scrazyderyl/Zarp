using Zarp.Core.Datatypes;

namespace Zarp.Core.Service
{
    public class Zarp
    {
        internal static PresetManager<FocusSessionPreset> FocusSessionPresetManager = new PresetManager<FocusSessionPreset>();
        internal static PresetManager<RulePreset> RulePresetManager = new PresetManager<RulePreset>();
        internal static PresetManager<RewardPreset> RewardPresetManager = new PresetManager<RewardPreset>();
        internal static Blocker Blocker = new Blocker();

        static Zarp()
        {
            Blocker.Enable();
        }

        public static object? DialogReturnValue;
    }
}
