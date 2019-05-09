using Logic.Prolog.Expressions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Logic.Collections
{
    public class Set : Expression
    {
        public Set(IEnumerable<CompoundExpression> definition, VariableExpression parameter)
        {
            this.m_definition = definition.ToList().AsReadOnly();
            this.m_parameter = parameter;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<CompoundExpression> m_definition;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        VariableExpression m_parameter;

        public IReadOnlyList<CompoundExpression> Definition
        {
            get
            {
                return m_definition;
            }
        }
        public VariableExpression Parameter
        {
            get
            {
                return m_parameter;
            }
        }

        internal override Expression Replace(Expression[] from, Expression[] to)
        {
            throw new System.NotImplementedException();
        }
    }
}
