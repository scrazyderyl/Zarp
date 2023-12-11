using System.Collections.ObjectModel;
using Zarp.Core.Datatypes;
using Zarp.GUI.Util;

namespace Zarp.Core.Service
{
    public class Zarp
    {
        public static PresetManager<FocusSessionPreset> FocusSessionPresetManager = new PresetManager<FocusSessionPreset>();
        public static PresetManager<RulePreset> RulePresetManager = new PresetManager<RulePreset>();
        public static PresetManager<RewardPreset> RewardPresetManager = new PresetManager<RewardPreset>();
        public static Blocker Blocker = new Blocker();

        public static ApplicationIconCache IconCache = new ApplicationIconCache();

        public static object? DialogReturnValue;
    }
}
