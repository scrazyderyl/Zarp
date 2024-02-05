using System.Collections;
using System.Collections.Generic;

namespace Zarp.Core.Datatypes
{
    public class BasicRuleCollection<T> : IEnumerable<T> where T : BasicRule
    {
        public bool IsWhitelist;

        private Dictionary<string, T> Rules;

        public BasicRuleCollection()
        {
            Rules = new Dictionary<string, T>();
        }

        public BasicRuleCollection(bool isWhitelist)
        {
            IsWhitelist = isWhitelist;
            Rules = new Dictionary<string, T>();
        }

        public BasicRuleCollection(BasicRuleCollection<T> other)
        {
            IsWhitelist = other.IsWhitelist;
            Rules = new Dictionary<string, T>(other.Rules);
        }

        public void Add(T item) => Rules.TryAdd(item.Id, item);
        public bool Remove(string id) => Rules.Remove(id);
        public bool Contains(string id) => Rules.ContainsKey(id);

        public bool IsBlocked(string id)
        {
            if (IsWhitelist)
            {
                return !Rules.ContainsKey(id);
            }
            else
            {
                return Rules.ContainsKey(id);
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => Rules.Values.GetEnumerator();
        public IEnumerator GetEnumerator() => GetEnumerator();
    }
}
