using System.Collections.Generic;
using System.Linq;

namespace Zarp.Core
{
    public class PresetManager
    {
        private Dictionary<string, FocusSessionPreset> FocusSessions;
        private Dictionary<string, RulePreset> Rules;

        public PresetManager()
        {
            FocusSessions = new Dictionary<string, FocusSessionPreset>();
            Rules = new Dictionary<string, RulePreset>();
        }

        public IEnumerable<string> GetFocusSessionPresets()
        {
            return FocusSessions.Keys.AsEnumerable();
        }

        public IEnumerable<string> GetRulePresets()
        {
            return Rules.Keys.AsEnumerable();
        }
    }
}
