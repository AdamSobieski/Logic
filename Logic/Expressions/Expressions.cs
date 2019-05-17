/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using Logic.Knowledge;
using Logic.Planning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Logic.Expressions
{
    internal struct Atom
    {
        public Atom(string name)
        {
            Contract.Requires(name != null);

            m_name = name;
        }

        readonly string m_name;
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
        internal static VariableExpression m_falseVariable = new VariableExpression(false);

        internal static SetExpression m_universalSet = Set(new VariableExpression[] { m_trueVariable }, m_emptyCompounds);
        internal static SetExpression m_emptySet = Set(new VariableExpression[] { m_trueVariable }, new CompoundExpression[] { m_falseCompound });



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
            return new ConstantExpression(value, null);
        }

        public static ConstantExpression Integer(int value)
        {
            return new ConstantExpression(value, null);
        }
        public static ConstantExpression Float(double value)
        {
            return new ConstantExpression(value, null);
        }
        public static ConstantExpression Atom(string name)
        {
            return new ConstantExpression(new Atom(name), null);
        }

        public static VariableExpression Variable()
        {
            return new VariableExpression();
        }
        public static VariableExpression Variable(IEnumerable<VariableExpression> parameters, IEnumerable<CompoundExpression> constraints)
        {
            Contract.Requires(parameters != null);
            Contract.Requires(constraints != null);

            return new VariableExpression(parameters, constraints);
        }

        public static SetExpression Set(IEnumerable<VariableExpression> parameters, IEnumerable<CompoundExpression> constraints)
        {
            Contract.Requires(parameters != null);
            Contract.Requires(constraints != null);

            return new SetExpression(parameters, constraints);
        }

        public static PredicateExpression Predicate(string module, string name, int arity)
        {
            return new PredicateExpression(module, name, arity);
        }
        public static PredicateExpression Predicate(string module, string name, int arity, IEnumerable<VariableExpression> parameters, IEnumerable<CompoundExpression> constraints)
        {
            return new PredicateExpression(module, name, arity, parameters, constraints);
        }

        public static CompoundExpression Compound(Expression predicate, params Expression[] arguments)
        {
            return new CompoundExpression(predicate, arguments);
        }
        public static CompoundExpression Compound(Expression predicate, IEnumerable<Expression> arguments)
        {
            return new CompoundExpression(predicate, arguments);
        }

        public static LambdaExpression Lambda(Expression atom, IEnumerable<CompoundExpression> preconditions, Expression body, IEnumerable<CompoundExpression> effects_remove, IEnumerable<CompoundExpression> effects_add, IEnumerable<VariableExpression> parameters, IEnumerable<CompoundExpression> constraints)
        {
            return new LambdaExpression(atom, preconditions, body, effects_remove, effects_add, parameters, constraints);
        }



        public virtual ClassExpression Type
        {
            get
            {
                return null;
            }
        }



        internal abstract Expression Replace(Expression[] from, Expression[] to);
    }

    public class ConstantExpression : Expression
    {
        internal ConstantExpression(object value, ClassExpression type)
        {
            m_value = value;
            m_type = type;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly object m_value;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly ClassExpression m_type;

        public object Value
        {
            get
            {
                return m_value;
            }
        }
        public override ClassExpression Type
        {
            get
            {
                return m_type;
            }
        }

        internal override Expression Replace(Expression[] from, Expression[] to)
        {
            int index = Array.IndexOf(from, this);
            return (index >= 0) ? to[index] : this;
        }
    }

    // public class TupleExpression
    // public class ListExpression
    // public class VectorExpression

    public class VariableExpression : Expression
    {
        internal VariableExpression(bool value)
        {
            if (value)
            {
                m_constraints = m_emptyCompounds;
                m_parameters = new VariableExpression[] { this };
            }
            else
            {
                m_constraints = new CompoundExpression[] { m_falseCompound };
                m_parameters = new VariableExpression[] { Expression.m_trueVariable };
            }
        }
        internal VariableExpression(IEnumerable<VariableExpression> parameters, IEnumerable<CompoundExpression> constraints)
        {
            m_constraints = constraints.ToList().AsReadOnly();
            m_parameters = parameters.ToList().AsReadOnly();
        }
        internal VariableExpression()
        {
            m_constraints = Expression.m_emptyCompounds;
            m_parameters = new VariableExpression[] { Expression.m_trueVariable };
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IReadOnlyList<VariableExpression> m_parameters;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IReadOnlyList<CompoundExpression> m_constraints;

        public IReadOnlyList<VariableExpression> Parameters
        {
            get
            {
                return m_parameters;
            }
        }
        public IReadOnlyList<CompoundExpression> Constraints
        {
            get
            {
                return m_constraints;
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

            count = Constraints.Count;
            c = new List<CompoundExpression>(count);

            for (index = 0; index < count; index++)
            {
                e = Constraints[index];
                n = e.Replace(from, to);
                if (!object.ReferenceEquals(e, n))
                {
                    any = true;
                }
                c.Add((CompoundExpression)n);
            }

            if (!any) return this;
            return Variable(p, c);
        }
    }

    public class SetExpression : Expression
    {
        internal SetExpression(IEnumerable<VariableExpression> parameters, IEnumerable<CompoundExpression> constraints)
        {
            this.m_constraints = constraints.ToList().AsReadOnly();
            this.m_parameters = parameters.ToList().AsReadOnly();
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IReadOnlyList<VariableExpression> m_parameters;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IReadOnlyList<CompoundExpression> m_constraints;

        public IReadOnlyList<VariableExpression> Parameters
        {
            get
            {
                return m_parameters;
            }
        }
        public IReadOnlyList<CompoundExpression> Constraints
        {
            get
            {
                return m_constraints;
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

            count = Constraints.Count;
            c = new List<CompoundExpression>(count);

            for (index = 0; index < count; index++)
            {
                e = Constraints[index];
                n = e.Replace(from, to);
                if (!object.ReferenceEquals(e, n))
                {
                    any = true;
                }
                c.Add((CompoundExpression)n);
            }

            if (!any) return this;
            return Set(p, c);
        }
    }

    public class ClassExpression : Expression // SetExpression?
    {
        internal ClassExpression(IEnumerable<VariableExpression> parameters, IEnumerable<CompoundExpression> constraints, VariableExpression descriptionParameter, IEnumerable<CompoundExpression> description)
        {
            m_parameters = parameters.ToList().AsReadOnly();
            m_constraints = constraints.ToList().AsReadOnly();
            m_descriptionParameter = descriptionParameter;
            m_description = description.ToList().AsReadOnly();
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IReadOnlyList<VariableExpression> m_parameters;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IReadOnlyList<CompoundExpression> m_constraints;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly VariableExpression m_descriptionParameter;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IReadOnlyList<CompoundExpression> m_description;

        public IReadOnlyList<VariableExpression> Parameters
        {
            get
            {
                return m_parameters;
            }
        }
        public IReadOnlyList<CompoundExpression> Constraints
        {
            get
            {
                return m_constraints;
            }
        }

        public VariableExpression DescriptionParameter
        {
            get
            {
                return m_descriptionParameter;
            }
        }
        public IReadOnlyList<CompoundExpression> Description
        {
            get
            {
                return m_description;
            }
        }

        // public IReadOnlyList<PredicateExpression> Predicates { get; }

        internal override Expression Replace(Expression[] from, Expression[] to)
        {
            throw new NotImplementedException();
        }
    }

    public interface IThing
    {
        IKnowledgebaseModule Knowledgebase { get; }

        ClassExpression GetClass();

        object this[PredicateExpression predicate] { get; set; }
    }

    public interface IExpando : IThing
    {
        bool Add(PredicateExpression predicate);
        bool Remove(PredicateExpression predicate);
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
            m_constraints = Expression.m_emptyCompounds;
            if (arity == 0)
            {
                m_parameters = Expression.m_emptyVariables;
            }
            else
            {
                List<VariableExpression> p = new List<VariableExpression>(arity);
                for (int i = 0; i < arity; i++)
                {
                    p.Add(Expression.Variable());
                }
                m_parameters = p.AsReadOnly();
            }
        }
        internal PredicateExpression(string module, string name, int arity, IEnumerable<VariableExpression> parameters, IEnumerable<CompoundExpression> constraints)
        {
            Contract.Requires(name != null);
            Contract.Requires(arity >= 0);
            Contract.Requires(parameters != null);
            Contract.Requires(constraints != null);

            m_module = module;
            m_name = name;
            m_arity = arity;

            if (arity == 0)
            {
                m_parameters = Expression.m_emptyVariables;
            }
            else
            {
                m_parameters = parameters.ToList().AsReadOnly();
            }

            Contract.Requires(m_parameters.Count == arity);

            m_constraints = constraints.ToList().AsReadOnly();
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly string m_module;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly string m_name;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly int m_arity;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IReadOnlyList<VariableExpression> m_parameters;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IReadOnlyList<CompoundExpression> m_constraints;

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
        public IReadOnlyList<CompoundExpression> Constraints
        {
            get
            {
                return m_constraints;
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

            count = Constraints.Count;
            c = new List<CompoundExpression>(count);

            for (index = 0; index < count; index++)
            {
                e = Constraints[index];
                n = e.Replace(from, to);
                if (!object.ReferenceEquals(e, n))
                {
                    any = true;
                }
                c.Add((CompoundExpression)n);
            }

            if (!any) return this;
            return Expression.Predicate(Module, Name, Arity, p, c);
        }
    }

    public class CompoundExpression : Expression
    {
        internal CompoundExpression(Expression predicate, IEnumerable<Expression> arguments)
        {
            m_predicate = predicate;
            m_arguments = arguments.ToList().AsReadOnly();
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Expression m_predicate;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IReadOnlyList<Expression> m_arguments;

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

            readonly IReadOnlyList<CompoundExpression> m_remove;
            readonly IReadOnlyList<CompoundExpression> m_add;

            public IReadOnlyList<CompoundExpression> Remove => m_remove;

            public IReadOnlyList<CompoundExpression> Add => m_add;
        }

        internal LambdaExpression(Expression atom, IEnumerable<CompoundExpression> preconditions, Expression body, IEnumerable<CompoundExpression> effects_remove, IEnumerable<CompoundExpression> effects_add, IEnumerable<VariableExpression> parameters, IEnumerable<CompoundExpression> constraints)
        {
            Contract.Requires(preconditions != null);
            Contract.Requires(body != null);
            Contract.Requires(effects_remove != null);
            Contract.Requires(effects_add != null);
            Contract.Requires(parameters != null);
            Contract.Requires(constraints != null);

            m_expression = atom;
            m_preconditions = preconditions.ToList().AsReadOnly();
            m_body = body;
            m_effects = new Delta(effects_remove, effects_add);
            m_parameters = parameters.ToList().AsReadOnly();
            m_constraints = constraints.ToList().AsReadOnly();
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Expression m_expression;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IReadOnlyList<VariableExpression> m_parameters;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IReadOnlyList<CompoundExpression> m_constraints;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IReadOnlyList<CompoundExpression> m_preconditions;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Expression m_body;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IDelta<CompoundExpression> m_effects;

        public Expression Expression
        {
            get
            {
                return m_expression;
            }
        }

        //public string Name
        //{
        //    get
        //    {
        //        return ((Atom)(Expression as ConstantExpression).Value).Name;
        //    }
        //}

        public IReadOnlyList<VariableExpression> Parameters
        {
            get
            {
                return m_parameters;
            }
        }
        public IReadOnlyList<CompoundExpression> Constraints
        {
            get
            {
                return m_constraints;
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

        public ClassExpression ReturnType
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

    public interface IVariableBindings
    {
        IList<VariableExpression> Variables { get; }
        IList<Expression> Bindings { get; }
    }

    public interface IReadOnlyVariableBindings
    {
        IReadOnlyList<VariableExpression> Variables { get; }
        IReadOnlyList<Expression> Bindings { get; }
    }
}