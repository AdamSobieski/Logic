using Logic.Explanation;
using System.Collections.Generic;

namespace Logic.Collections
{
    public enum Mode
    {
        StoredAdditions = 1,
        StoredRemovals = 2,
        StoredAndDerivedAdditions = 5
    }

    public interface IStorage
    {
        bool IsScope { get; }

        void Store(object value, params object[] tuple);
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

        IEnumerable<Justification> AsEnumerable(Mode mode, object[] data, RuleSettings settings);
    }
}