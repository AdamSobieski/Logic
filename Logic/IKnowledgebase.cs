using System;
using System.Linq.Expressions;

namespace Logic
{
    public interface IKnowledgebase
    {
        void AddFunctor(string functor, Type valueType, params Type[] types);
        void RemoveFunctor(string functor);

		bool Contains(string functor, out object value, params object[] tuple);
        void Store(string functor, object value, params object[] tuple);
		void Remove(string functor, params object[] tuple);

		void AddRule(Expression rule);
        void RemoveRule(Expression rule);
    }
}