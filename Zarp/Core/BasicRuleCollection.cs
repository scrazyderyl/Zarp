using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zarp.Core
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
            try
            {
                Rules.Add(rule.Id, rule);
            }
            catch { }
        }

        public void AddRules(IEnumerable<T> rules)
        {
            foreach (T rule in rules)
            {
                try
                {
                    Rules.Add(rule.Id, rule);
                }
                catch { }
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
