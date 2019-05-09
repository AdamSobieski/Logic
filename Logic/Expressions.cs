/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using Logic.Planning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Logic.Prolog.Expressions
{
    internal struct Atom
    {
        public Atom(string name)
        {
            m_name = name;
        }

        string m_name;

        public string Name
        {
            get
            {
                return m_name;
            }
        }
    }

    public abstract class Expression
    {
        internal static IReadOnlyList<CompoundExpression> m_emptyCompounds = new List<CompoundExpression>(0).AsReadOnly();
        internal static IReadOnlyList<VariableExpression> m_emptyVariables = new List<VariableExpression>(0).AsReadOnly();

        internal static PredicateExpression m_true = Predicate(null, "true", 0);
        internal static PredicateExpression m_false = Predicate(null, "false", 0);

        internal static CompoundExpression m_trueCompound = Compound(m_true);
        internal static CompoundExpression m_falseCompound = Compound(m_false);

        internal static VariableExpression m_trueVariable = new VariableExpression(true);
        internal static VariableExpression m_falseVariable = Variable(new CompoundExpression[] { m_falseCompound }, m_trueVariable);

        internal static SetExpression m_universalSet = Set(m_emptyCompounds, m_trueVariable);
        internal static SetExpression m_emptySet = Set(new CompoundExpression[] { m_falseCompound }, m_trueVariable);



        public static SetExpression UniversalSet
        {
            get
            {
                return m_universalSet;
            }
        }
        public static SetExpression EmptySet
        {
            get
            {
                return m_emptySet;
            }
        }



        public static ConstantExpression Constant(object value)
        {
            return new ConstantExpression(value, value != null ? value.GetType() : typeof(object));
        }
        public static ConstantExpression Constant(object value, Type type)
        {
            Contract.Requires(type != null);
            if (value == null && type.IsValueType && !(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                throw new ArgumentException();
            }
            if (value != null && !type.IsAssignableFrom(value.GetType()))
            {
                throw new ArgumentException();
            }
            return new ConstantExpression(value, type);
        }

        public static ConstantExpression Integer(int value)
        {
            return new ConstantExpression(value, typeof(int));
        }
        public static ConstantExpression Float(double value)
        {
            return new ConstantExpression(value, typeof(double));
        }
        public static ConstantExpression Atom(string name)
        {
            return new ConstantExpression(new Atom(name), typeof(Atom));
        }

        public static VariableExpression Variable()
        {
            return new VariableExpression();
        }
        public static VariableExpression Variable(IEnumerable<CompoundExpression> constraints, VariableExpression parameter)
        {
            Contract.Requires(constraints != null);
            Contract.Requires(parameter != null);

            return new VariableExpression(constraints, parameter);
        }

        public static SetExpression Set(IEnumerable<CompoundExpression> definition, VariableExpression parameter)
        {
            return new SetExpression(definition, parameter);
        }

        public static PredicateExpression Predicate(string module, string name, int arity)
        {
            return new PredicateExpression(module, name, arity);
        }
        public static PredicateExpression Predicate(string module, string name, int arity, IEnumerable<CompoundExpression> preconditions, IEnumerable<VariableExpression> parameters)
        {
            return new PredicateExpression(module, name, arity, preconditions, parameters);
        }

        public static CompoundExpression Compound(Expression predicate, params Expression[] arguments)
        {
            return new CompoundExpression(predicate, arguments);
        }
        public static CompoundExpression Compound(Expression predicate, IEnumerable<Expression> arguments)
        {
            return new CompoundExpression(predicate, arguments);
        }

        public static LambdaExpression Lambda(string module, string name, IEnumerable<CompoundExpression> preconditions, Expression body, IEnumerable<CompoundExpression> effects_remove, IEnumerable<CompoundExpression> effects_add, IEnumerable<VariableExpression> parameters)
        {
            return new LambdaExpression(module, name, preconditions, body, effects_remove, effects_add, parameters);
        }



        public virtual Type Type
        {
            get
            {
                return typeof(object);
            }
        }



        internal virtual Expression Replace(Expression[] from, Expression[] to)
        {
            int index = Array.IndexOf(from, this);
            return (index >= 0) ? to[index] : this;
        }
    }

    public class ConstantExpression : Expression
    {
        internal ConstantExpression(object value, Type type)
        {
            m_value = value;
            m_type = type;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object m_value;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Type m_type;

        public object Value => m_value;
        public override Type Type => m_type;
    }

    public class VariableExpression : Expression
    {
        internal VariableExpression(bool value)
        {
            m_constraints = m_emptyCompounds;
            m_parameter = this;
        }
        internal VariableExpression(IEnumerable<CompoundExpression> constraints, VariableExpression parameter)
        {
            m_constraints = constraints.ToList().AsReadOnly();
            m_parameter = parameter;
        }
        internal VariableExpression()
        {
            m_constraints = Expression.m_emptyCompounds;
            m_parameter = Expression.m_trueVariable;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<CompoundExpression> m_constraints;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        VariableExpression m_parameter;

        public IReadOnlyList<CompoundExpression> Constraints
        {
            get
            {
                return m_constraints;
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
            bool any = false;
            Expression e;
            CompoundExpression n;
            VariableExpression p;
            VariableExpression pn;
            int index;
            int count = Constraints.Count;
            List<CompoundExpression> nc = new List<CompoundExpression>(count);

            index = Array.IndexOf(from, this);
            if (index >= 0) return to[index];

            if(count > 0)
            {
                p = Parameter;
                if (!object.ReferenceEquals(p, this))
                {
                    pn = p.Replace(from, to) as VariableExpression;
                    if (!object.ReferenceEquals(p, pn))
                    {
                        any = true;
                    }
                }
                else
                {
                    pn = p;
                }
            }
            else
            {
                pn = Parameter;
            }

            for (index = 0; index < count; index++)
            {
                e = Constraints[index];
                n = e.Replace(from, to) as CompoundExpression;
                if (!object.ReferenceEquals(e, n))
                {
                    any = true;
                }
                nc.Add(n);
            }

            if (!any) return this;

            return Expression.Variable(nc, pn);
        }
    }

    public class SetExpression : Expression
    {
        public SetExpression(IEnumerable<CompoundExpression> definition, VariableExpression parameter)
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

    public class PredicateExpression : Expression
    {
        internal PredicateExpression(string module, string name, int arity)
        {
            Contract.Requires(name != null);
            Contract.Requires(arity >= 0);

            m_module = module;
            m_name = name;
            m_arity = arity;
            m_preconditions = Expression.m_emptyCompounds;
            if(arity == 0)
            {
                m_parameters = Expression.m_emptyVariables;
            }
            else
            {
                List<VariableExpression> p = new List<VariableExpression>(arity);
                for(int i = 0; i < arity; i++)
                {
                    p.Add(Expression.Variable());
                }
                m_parameters = p.AsReadOnly();
            }
        }
        internal PredicateExpression(string module, string name, int arity, IEnumerable<CompoundExpression> preconditions, IEnumerable<VariableExpression> parameters)
        {
            Contract.Requires(name != null);
            Contract.Requires(arity >= 0);
            Contract.Requires(preconditions != null);
            Contract.Requires(parameters != null);

            m_module = module;
            m_name = name;
            m_arity = arity;

            if(arity == 0)
            {
                m_parameters = Expression.m_emptyVariables;
            }
            else
            {
                m_parameters = parameters.ToList().AsReadOnly();
            }

            Contract.Requires(m_parameters.Count == arity);

            m_preconditions = preconditions.ToList().AsReadOnly();
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string m_module;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string m_name;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int m_arity;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<CompoundExpression> m_preconditions;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<VariableExpression> m_parameters;

        public string Module
        {
            get
            {
                return m_module;
            }
        }
        public string Name
        {
            get
            {
                return m_name;
            }
        }
        public int Arity
        {
            get
            {
                return m_arity;
            }
        }

        public IReadOnlyList<VariableExpression> Parameters
        {
            get
            {
                return m_parameters;
            }
        }
        public IReadOnlyList<CompoundExpression> Preconditions
        {
            get
            {
                return m_preconditions;
            }
        }

        internal override Expression Replace(Expression[] from, Expression[] to)
        {
            bool any = false;
            Expression e;
            Expression n;
            List<VariableExpression> p;
            List<CompoundExpression> c;
            int index;
            int count;

            index = Array.IndexOf(from, this);
            if (index >= 0) return to[index];

            count = Parameters.Count;
            p = new List<VariableExpression>(count);

            for (index = 0; index < count; index++)
            {
                e = Parameters[index];
                n = e.Replace(from, to);
                if (!object.ReferenceEquals(e, n))
                {
                    any = true;
                }
                p.Add((VariableExpression)n);
            }

            count = Preconditions.Count;
            c = new List<CompoundExpression>(count);

            for (index = 0; index < count; index++)
            {
                e = Preconditions[index];
                n = e.Replace(from, to);
                if (!object.ReferenceEquals(e, n))
                {
                    any = true;
                }
                c.Add((CompoundExpression)n);
            }

            if (!any) return this;
            return Expression.Predicate(Module, Name, Arity, c, p);
        }
    }

    public class CompoundExpression : Expression
    {
        public CompoundExpression(Expression predicate, IEnumerable<Expression> arguments)
        {
            m_predicate = predicate;
            m_arguments = arguments.ToList().AsReadOnly();
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Expression m_predicate;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<Expression> m_arguments;

        public new Expression Predicate
        {
            get
            {
                return m_predicate;
            }
        }
        public IReadOnlyList<Expression> Arguments
        {
            get
            {
                return m_arguments;
            }
        }

        internal override Expression Replace(Expression[] from, Expression[] to)
        {
            bool any = false;
            Expression e;
            Expression n;
            Expression p;
            List<Expression> args;
            int index;
            int count;

            index = Array.IndexOf(from, this);
            if (index >= 0) return to[index];

            e = Predicate;
            p = e.Replace(from, to);

            if (!object.ReferenceEquals(e, p))
            {
                any = true;
            }

            count = Arguments.Count;
            args = new List<Expression>(count);
            for (index = 0; index < count; ++index)
            {
                e = Arguments[index];
                n = e.Replace(from, to);
                if (!object.ReferenceEquals(e, n))
                {
                    any = true;
                }
                args.Add(n);
            }
            if (!any) return this;
            else return Expression.Compound(p, args);
        }

        //public override string ToString()
        //{
        //    string retval = Predicate.ToString();
        //    int count = Arguments.Count;
        //    if(count > 0)
        //    {
        //        retval += "(" + Arguments[0].ToString();
        //        for(int i = 1; i < count; i++)
        //        {
        //            retval += "," + Arguments[i].ToString();
        //        }
        //        retval += ")";
        //    }
        //    return retval;
        //}
    }

    public class LambdaExpression : Expression, IAction
    {
        internal class Delta : IDelta<CompoundExpression>
        {
            internal Delta(IEnumerable<CompoundExpression> remove, IEnumerable<CompoundExpression> add)
            {
                m_remove = remove.ToList().AsReadOnly();
                m_add = add.ToList().AsReadOnly();
            }

            IReadOnlyList<CompoundExpression> m_remove;
            IReadOnlyList<CompoundExpression> m_add;

            public IReadOnlyList<CompoundExpression> Remove => m_remove;

            public IReadOnlyList<CompoundExpression> Add => m_add;
        }

        internal LambdaExpression(string module, string name, IEnumerable<CompoundExpression> preconditions, Expression body, IEnumerable<CompoundExpression> effects_remove, IEnumerable<CompoundExpression> effects_add, IEnumerable<VariableExpression> parameters)
        {
            Contract.Requires(preconditions != null);
            Contract.Requires(body != null);
            Contract.Requires(effects_remove != null);
            Contract.Requires(effects_add != null);
            Contract.Requires(parameters != null);

            m_module = module;
            m_name = name;
            m_preconditions = preconditions.ToList().AsReadOnly();
            m_body = body;
            m_effects = new Delta(effects_remove, effects_add);
            m_parameters = parameters.ToList().AsReadOnly();
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string m_module;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string m_name;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<VariableExpression> m_parameters;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<CompoundExpression> m_preconditions;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Expression m_body;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IDelta<CompoundExpression> m_effects;

        public string Module
        {
            get
            {
                return m_module;
            }
        }
        public string Name
        {
            get
            {
                return m_name;
            }
        }

        public IReadOnlyList<VariableExpression> Parameters
        {
            get
            {
                return m_parameters;
            }
        }
        public IReadOnlyList<CompoundExpression> Preconditions
        {
            get
            {
                return m_preconditions;
            }
        }
        public Expression Body
        {
            get
            {
                return m_body;
            }
        }
        public IDelta<CompoundExpression> Effects
        {
            get
            {
                return m_effects;
            }
        }

        public Type ReturnType
        {
            get
            {
                return m_body.Type;
            }
        }

        internal override Expression Replace(Expression[] from, Expression[] to)
        {
            throw new NotImplementedException();
        }
    }
}