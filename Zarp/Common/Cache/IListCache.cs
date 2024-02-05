using System.Collections.Generic;

namespace Zarp.Common.Cache
{
    internal interface IListCache<T> : IEnumerable<T>
    {
        public int Count { get; }
        public bool Updated { get; }
    }
}
