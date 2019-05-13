﻿/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using Logic.Ethics;
using Logic.Expressions;
using Logic.Knowledge;
using Logic.Planning.Agents;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;

namespace Logic.Planning
{
    public interface IAction
    {
        string Module { get; }
        string Name { get; }

        IReadOnlyList<VariableExpression> Parameters { get; }
        IReadOnlyList<CompoundExpression> Preconditions { get; }
        IDelta<CompoundExpression> Effects { get; }

        // to do: multi-criteria?
        // IComparable GetCost(IState state);

        // DateTimeOffset? Duration { get; }

        // dynamic Metadata { get; }
    }

    public interface IInvocableAction
    {
        bool Invoke(IAgent agent, IState currentState, object[] args);
        Task<bool> InvokeAsync(IAgent agent, IState currentState, object[] args, CancellationToken token);
    }

    public interface IState
    {
        // dynamic Metadata { get; }
    }

    public interface IPlan
    {

    }

    public interface IPlanner : IHasExpression
    {
        IObservable<IPlan> Plan(IKnowledgebaseModule kb, IState initial, IState goal); // a default set of actions could be all of the actions in the kb module
        IObservable<IPlan> Plan(IKnowledgebaseModule kb, IState initial, IState goal, IEnumerable<IAction> actions);
        // to do: landmarks, preferences, etc... IObservable<IPlan> Plan(PlannerSettings settings) ?
    }
}

namespace Logic.Planning.Agents
{
    // https://en.wikipedia.org/wiki/Belief%E2%80%93desire%E2%80%93intention_software_model
    // https://plato.stanford.edu/entries/belief/
    // https://plato.stanford.edu/entries/desire/
    // https://plato.stanford.edu/entries/intention/
    // https://en.wikipedia.org/wiki/Intelligent_agent
    public interface IAgent : IHasExpression
    {
        IPlanner Planner { get; }

        IKnowledgebaseModule Beliefs { get; }
        IReadOnlyCollection<CompoundExpression> Desires { get; }    // instead of collections, might these be stacks or trees with task/subtask or goal/subgoal structures, perhaps weighted sets of stacks or trees?
        IReadOnlyCollection<CompoundExpression> Intentions { get; } //
    }
}