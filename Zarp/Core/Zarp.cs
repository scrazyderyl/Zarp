using Microsoft.Win32;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Zarp.Core
{
    class Zarp
    {
        RuleManager ruleManager;
        Scheduler scheduler;

        public Zarp()
        {
            ruleManager = new RuleManager();
            scheduler = new Scheduler();
        }

        public void Save()
        {
            ruleManager.Save();
            scheduler.Save();
        }
    }
}
