using System.Collections.Generic;

namespace Zarp.Core.Datatypes
{
    public class BasicRuleCollection<T> where T : BasicRule
    {
        public bool IsWhitelist;

        private Dictionary<string, T> Rules;

        public BasicRuleCollection(bool isWhitelist)
        {
            IsWhitelist = isWhitelist;
            Rules = new Dictionary<string, T>();
        }

        public void AddRule(T rule)
        {
            Rules.TryAdd(rule.Id, rule);
        }

        public void AddRules(IEnumerable<T> rules)
        {
            foreach (T rule in rules)
            {
                Rules.TryAdd(rule.Id, rule);
            }
        }

        public void RemoveRule(string id)
        {
            Rules.Remove(id);
        }

        public IEnumerable<T> GetRules()
        {
            return Rules.Values;
        }

        public bool Contains(string id)
        {
            return Rules.ContainsKey(id);
        }

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

    }
}
