using Logic.Expressions;
using Logic.Incremental;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Logic.Collections
{
    public interface ICompoundExpressionContainer
    {
        bool Contains(CompoundExpression expression);
    }

    public interface ICompoundExpressionCollection : ICompoundExpressionContainer
    {
        bool IsReadOnly { get; }

        bool Add(CompoundExpression expression);
        bool Remove(CompoundExpression expression);
        bool Replace(CompoundExpression remove, CompoundExpression add);
    }

    public interface ICompoundExpressionList : ICompoundExpressionCollection, IEnumerable<CompoundExpression>
    {
        int Count { get; }

        CompoundExpression this[int index] { get; }
    }

    public interface IKnowledgebase
    {
        IKnowledgebaseModule CreateModule();
        IKnowledgebaseModule CreateModule(string name);
    }

    public interface IKnowledgebaseModule : ICompoundExpressionCollection
    {
        string Name { get; }

        IKnowledgebaseTable Table(PredicateExpression predicate);

        bool AddRule(CompoundExpression rule);
        bool RemoveRule(CompoundExpression rule);
        bool ContainsRule(CompoundExpression rule);

        bool Add(IEnumerable<CompoundExpression> expressions);
        bool Remove(IEnumerable<CompoundExpression> expressions);

        bool Replace(IEnumerable<CompoundExpression> removals, IEnumerable<CompoundExpression> additions);
        bool Replace(ICompoundExpressionDelta delta);
    }

    public delegate void KnowledgebaseTableChangedEventHandler(object sender, ICompoundExpressionDelta delta);

    public interface IKnowledgebaseTable : ICompoundExpressionList, INotifyCollectionChanged
    {
        event KnowledgebaseTableChangedEventHandler OnTableChanged;
    }
}