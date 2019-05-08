using Logic.Prolog.Expressions;
using Logic.Prolog.Incremental;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Logic.Prolog.Knowledge
{
    public interface IKnowledgebase
    {
        IKnowledgebaseModule CreateModule();
        IKnowledgebaseModule CreateModule(string name);
    }

    public interface IKnowledgebaseModule
    {
        string Name { get; }

        IKnowledgebaseTable Table(PredicateExpression predicate);

        bool IsReadOnly { get; }

        bool Contains(CompoundExpression expression);

        bool Add(CompoundExpression expression);
        bool Remove(CompoundExpression expression);
        bool Replace(CompoundExpression remove, CompoundExpression add);

        bool Add(IEnumerable<CompoundExpression> expressions);
        bool Remove(IEnumerable<CompoundExpression> expressions);
        bool Replace(IEnumerable<CompoundExpression> removals, IEnumerable<CompoundExpression> additions);

        bool Replace(ICompoundExpressionDelta delta);

        bool AddRule(CompoundExpression rule);
        bool RemoveRule(CompoundExpression rule);
        bool ContainsRule(CompoundExpression rule);

        IKnowledgebaseModule Clone();
        IKnowledgebaseModule Clone(string name);
    }

    public delegate void KnowledgebaseTableChangedEventHandler(object sender, ICompoundExpressionDelta delta);

    public interface IKnowledgebaseTable : IEnumerable<CompoundExpression>, INotifyCollectionChanged
    {
        int Count { get; }

        CompoundExpression this[int index] { get; }

        event KnowledgebaseTableChangedEventHandler OnTableChanged;
    }
}