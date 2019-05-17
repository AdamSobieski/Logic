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

namespace Logic.Agents
{
    // https://en.wikipedia.org/wiki/Belief%E2%80%93desire%E2%80%93intention_model
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

        // should these be parameters to plan generation and/or to plan evaluation and comparison?
        IReadOnlyCollection<CompoundExpression> Principles { get; }
    }
}
