using Logic.Explanation;
using Logic.Reflection;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Logic.Collections
{
    public interface IKnowledgebase : INotifyCollectionChanged, INotifyPropertyChanged
    {
        bool IsScope { get; }
        IKnowledgebase Parent { get; }

        void AddFunctor(FunctorInfo functor);
        void AddFunctor(FunctorInfo functor, IStorage custom);
        bool ContainsFunctor(FunctorInfo functor);
        IEnumerable<FunctorInfo> GetFunctors();
        bool RemoveFunctor(FunctorInfo functor);

        bool Store(FunctorInfo functor, object value, params object[] tuple);
        bool Contains(FunctorInfo functor, out object value, params object[] tuple);
        bool Contains(FunctorInfo functor, out Justification justification, out object value, params object[] tuple);
        bool Remove(FunctorInfo functor, params object[] tuple);

        int CountOf(FunctorInfo functor);
        int Count { get; }

        void AddRule(FunctorInfo functor, Rule rule);
        bool ContainsRule(FunctorInfo functor, Rule rule);
        IEnumerable<Rule> GetRules(FunctorInfo functor);
        bool RemoveRule(FunctorInfo functor, Rule rule);

        void AddConstraint(Constraint constraint);
        bool ContainsConstraint(Constraint constraint);
        IEnumerable<Constraint> GetConstraints();
        bool RemoveConstraint(Constraint constraint);

        bool CheckConstraints();

        IEnumerable<Justification> Match(FunctorInfo functor, Mode mode, object[] pattern, RuleSettings settings);
        int CountMatch(FunctorInfo functor, Mode mode, object[] pattern);

        IKnowledgebase Scope();
        IKnowledgebase Commit();
        IKnowledgebase Rollback();
    }
}