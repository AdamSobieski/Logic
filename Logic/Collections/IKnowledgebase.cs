using Logic.Explanation;
using System;
using System.Collections.Generic;

namespace Logic.Collections
{
    public interface IKnowledgebase
    {
        void AddFunctor(string functor, Type valueType, params Type[] tupleTypes);
        bool ContainsFunctor(string functor);
        bool RemoveFunctor(string functor);
        void Store(string functor, object value, params object[] tuple);
        bool Contains(string functor, out object value, params object[] tuple);
        bool Contains(string functor, out Justification justification, out object value, params object[] tuple);
        bool Remove(string functor, params object[] tuple);
        void AddRule(string functor, Rule rule);
        bool ContainsRule(string functor, Rule rule);
        bool RemoveRule(string functor, Rule rule);

        IEnumerable<Justification> GetContents(string functor);
        IEnumerable<Justification> GetContents(string functor, object[] data, JustificationSettings mode);

        IKnowledgebase Scope();
        IKnowledgebase Commit();
        IKnowledgebase Rollback();

        IDisposable Edit();
    }
}