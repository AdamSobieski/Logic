using Logic.Explanation;
using System.Collections.Generic;

namespace Logic.Collections
{
    internal interface IStorage : IEnumerable<object[]>
    {
        bool Contains(out object value, params object[] tuple);
        bool Contains(out Justification justification, out object value, params object[] tuple);
        void Contains(out IEnumerable<object> value, params object[] tuple);
        void Store(object value, params object[] tuple);
        bool Remove(params object[] tuple);
        void Clear();

        void AddRule(Rule rule);
        bool ContainsRule(Rule rule);
        bool RemoveRule(Rule rule);

        IStorage Scope(IKnowledgebase kb);

        IEnumerable<object> AsEnumerable(object[] data);
    }
}