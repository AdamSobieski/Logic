using Logic.Collections;
using Logic.Expressions;
using Logic.Incremental;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Logic.Knowledge
{
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

        IKnowledgebaseModule Clone();
        IKnowledgebaseModule Clone(string name);
    }

    public delegate void KnowledgebaseTableChangedEventHandler(object sender, ICompoundExpressionDelta delta);

    public interface IKnowledgebaseTable : ICompoundExpressionList, INotifyCollectionChanged
    {
        event KnowledgebaseTableChangedEventHandler OnTableChanged;
    }
}