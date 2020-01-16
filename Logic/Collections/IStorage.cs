using Logic.Explanation;
using System.Collections.Generic;

namespace Logic.Collections
{
    public enum Mode
    {
        Stored = 1,
        StoredAndDerived = 3
    }

    public interface IStorage
    {
        void Store(object value, params object[] tuple);
        bool Contains(Mode mode, out object value, params object[] tuple);
        bool Contains(Mode mode, out Justification justification, out object value, params object[] tuple);
        bool Remove(params object[] tuple);

        void AddRule(Rule rule);
        bool ContainsRule(Rule rule);
        bool RemoveRule(Rule rule);

        IStorage Scope(IKnowledgebase kb);
        IStorage Commit();
        IStorage Rollback();

        IEnumerable<Justification> AsEnumerable(Mode mode);
        IEnumerable<Justification> AsEnumerable(Mode mode, object[] data, JustificationSettings settings);
    }
}