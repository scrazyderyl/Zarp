using System.Collections.ObjectModel;

namespace Zarp.Core
{
    public class Zarp
    {
        public static PresetManager PresetManager = new PresetManager();
        public static Blocker Blocker = new Blocker();

        public static object DialogReturnValue;
        public static ObservableCollection<ApplicationInfo>? CurrentRuleset;
    }
}
