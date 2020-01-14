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
        bool Remove(string functor, params object[] tuple);
        void AddRule(string functor, Rule rule);
        bool ContainsRule(string functor, Rule rule);
        bool RemoveRule(string functor, Rule rule);

        IEnumerable<object[]> GetContents(string functor);
        IEnumerable<object> GetContents(string functor, object[] data);
        IEnumerable<Rule> GetRules(string functor);

        IKnowledgebase Scope();
        IDisposable Edit();
    }
}