using System.Collections.Generic;

namespace Zarp.Common.Cache
{
    public interface IListCache<T> : IEnumerable<T>
    {
        public int Count { get; }
        public bool Updated { get; }
    }
}
