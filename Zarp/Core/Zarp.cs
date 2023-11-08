using System.Collections.ObjectModel;

namespace Zarp.Core
{
    public class Zarp
    {
        public static PresetManager<FocusSessionPreset> FocusSessionPresetManager = new PresetManager<FocusSessionPreset>();
        public static PresetManager<RulePreset> RulePresetManager = new PresetManager<RulePreset>();
        public static Blocker Blocker = new Blocker();

        public static object? DialogReturnValue;
    }
}
