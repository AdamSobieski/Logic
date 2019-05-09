using Logic.Prolog.Expressions;
using System.Collections.Generic;

namespace Logic.Collections
{
    public class IntensionalSet
    {
        public string Name { get; }
        public IReadOnlyList<CompoundExpression> Definition { get; }
        public VariableExpression Parameter { get; }

        public VariableExpression CreateVariable(string name)
        {
            return Expression.Variable(name, Definition, Parameter);
        }
    }
}
