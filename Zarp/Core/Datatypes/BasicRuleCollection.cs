using System.Collections;
using System.Collections.Generic;

namespace Zarp.Core.Datatypes
{
    internal class BasicRuleCollection<T> : IEnumerable<T> where T : IBasicRule
    {
        public bool IsWhitelist;

        private Dictionary<string, T> _Rules;

        public BasicRuleCollection()
        {
            _Rules = new Dictionary<string, T>();
        }

        public BasicRuleCollection(bool isWhitelist)
        {
            IsWhitelist = isWhitelist;
            _Rules = new Dictionary<string, T>();
        }

        public BasicRuleCollection(BasicRuleCollection<T> other)
        {
            IsWhitelist = other.IsWhitelist;
            _Rules = new Dictionary<string, T>(other._Rules);
        }

        public void Add(T item) => _Rules.TryAdd(item.Id, item);
        public bool Remove(string id) => _Rules.Remove(id);
        public bool Contains(string id) => _Rules.ContainsKey(id);

        public bool IsBlocked(string id)
        {
            if (IsWhitelist)
            {
                return !_Rules.ContainsKey(id);
            }
            else
            {
                return _Rules.ContainsKey(id);
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => _Rules.Values.GetEnumerator();
        public IEnumerator GetEnumerator() => GetEnumerator();
    }
}
