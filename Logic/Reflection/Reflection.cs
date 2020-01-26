using Logic.Expressions;
using System;
using System.Reflection;

namespace Logic.Reflection
{
    public static class Builtin
    {
        public static RuleInfo GetRule(string qualifiedName)
        {
            int length = qualifiedName.Length;
            int firstComma = qualifiedName.IndexOf(',');
            string assemblyName = qualifiedName.Substring(firstComma + 1, length - (firstComma + 1)).TrimStart();
            string path = qualifiedName.Substring(0, firstComma);
            return Assembly.Load(assemblyName).GetRule(path);
        }
        public static FunctorInfo GetFunctor(string qualifiedName)
        {
            int length = qualifiedName.Length;
            int firstComma = qualifiedName.IndexOf(',');
            string assemblyName = qualifiedName.Substring(firstComma + 1, length - (firstComma + 1)).TrimStart();
            string path = qualifiedName.Substring(0, firstComma);
            return Assembly.Load(assemblyName).GetFunctor(path);
        }

        public static RuleInfo GetRule(this Assembly assembly, string name)
        {
            int lastIndexOf = name.LastIndexOf('.');
            string domainTypeName = name.Substring(0, lastIndexOf);
            string ruleName = name.Substring(lastIndexOf + 1, name.Length - (lastIndexOf + 1));
            Type domainType = assembly.GetType(domainTypeName);
            //DomainAttribute domainAttribute = domainType.GetCustomAttribute<DomainAttribute>();
            //if (domainAttribute == null) throw new ArgumentException("", nameof(name));

            MethodInfo m = domainType.GetMethod(ruleName, BindingFlags.Public | BindingFlags.Static);

            // TO DO: parse a RuleExpression from an attribute's text value
            return new RuntimeRuleInfo(domainType, ruleName, null);
        }
        public static FunctorInfo GetFunctor(this Assembly assembly, string name)
        {
            int lastIndexOf = name.LastIndexOf('.');
            string domainTypeName = name.Substring(0, lastIndexOf);
            string propertyName = name.Substring(lastIndexOf + 1, name.Length - (lastIndexOf + 1));
            Type domainType = assembly.GetType(domainTypeName);
            //DomainAttribute domainAttribute = domainType.GetCustomAttribute<DomainAttribute>();
            //if (domainAttribute == null) throw new ArgumentException("", nameof(name));

            PropertyInfo p = domainType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
            //FunctorAttribute functorAttribute = p.GetCustomAttribute<FunctorAttribute>();
            //if (functorAttribute == null) throw new ArgumentException("", nameof(name));

            Type shim = p.PropertyType;
            var dma = shim.GetCustomAttribute<DefaultMemberAttribute>();
            string mn = dma.MemberName;
            p = shim.GetProperty(mn, BindingFlags.Public | BindingFlags.Instance);

            var functorType = p.PropertyType;
            var ps = p.GetIndexParameters();
            int length = ps.Length;
            ParameterInfo[] parameters = new ParameterInfo[length - 1];
            for (int i = 1; i < length; ++i)
                parameters[i - 1] = ps[i];

            return new RuntimeFunctorInfo(domainType, propertyName, functorType, parameters);
        }
    }

    internal sealed class RuntimeRuleInfo : RuleInfo
    {
        public RuntimeRuleInfo(Type type, string name, RuleExpression expression)
        {
            m_type = type;
            m_name = name;
            m_expr = expression;
        }
        readonly Type m_type;
        readonly string m_name;
        readonly RuleExpression m_expr;

        public override string Name => m_name;

        public override RuleExpression Expression => m_expr;

        public override Type DeclaringType => m_type;

        public override Type ReflectedType => m_type;

        public override object[] GetCustomAttributes(bool inherit)
        {
            return m_type.GetMember(m_name, BindingFlags.Public | BindingFlags.Static)[0].GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return m_type.GetMember(m_name, BindingFlags.Public | BindingFlags.Static)[0].GetCustomAttributes(attributeType, inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return m_type.GetMember(m_name, BindingFlags.Public | BindingFlags.Static)[0].IsDefined(attributeType, inherit);
        }
    }

    internal sealed class RuntimeFunctorInfo : FunctorInfo
    {
        public RuntimeFunctorInfo(Type type, string name, Type functorType, ParameterInfo[] parameters)
        {
            m_type = type;
            m_name = name;
            m_functorType = functorType;
            m_parameters = parameters;
        }
        readonly Type m_type;
        readonly string m_name;
        readonly Type m_functorType;
        readonly ParameterInfo[] m_parameters;

        public override string Name => m_name;

        public override Type FunctorType => m_functorType;

        public override Type DeclaringType => m_type;

        public override Type ReflectedType => m_type;

        public override ParameterInfo[] GetParameters()
        {
            return (ParameterInfo[])m_parameters.Clone();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return m_type.GetMember(m_name, BindingFlags.Public | BindingFlags.Static)[0].GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return m_type.GetMember(m_name, BindingFlags.Public | BindingFlags.Static)[0].GetCustomAttributes(attributeType, inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return m_type.GetMember(m_name, BindingFlags.Public | BindingFlags.Static)[0].IsDefined(attributeType, inherit);
        }
    }

    public abstract class RuleInfo : ICustomAttributeProvider, IEquatable<RuleInfo>
    {
        public abstract Type DeclaringType { get; }

        public abstract Type ReflectedType { get; }

        public abstract RuleExpression Expression { get; }

        //public FunctorInfo[] GetDependencies()
        //{
        //    throw new NotImplementedException();
        //}

        public abstract string Name { get; }

        // MethodInfo Method { get; }

        public override bool Equals(object obj)
        {
            if (obj is RuleInfo other)
            {
                return object.ReferenceEquals(this, other) || Equals(other);
            }
            else
            {
                return false;
            }
        }
        public virtual bool Equals(RuleInfo other)
        {
            if (!DeclaringType.Equals(other.DeclaringType)) return false;
            if (!Name.Equals(other.Name)) return false;
            return true;
        }

        public abstract object[] GetCustomAttributes(bool inherit);

        public abstract object[] GetCustomAttributes(Type attributeType, bool inherit);

        public abstract bool IsDefined(Type attributeType, bool inherit);
    }

    public abstract class FunctorInfo : ICustomAttributeProvider, IEquatable<FunctorInfo>
    {
        public abstract Type FunctorType { get; }

        public abstract ParameterInfo[] GetParameters();

        public abstract Type DeclaringType { get; }

        public abstract Type ReflectedType { get; }

        public abstract string Name { get; }

        public override bool Equals(object obj)
        {
            if (obj is FunctorInfo other)
            {
                return object.ReferenceEquals(this, other) || Equals(other);
            }
            else
            {
                return false;
            }
        }
        public virtual bool Equals(FunctorInfo other)
        {
            if (!DeclaringType.Equals(other.DeclaringType)) return false;
            if (!Name.Equals(other.Name)) return false;
            return true;
        }

        public abstract object[] GetCustomAttributes(bool inherit);

        public abstract object[] GetCustomAttributes(Type attributeType, bool inherit);

        public abstract bool IsDefined(Type attributeType, bool inherit);
    }
}