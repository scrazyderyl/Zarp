using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zarp.Common
{
    public interface ListCache<T> : IEnumerable<T>
    {
        public bool IsUpdated { get; }
        public int Count { get; }

        public void Populate();
        public void Update();
    }
}
