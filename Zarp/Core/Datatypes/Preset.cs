using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Zarp.Core.Datatypes
{
    public class Preset
    {
        public string Name;

        public override string ToString()
        {
            return Name;
        }
    }
}
