using Logic.Explanation;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Logic.Collections
{
    public enum Mode
    {
        StoredAdditions = 1,
        StoredRemovals = 2,
        StoredAndDerivedAdditions = 5
    }

    public interface IStorage : INotifyCollectionChanged, INotifyPropertyChanged
    {
        bool IsScope { get; }
        int Count { get; }

        bool Store(object value, params object[] tuple);
        bool Contains(Mode mode, out object value, params object[] tuple);
        bool Contains(Mode mode, out Justification justification, out object value, params object[] tuple);
        bool Remove(params object[] tuple);

        void AddRule(Rule rule);
        bool ContainsRule(Rule rule);
        IEnumerable<Rule> GetRules();
        bool RemoveRule(Rule rule);

        IStorage Scope(IKnowledgebase kb);
        IStorage Commit();
        IStorage Rollback();

        IEnumerable<Justification> Match(Mode mode, object[] pattern, RuleSettings settings);
        int CountMatch(Mode mode, object[] pattern);
    }
}