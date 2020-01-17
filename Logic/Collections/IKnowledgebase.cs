using Logic.Explanation;
using System;
using System.Collections.Generic;

namespace Logic.Collections
{
    public interface IKnowledgebase
    {
        bool IsScope { get; }
        IKnowledgebase Parent { get; }

        void AddFunctor(string functor, Type valueType, params Type[] tupleTypes);
        void AddFunctor(string functor, IStorage custom);
        bool ContainsFunctor(string functor);
        IEnumerable<string> GetFunctors();
        bool RemoveFunctor(string functor);
        int GetLength(string functor);

        void Store(string functor, object value, params object[] tuple);
        bool Contains(string functor, out object value, params object[] tuple);
        bool Contains(string functor, out Justification justification, out object value, params object[] tuple);
        bool Remove(string functor, params object[] tuple);

        void AddRule(string functor, Rule rule);
        bool ContainsRule(string functor, Rule rule);
        IEnumerable<Rule> GetRules(string functor);
        bool RemoveRule(string functor, Rule rule);

        void AddConstraint(Constraint constraint);
        bool ContainsConstraint(Constraint constraint);
        IEnumerable<Constraint> GetConstraints();
        bool RemoveConstraint(Constraint constraint);

        bool CheckConstraints();

        IEnumerable<Justification> Match(string functor, Mode mode, object[] pattern, RuleSettings settings);
        IEnumerable<Justification> AsEnumerable(string functor, Mode mode, object[] data, RuleSettings settings);

        IKnowledgebase Scope();
        IKnowledgebase Commit();
        IKnowledgebase Rollback();
    }
}