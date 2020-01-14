using Logic.Explanation;
using System.Collections.Generic;

namespace Logic.Collections
{
    internal enum ContainsMode
    {
        Stored = 1,
        StoredAndDerived = 3
    }

    internal interface IStorage : IEnumerable<object[]>
    {
        bool Contains(ContainsMode mode, out object value, params object[] tuple);
        bool Contains(ContainsMode mode, out Justification justification, out object value, params object[] tuple);
        void Store(object value, params object[] tuple);
        bool Remove(params object[] tuple);

        void AddRule(Rule rule);
        bool ContainsRule(Rule rule);
        bool RemoveRule(Rule rule);

        IStorage Scope(IKnowledgebase kb);

        IEnumerable<object> AsEnumerable(ContainsMode mode1, object[] data, JustificationMode mode2);
    }
}