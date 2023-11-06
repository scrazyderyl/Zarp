using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zarp.View;

namespace Zarp.Core
{
    public class Zarp
    {
        static RuleManager ruleManager;
        static WindowWatcher windowWatcher;

        internal Zarp()
        {
            ruleManager = new RuleManager();
            windowWatcher = new WindowWatcher();
        }
    }
}
