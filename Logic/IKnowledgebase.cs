using System;
using System.Linq.Expressions;

namespace Logic
{
    public interface IKnowledgebase
    {
        void AddFunctor(string functor, Type valueType, params Type[] tupleTypes);
        void RemoveFunctor(string functor);
        void Store(string functor, object value, params object[] tuple);
        bool Contains(string functor, out object value, params object[] tuple);
        void Remove(string functor, params object[] tuple);
        void AddRule(string functor, Expression rule);
        void RemoveRule(string functor, Expression rule);
    }
}