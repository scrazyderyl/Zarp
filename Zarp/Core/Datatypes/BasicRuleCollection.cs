using System.Collections.Generic;

namespace Zarp.Core.Datatypes
{
    public class BasicRuleCollection<T> where T : BasicRule
    {
        public bool IsWhitelist;
        public IEnumerable<T> Rules => _Rules.Values;

        private Dictionary<string, T> _Rules;

        public BasicRuleCollection(bool isWhitelist)
        {
            IsWhitelist = isWhitelist;
            _Rules = new Dictionary<string, T>();
        }

        public BasicRuleCollection(BasicRuleCollection<T> basicRuleCollection)
        {
            IsWhitelist = basicRuleCollection.IsWhitelist;
            _Rules = new Dictionary<string, T>(basicRuleCollection._Rules);
        }

        public void AddRule(T rule)
        {
            _Rules.TryAdd(rule.Id, rule);
        }

        public void AddRules(IEnumerable<T> rules)
        {
            foreach (T rule in rules)
            {
                _Rules.TryAdd(rule.Id, rule);
            }
        }

        public void RemoveRule(string id)
        {
            _Rules.Remove(id);
        }

        public bool Contains(string id)
        {
            return _Rules.ContainsKey(id);
        }

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
    }
}
