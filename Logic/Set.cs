using Logic.Prolog.Expressions;
using System.Collections.Generic;

namespace Logic.Collections
{
    public class Set
    {
        public IReadOnlyList<CompoundExpression> Definition { get; }
        public VariableExpression Parameter { get; }
    }
}
