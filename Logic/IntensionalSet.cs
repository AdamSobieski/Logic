using Logic.Prolog.Expressions;
using System.Collections.Generic;

namespace Logic.Collections
{
    public class IntensionalSet
    {
        public IReadOnlyList<CompoundExpression> Definition { get; }
        public VariableExpression Parameter { get; }
    }
}
