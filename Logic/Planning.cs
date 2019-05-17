/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using Logic.Agents;
using Logic.Ethics;
using Logic.Expressions;
using Logic.Knowledge;
using Logic.Planning;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;

namespace Logic.Planning
{
    public interface IAction
    {
        //string Module { get; }
        //string Name { get; }

        IReadOnlyList<VariableExpression> Parameters { get; }
        IReadOnlyList<CompoundExpression> Constraints { get; }

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
        // dynamic Metadata { get; }
    }

    public interface IPlanner
    {
        IObservable<IPlan> Plan(IKnowledgebaseModule kb, IState initial, IState goal); // a default set of actions could be all of the actions in the kb module
        IObservable<IPlan> Plan(IKnowledgebaseModule kb, IState initial, IState goal, IEnumerable<IAction> actions);
        // to do: landmarks, preferences, constraints, etc... IObservable<IPlan> Plan(PlannerSettings settings) ?
        // Gerevini, Alfonso, and Derek Long. Plan constraints and preferences in PDDL3. Technical Report 2005-08-07, Department of Electronics for Automation, University of Brescia, Brescia, Italy, 2005.
    }
}