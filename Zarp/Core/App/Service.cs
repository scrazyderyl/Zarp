using System;
using Zarp.Core.Datatypes;

namespace Zarp.Core.App
{
    internal class Service
    {
        internal static string UserDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Zarp\";

        internal static PresetCollection<FocusSessionPreset> FocusSessionPresets = new PresetCollection<FocusSessionPreset>();
        internal static PresetCollection<RulePreset> RulePresets = new PresetCollection<RulePreset>();
        internal static PresetCollection<RewardPreset> RewardPresets = new PresetCollection<RewardPreset>();
        internal static Blocker Blocker = new Blocker();

        static Service()
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            Blocker.Enable();
        }

        private static void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {

        }

        public static object? DialogReturnValue;
    }
}
