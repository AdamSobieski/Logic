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

            return new RuntimeRuleInfo(assembly, domainTypeName, ruleName, null);
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
            //FunctorAttribute fa = p.GetCustomAttribute<FunctorAttribute>();
            //if (fa == null) throw new ArgumentException("", nameof(name));

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

            return new RuntimeFunctorInfo(assembly, domainTypeName, propertyName, functorType, parameters);
        }
    }

    internal sealed class RuntimeRuleInfo : RuleInfo
    {
        public RuntimeRuleInfo(Assembly assembly, string ns, string name, RuleExpression expression)
        {
            m_assembly = assembly;
            m_ns = ns;
            m_name = name;
            m_expr = expression;
        }
        readonly Assembly m_assembly;
        readonly string m_ns;
        readonly string m_name;
        readonly RuleExpression m_expr;

        public override Assembly Assembly => m_assembly;

        public override string Name => m_name;

        public override string Namespace => m_ns;

        public override RuleExpression Expression => m_expr;

        public override FunctorInfo[] GetDependencies()
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class RuntimeFunctorInfo : FunctorInfo
    {
        public RuntimeFunctorInfo(Assembly assembly, string ns, string name, Type functorType, ParameterInfo[] parameters)
        {
            m_assembly = assembly;
            m_ns = ns;
            m_name = name;
            m_functorType = functorType;
            m_parameters = parameters;
        }
        readonly Assembly m_assembly;
        readonly string m_ns;
        readonly string m_name;
        readonly Type m_functorType;
        readonly ParameterInfo[] m_parameters;

        public override Assembly Assembly => m_assembly;

        public override string Name => m_name;

        public override string Namespace => m_ns;

        public override Type FunctorType => m_functorType;

        public override ParameterInfo[] GetParameters()
        {
            return (ParameterInfo[])m_parameters.Clone();
        }
    }

    public abstract class RuleInfo : IEquatable<RuleInfo>
    {
        public abstract Assembly Assembly { get; }

        public abstract string Name { get; }
        public abstract string Namespace { get; }
        public string FullName => Namespace + "." + Name;

        public abstract FunctorInfo[] GetDependencies();

        public abstract RuleExpression Expression { get; }

        public override bool Equals(object obj)
        {
            if (obj is RuleInfo other)
            {
                return Equals(other);
            }
            else
            {
                return false;
            }
        }
        public bool Equals(RuleInfo other)
        {
            if (!Assembly.Equals(other.Assembly)) return false;
            if (!FullName.Equals(other.FullName)) return false;
            return true;
        }
    }

    public abstract class FunctorInfo : IEquatable<FunctorInfo>
    {
        public abstract Assembly Assembly { get; }

        public abstract string Name { get; }
        public abstract string Namespace { get; }
        public string FullName => Namespace + "." + Name;

        public abstract Type FunctorType { get; }
        public abstract ParameterInfo[] GetParameters();

        public override bool Equals(object obj)
        {
            if (obj is FunctorInfo other)
            {
                return Equals(other);
            }
            else
            {
                return false;
            }
        }
        public bool Equals(FunctorInfo other)
        {
            if (!Assembly.Equals(other.Assembly)) return false;
            if (!FullName.Equals(other.FullName)) return false;
            return true;
        }
    }
}
