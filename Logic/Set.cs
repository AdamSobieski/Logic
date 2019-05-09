using Logic.Prolog.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace Logic.Collections
{
    public class Set
    {
        public Set(IEnumerable<CompoundExpression> definition, VariableExpression parameter)
        {
            this.m_definition = definition.ToList().AsReadOnly();
            this.m_parameter = parameter;
        }

        IReadOnlyList<CompoundExpression> m_definition;
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
    }
}
