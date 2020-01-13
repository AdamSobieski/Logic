using System;
using System.Linq;
using System.Linq.Expressions;

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
        void AddRule(string functor, Expression rule);
        bool RemoveRule(string functor, Expression rule);

        IQueryable<IRow> GetAssertions(string functor);
        IQueryable<Expression> GetRules(string functor);

        IKnowledgebase Scope();
        IDisposable Edit();
    }
}