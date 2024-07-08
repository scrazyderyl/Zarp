using Zarp.Core.Datatypes;

namespace Zarp.Core.App
{
    internal static class PresetManager
    {
        internal static PresetCollection<FocusSession> FocusSessions;
        internal static PresetCollection<RuleSet> RuleSets;
        internal static PresetCollection<Reward> Rewards;

        static PresetManager()
        {
            FocusSessions = new PresetCollection<FocusSession>();
            RuleSets = new PresetCollection<RuleSet>();
            Rewards = new PresetCollection<Reward>();
        }
    }
}